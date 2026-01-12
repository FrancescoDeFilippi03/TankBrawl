using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;

public class Tank : NetworkBehaviour, IDamageble
{
    // === References ===
    [SerializeField] private TankStateManager tankStateManager;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform weaponPivotTransform;
    [SerializeField] private ShootingSystem shootingSystem;
    [SerializeField] private TankConfig tankConfig;
    


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
    public NetworkVariable<float> visualAlpha = new NetworkVariable<float>(
        1f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private float MaxHealth;
    private float MaxShield;

    // === Movement ===
    public event Action<Vector2> OnDashPerformed;
    public event Action OnDashCooldownStarted;
    public event Action<float> OnMovementSpeedChanged;

    [SerializeField] private float movementSmoothing = 5f;
    [SerializeField] private float rotationSmoothing = 7f;

    private Vector2 smoothedMovementInput;
    private Vector2 currentVelocity = Vector2.zero;
    private float movementSpeed;
    private float dashSpeed;
    private float dashDuration;

    // === Shooting ===
    public event Action<Vector2> OnShootPerformed;
    public event Action<WeaponData> OnWeaponChanged;
    public event Action OnAmmoEmpty;

    [SerializeField] private float turretRotationSmoothing = 7f;
    private Vector2 aimDirection;

    // === Properties ===
    public Rigidbody2D Rigidbody => rb;
    public NetworkVariable<float> Health => healthNetwork;
    public NetworkVariable<float> Shield => shieldNetwork;
    public float DashDuration => dashDuration;
    public Transform WeaponPivotTransform => weaponPivotTransform;
    public ShootingSystem ShootingSystem => shootingSystem;

    // === Other ===
    private TankConfigData tankConfigData;
    public TankConfigData TankConfigData => tankConfigData;
    public bool isRedTeam = false;


    public override void OnNetworkSpawn()
    {
        healthNetwork.OnValueChanged += HandleHealthChanged;
        visualAlpha.OnValueChanged += HandleAlphaChanged;

        InitializeTank(tankConfig);
    }

    public override void OnNetworkDespawn()
    {
        healthNetwork.OnValueChanged -= HandleHealthChanged;
        visualAlpha.OnValueChanged -= HandleAlphaChanged;
    }
    // ============================================
    // INITIALIZATION
    // ============================================
    public void InitializeTank(TankConfig tankConfig)
    {
        // Initialize shooting on all clients (needed for bullet pool)
        InitializeShooting(tankConfig.weaponData);
        
        // Initialize Health on server (needed for respawn)
        if (IsServer)
        {
            InitializeHealth(
                tankConfig.maxHealth,
                tankConfig.maxShield
            );
        }
        
        if(!IsOwner) return;

        tankConfigData = TeamManager.Instance.GetTankConfigDataForClient(OwnerClientId);

        isRedTeam = tankConfigData.Team == TeamColor.Red;

        CameraSetupOnSpawn();

        // Initialize Movement
        InitializeMovement(
            tankConfig.moveSpeed,
            tankConfig.dashSpeed,
            tankConfig.dashDuration
        );
    }

    // ============================================
    // HEALTH SYSTEM
    // ============================================

    private void HandleHealthChanged(float previousValue, float newValue)
    {
        OnHealthChanged?.Invoke(newValue, shieldNetwork.Value);
        
        if (newValue <= 0 && previousValue > 0)
        {
            if (IsOwner)
            {
                OnDeath?.Invoke();
            }
        }
    }

    public void InitializeHealth(float health, float shield)
    {
        MaxHealth = health;
        MaxShield = shield;

        healthNetwork.Value = MaxHealth;
        shieldNetwork.Value = MaxShield;
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

        if (shieldNetwork.Value > 0)
        {
            float shieldDamage = Mathf.Min(shieldNetwork.Value, damageAmount);
            shieldNetwork.Value -= shieldDamage;
            damageAmount -= shieldDamage;
        }
        if (damageAmount > 0)
        {
            healthNetwork.Value = Mathf.Max(healthNetwork.Value - damageAmount, 0);
            hitColor = Color.red;
        }

        // Sync color change to all clients
        ShowHitEffectClientRpc(hitColor);
        OnDamageTaken?.Invoke(totalDamage);
    }

    [ClientRpc]
    private void ShowHitEffectClientRpc(Color hitColor)
    {
        ChangeColorOnHit(hitColor);
        StartCoroutine(RevertColorCoroutine(0.1f));
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

    public void InitializeMovement(float movementSpeed, float dashSpeed, float dashDuration)
    {
        this.movementSpeed = movementSpeed;
        this.dashSpeed = dashSpeed;
        this.dashDuration = dashDuration;
    }

    public void MoveTank(Vector2 movementInput)
    {
        smoothedMovementInput = Vector2.SmoothDamp(
            smoothedMovementInput,
            movementInput,
            ref currentVelocity,
            1f / movementSmoothing
        );

        if (smoothedMovementInput.magnitude > 0.01f)
        {
            Vector2 movement = movementSpeed * Time.fixedDeltaTime * smoothedMovementInput;
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
        Vector2 dashVelocity = Time.fixedDeltaTime * dashSpeed * movementInput.normalized;
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

    public void InitializeShooting(WeaponData weaponData)
    {
        shootingSystem.InitializeWeapon(weaponData);
        CursorInitialization(weaponData.crosshairSprite);
        OnWeaponChanged?.Invoke(weaponData);
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
