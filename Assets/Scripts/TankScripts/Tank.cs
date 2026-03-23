using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using Unity.Services.Matchmaker.Models;
using TMPro;

public class Tank : NetworkBehaviour, IDamageble
{
    // === References ===
    [SerializeField] private TankStateManager tankStateManager;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform weaponPivotTransform;
    [SerializeField] private ShootingSystem shootingSystem;
    [SerializeField] private TankConfig tankConfig;
    public TankConfig TankConfig => tankConfig;

    // === Sprite References ===
    [Serializable]
    public struct TankSprites
    {
        public SpriteRenderer Body;
        public SpriteRenderer Turret;
        public SpriteRenderer LeftTrack;
        public SpriteRenderer RightTrack;
    } 
    [SerializeField] private TankSprites tankSprites;
    // === Animation ===
    [SerializeField]private Animator tankAnimator;
    public Animator TankAnimator => tankAnimator;

    // === Health ===
    public event Action OnDeath;
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnShieldChanged;
    public event Action<float> OnDamageTaken;

    public NetworkVariable<float> healthNetwork = new NetworkVariable<float>(
        100f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<float> shieldNetwork = new NetworkVariable<float>(
        50f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<int> currentAmmo = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<float> visualAlpha = new NetworkVariable<float>(
        1f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private float MaxHealth;
    private float MaxShield;
    private float shieldRegenRate;
    private float shieldRegenDelay;
    private float lastDamageTakenTime = -999f;

    // === Movement ===
    public event Action<Vector2> OnDashPerformed;
    public event Action OnDashCooldownStarted;
    public event Action<float> OnMovementSpeedChanged;

    [SerializeField] private float movementSmoothing = 5f;
    [SerializeField] private float rotationSmoothing = 7f;
    [SerializeField] private float baseMoveSpeed = 10f;

    [SerializeField] private float maxWeight = 80f; //in tons
    [SerializeField] private float minWeight = 20f; //in tons

    private Vector2 smoothedMovementInput;
    private Vector2 currentVelocity = Vector2.zero;

    // === Shooting ===
    public event Action<Vector2> OnShootPerformed;
    public event Action OnAmmoEmpty;

    [SerializeField] private float turretRotationSmoothing = 7f;
    private Vector2 aimDirection;

    // === Properties ===
    public Rigidbody2D Rigidbody => rb;
    public NetworkVariable<float> Health => healthNetwork;
    public NetworkVariable<float> Shield => shieldNetwork;
    public float DashDuration => 1f; // Placeholder, can be moved to TankConfig if needed
    public Transform WeaponPivotTransform => weaponPivotTransform;
    public ShootingSystem ShootingSystem => shootingSystem;

    // === Other ===
    private SessionPlayerData playerData;
    public SessionPlayerData PlayerData => playerData;
    public bool isRedTeam = false;

    // === Game Stats === 
    public NetworkVariable<int> kill = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<int> assist = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<int> death = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private ulong lastDamageAppliedFrom;


    [SerializeField] private GameObject playerNameText;
    

    public override void OnNetworkSpawn()
    {
        healthNetwork.OnValueChanged += HandleHealthChanged;
        shieldNetwork.OnValueChanged += HandleShieldChanged;
        visualAlpha.OnValueChanged += HandleAlphaChanged;

        InitializeTank(tankConfig);
    }

    public override void OnNetworkDespawn()
    {
        healthNetwork.OnValueChanged -= HandleHealthChanged;
        shieldNetwork.OnValueChanged -= HandleShieldChanged;
        visualAlpha.OnValueChanged -= HandleAlphaChanged;

        if (TankMainUI.Instance != null)
        {
            TankMainUI.Instance.UnsubscribeFromTank();
        }


        CursorReset();
    }

    // ============================================
    // INITIALIZATION
    // ============================================
    public void InitializeTank(TankConfig tankConfig)
    {
        
        playerData = SessionDataManager.Instance.GetPlayerData(OwnerClientId);
        // Initialize shooting on all clients (needed for bullet pool)
        InitializeShooting();
        
        // Initialize Health on server (needed for respawn)
        if (IsServer)
        {
            InitializeHealth(
                tankConfig.maxHealth,
                tankConfig.maxShield,
                tankConfig.shieldRegenRate,
                tankConfig.shieldRegenDelay
            );
        }

        var gameStats = FindAnyObjectByType<GameStatsUI>();
        if(gameStats != null)
        {
            gameStats.BindTank(this);
        }
        
        if(!IsOwner) return;

        isRedTeam = playerData.Team == TeamColor.Red;

        CameraSetupOnSpawn();

        // Connect to UI
        if (TankMainUI.Instance != null)
        {
            TankMainUI.Instance.SubscribeToTank(this);
        }

        //IN CASE I WANT ADD NAME TO TANK
        //SetName(playerData.PlayerName.ToString());
    }

    public void SetName(string name)
    {
        playerNameText.GetComponent<TMP_Text>().text = name;
    }

    // ============================================
    // HEALTH SYSTEM
    // ============================================

    private void HandleHealthChanged(float previousValue, float newValue)
    {
        OnHealthChanged?.Invoke(previousValue, newValue);
        
        if (newValue <= 0 && previousValue > 0)
        {
            if (IsOwner)
            {
                OnDeath?.Invoke();
            }
        }
    }

    private void HandleShieldChanged(float previousValue, float newValue)
    {
        OnShieldChanged?.Invoke(previousValue, newValue);
    }

    public void InitializeHealth(float health, float shield, float regenRate, float regenDelay)
    {
        MaxHealth = health;
        MaxShield = shield;
        shieldRegenRate = regenRate;
        shieldRegenDelay = regenDelay;

        healthNetwork.Value = MaxHealth;
        shieldNetwork.Value = MaxShield;
    }

    private void Update()
    {
        if (!IsServer) return;

        if (tankStateManager != null &&
            (tankStateManager.playerState.Value == TankStateManager.PlayerState.Dead ||
             tankStateManager.playerState.Value == TankStateManager.PlayerState.Respawn))
            return;

        CheckShieldRegen();
    }

    void CheckShieldRegen()
    {
        if (shieldNetwork.Value < MaxShield && Time.time >= lastDamageTakenTime + shieldRegenDelay)
        {
            shieldNetwork.Value = Mathf.Min(shieldNetwork.Value + shieldRegenRate * Time.deltaTime, MaxShield);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (!IsServer) return;

        if (tankStateManager != null && 
            (tankStateManager.playerState.Value == TankStateManager.PlayerState.Dead ||
             tankStateManager.playerState.Value == TankStateManager.PlayerState.Respawn))
        {
            return;
        }

        float totalDamage = damageAmount;
        Color hitColor = new Color(2f, 2f, 2f, 1f);

        lastDamageTakenTime = Time.time;

        if (shieldNetwork.Value > 0)
        {
            shieldNetwork.Value = Mathf.Max(shieldNetwork.Value - damageAmount, 0f);
        }
        else
        {
            healthNetwork.Value = Mathf.Max(healthNetwork.Value - damageAmount, 0);
            hitColor = Color.red;
        }

        if(healthNetwork.Value <= 0)
            ReportKill();

        ShowHitEffectClientRpc(hitColor);

        OnDamageTaken?.Invoke(totalDamage);
    }

    [ClientRpc]
    private void ShowHitEffectClientRpc(Color hitColor)
    {
        ChangeColorOnHit(hitColor);
        StartCoroutine(RevertColorCoroutine(0.1f));
    }

    public void RecordDamageFrom(ulong attackerId)
    {
        if (!IsServer) return;
        lastDamageAppliedFrom = attackerId;
    }

    public void ReportKill()
    {
        if (!IsServer) return;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(lastDamageAppliedFrom, out var killerClient))
        {
            var killerTank = killerClient.PlayerObject != null
                ? killerClient.PlayerObject.GetComponent<Tank>()
                : null;

            if (killerTank != null)
            {
                killerTank.kill.Value++;
            }
        }
    }

    public void ResetHealth()
    {
        if (IsOwner)
        {
            ResetHealthServerRpc();
        }
    }

    [ServerRpc]
    private void ResetHealthServerRpc()
    {
        healthNetwork.Value = MaxHealth;
        shieldNetwork.Value = MaxShield;
    }

    public void Heal(float healAmount)
    {
        if (!IsServer) return;
        healthNetwork.Value = Mathf.Min(healthNetwork.Value + healAmount, MaxHealth);
    }

    public void RechargeShield(float shieldAmount)
    {
        if (!IsServer) return;
        shieldNetwork.Value = Mathf.Min(shieldNetwork.Value + shieldAmount, MaxShield);
    }

    // ============================================
    // MOVEMENT SYSTEM
    // ============================================

    public void MoveTank(Vector2 movementInput)
    {
        smoothedMovementInput = Vector2.SmoothDamp(
            smoothedMovementInput,
            movementInput,
            ref currentVelocity,
            1f / movementSmoothing
        );

        float weightScale = (tankConfig.WeightScale - minWeight) / (maxWeight - minWeight);

        if (smoothedMovementInput.magnitude > 0.01f)
        {
            Vector2 movement = baseMoveSpeed * weightScale * Time.fixedDeltaTime * smoothedMovementInput;
            rb.MovePosition(rb.position + movement);
        }

    }

    public void HandleRotation(Vector2 inputDirection)
    {
        float targetAngle = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        Quaternion currentRotation = Quaternion.Euler(0, 0, rb.rotation);

        Quaternion newRotation = Quaternion.Slerp(
            currentRotation,
            targetRotation,
            rotationSmoothing * Time.fixedDeltaTime
        );

        rb.MoveRotation(newRotation.eulerAngles.z);
    }

    public void Dash(Vector2 movementInput)
    {
        Vector2 dashVelocity = Time.fixedDeltaTime * 20f* movementInput.normalized;
        rb.MovePosition(rb.position + dashVelocity);

        OnDashPerformed?.Invoke(movementInput.normalized);
    }

    public void ResetVelocity()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    // ============================================
    // SHOOTING SYSTEM
    // ============================================

    public void InitializeShooting()
    {
        shootingSystem.InitializeWeapon(this);
        CursorInitialization(TankConfig.crosshairSprite);
    }

    public void Shoot(bool isHoldingTrigger)
    {   

        shootingSystem.TryShoot(aimDirection, isHoldingTrigger);
        
        if (isHoldingTrigger)
        {
            OnShootPerformed?.Invoke(aimDirection);
        }
    }

    private Vector2 GetAimDirection(Vector2 aimInput)
    {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(aimInput.x, aimInput.y, 0f));
        Vector2 direction = (worldMousePosition - weaponPivotTransform.position).normalized;
        return direction;
    }

    public void HandleTurretRotation(Vector2 aimInput)
    {
        aimDirection = GetAimDirection(aimInput);

        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        weaponPivotTransform.rotation = Quaternion.Slerp(
            weaponPivotTransform.rotation,
            targetRotation,
            Time.fixedDeltaTime * turretRotationSmoothing
        );
    }

    private void CursorInitialization(Sprite crosshairSprite)
    {
        if (crosshairSprite != null)
        {
            Texture2D cursorTexture = crosshairSprite.texture;
            Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }
        Cursor.visible = true;
    }

    public void CursorReset()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }


    // ============================================
    // CAMERA SETUP
    // ============================================

    void CameraSetupOnSpawn()
    {
        var cameraInScene = FindAnyObjectByType<Unity.Cinemachine.CinemachineCamera>();
        cameraInScene.Target.TrackingTarget = this.transform;

        if (isRedTeam)
        {
            // Red team spawns facing down, rotate camera 180 degrees
            cameraInScene.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            // Blue team faces up, default rotation
            cameraInScene.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // ============================================
    // VISUALS
    // ============================================
    public void SetAlpha(float alpha)
    {
        if (IsOwner)
        {
            visualAlpha.Value = alpha;
        }
    }

    private void HandleAlphaChanged(float previousValue, float newValue)
    {
        ApplyAlphaToSprites(newValue);
    }

    private void ApplyAlphaToSprites(float alpha)
    {
        Color color = tankSprites.Body.color;
        color.a = alpha;
        tankSprites.Body.color = color;
        tankSprites.Turret.color = color;
        tankSprites.LeftTrack.color = color;
        tankSprites.RightTrack.color = color;
    }
    
    void ChangeColorOnHit(Color hitColor = default)
    {
        if (hitColor == default)
        {
            hitColor = Color.red;
        }
        tankSprites.Body.color = hitColor;
        tankSprites.Turret.color = hitColor;
        tankSprites.LeftTrack.color = hitColor;
        tankSprites.RightTrack.color = hitColor;

    }

    IEnumerator RevertColorCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Color normalColor = new Color(1f, 1f, 1f, visualAlpha.Value);
        tankSprites.Body.color = normalColor;
        tankSprites.Turret.color = normalColor;
        tankSprites.LeftTrack.color = normalColor;
        tankSprites.RightTrack.color = normalColor;
    }
}
