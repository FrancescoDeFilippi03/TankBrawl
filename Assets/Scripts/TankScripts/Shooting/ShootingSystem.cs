using Unity.Netcode;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ShootingSystem : NetworkBehaviour
{
 
    public BulletPool bulletPool;
    public Transform[] firePoints;
    [SerializeField] private Image reloadIndicator;
    public NetworkVariable<bool> isReloading = new NetworkVariable<bool>(
        false, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );
    //reloading
    private float reloadTimer = 0f;
    // Fire rate control
    private float fireCooldown = 0f;
    private bool hasFiredThisPress = false;

    private Tank tank;

    private TankConfig TankConfig => tank.TankConfig;

    private int currentAmmo { get => tank.currentAmmo.Value; set => tank.currentAmmo.Value = value; }
    int mask = 0;

    public void InitializeWeapon(Tank tank)
    {
        this.tank = tank;

        
        reloadTimer = 0f;

        int bulletLayer = (tank.PlayerData.Team == TeamColor.Red) ? LayerMask.NameToLayer("BulletRed") : LayerMask.NameToLayer("BulletBlue");
        Debug.Log("Team color: " + tank.PlayerData.Team + " , bullet layer: " + LayerMask.LayerToName(bulletLayer));
        mask = Physics2D.GetLayerCollisionMask(bulletLayer);

        if (TankConfig.BulletPrefab != null)
        {
            bulletPool.InitializePool(TankConfig.BulletPrefab, TankConfig.AmmoCapacity , tank.PlayerData.Team);
        }

        reloadIndicator.fillAmount = 0f;

        isReloading.OnValueChanged += HandleReloadStateChanged;

        if (IsServer)
        {
            currentAmmo = TankConfig.AmmoCapacity;
            isReloading.Value = false;
        }
            
    }
    
    void Update()
    {
        if (!IsOwner) return;
        
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

        UpdateClientReloadUI();
    }

    public void TryShoot(Vector2 shootDirection, bool isHoldingTrigger)
    {
        if (!IsOwner || TankConfig.BulletPrefab == null) return;

        if (!isHoldingTrigger)
        {
            hasFiredThisPress = false;
            return;
        }

        if (!CanFire()) return;

        switch (TankConfig.weaponData.shootingType)
        {
            case ShootingType.Single:
                if (!hasFiredThisPress)
                {
                    Shoot(shootDirection);
                    fireCooldown = 1f / TankConfig.FireRate;
                    hasFiredThisPress = true;
                }
                break;
            
            case ShootingType.Automatic:
                Shoot(shootDirection);
                fireCooldown = 1f / TankConfig.FireRate;
                break;
                
            case ShootingType.Burst:
                StartCoroutine(BurstFire(shootDirection));
                fireCooldown = 1f / TankConfig.FireRate;
                break;
        }
    }
    
    private bool CanFire()
    {
        return fireCooldown <= 0;
    }
    private IEnumerator BurstFire(Vector2 shootDirection)
    {
        for (int i = 0; i < TankConfig.weaponData.burstCount; i++)
        {
            Shoot(shootDirection);
            
            if (i < TankConfig.BurstCount - 1) 
            {
                yield return new WaitForSeconds(TankConfig.weaponData.burstDelay);
            }
        }
    }

    void Shoot(Vector2 shootDirection)
    {
        Vector2 dir = shootDirection.normalized;

        for (int i = 0; i < firePoints.Length; i++)
        {
            if (firePoints[i] != null)
            {
                Vector2 origin = firePoints[i].position;
                ShootClientAuthoritative(origin, dir, mask , TankConfig.Range);
            }
        }
    }

    private void ShootClientAuthoritative(Vector2 origin, Vector2 dir, int mask = 0 , float range = 0f)
    {
        if (currentAmmo <= 0 ) return;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, range, mask);

        if(hit.collider != null)
        {
            if (hit.collider.TryGetComponent<IDamageble>(out var damageble))
            {
                damageble.TakeDamage(TankConfig.Damage);
            }

            Debug.Log($"Bullet hit: {hit.collider.name} at distance: {hit.distance}");
        }
        float length = hit.collider != null ? hit.distance : range;

        if (hit.collider != null && hit.collider.TryGetComponent<NetworkObject>(out var netObj))
        {
            if(netObj.TryGetComponent<Tank>(out var hitTank))
            {
                ReportHitAndApplyDamageServerRpc(origin, dir, length, netObj.NetworkObjectId);
                hitTank.ShowHitEffectServerRpc();
            }
        }

        SpawnBulletVisual(origin, dir , length);
    }



    [ServerRpc]
    private void ReportHitAndApplyDamageServerRpc(Vector2 origin, Vector2 dir,float length , ulong hitObjectId)
    {
        if (currentAmmo <= 0) return;

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(hitObjectId, out var hitObject))
        {
            if (hitObject.TryGetComponent<IDamageble>(out var damageable))
            {
                damageable.TakeDamage(TankConfig.Damage);
            }
        }

        SpawnBulletVisual(origin, dir, length);

        SpawnBulletVisualClientRpc(origin, dir , length);

        currentAmmo--;      

        if (currentAmmo == 0)
        {
            StartCoroutine(ServerReloadCoroutine());
        }  
    }

    [ClientRpc]
    private void SpawnBulletVisualClientRpc(Vector2 origin, Vector2 dir, float distance)
    {
        if (IsOwner) return; // Owner already spawned the bullet visual in ShootClientAuthoritative

        SpawnBulletVisual(origin, dir, distance);
    }


    void SpawnBulletVisual(Vector2 pos, Vector2 dir, float distance)
    {
        if (TankConfig.BulletPrefab == null || bulletPool == null) return;
        
        Bullet bullet = bulletPool.bulletPool.Get(); 
        bullet.transform.position = pos;
        bullet.Initialize(dir, distance, bulletPool.bulletPool, TankConfig.ProjectileSpeed);
    }

    private void HandleReloadStateChanged(bool previousValue, bool newValue)
    {
        if (newValue == true && IsOwner)
        {
            reloadTimer = TankConfig.ReloadTime;
        }
    }

    void UpdateClientReloadUI()
    {
        if (isReloading.Value)
        {
            reloadTimer -= Time.deltaTime;
            reloadIndicator.fillAmount = 1f - (reloadTimer / TankConfig.ReloadTime);
        }
        else
        {
            reloadIndicator.fillAmount = 0f;
        }
    }

    private IEnumerator ServerReloadCoroutine()
    {
        isReloading.Value = true;
        yield return new WaitForSeconds(TankConfig.ReloadTime);
        currentAmmo = TankConfig.AmmoCapacity;  
        isReloading.Value = false;
    }

    [ServerRpc]
    public void StartReloadingServerRpc()
    {
        StartReloading();
    }
    

    public void StartReloading()
    {
        if (isReloading.Value || currentAmmo == TankConfig.AmmoCapacity) return;

        isReloading.Value = true;
        reloadTimer = TankConfig.ReloadTime;
    }
    public void ResetShootingState()
    {
        StopAllCoroutines();
        fireCooldown = 0f;
        hasFiredThisPress = false;
        
        Debug.Log("Shooting System Reset");
    }

    public override void OnNetworkDespawn()
    {
        isReloading.OnValueChanged -= HandleReloadStateChanged;
    }
}