using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class ShootingSystem : NetworkBehaviour
{
    public BulletPool bulletPool;
    private Transform[] firePoints;
    private WeaponConfig weapon;
    
    // Fire rate control
    private float fireCooldown = 0f;
    private bool isFiring = false;
    
    // Burst control
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstDelay = 0.1f;
    
    public void InitWeapon(WeaponConfig weapon, int ammoCount, Transform[] weaponFirePoints)
    {
        this.weapon = weapon;
        firePoints = weaponFirePoints;
        bulletPool.InitializePool(weapon.bulletConfig.bulletPrefab, ammoCount);
    }
    
    void Update()
    {
        if (!IsOwner) return;
        
        // Countdown fire cooldown
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Attempts to shoot based on the weapon's fire mode
    /// </summary>
    /// <param name="shootDirection">Direction to shoot</param>
    /// <param name="isHoldingTrigger">Is the player holding the fire button?</param>
    public void TryShoot(Vector2 shootDirection, bool isHoldingTrigger)
    {
        if (!IsOwner) return;
        if (weapon == null) 
        {
            Debug.LogWarning("Weapon not initialized!");
            return;
        }
        switch (weapon.shootingType)
        {
            case ShootingType.Single:
                // Single shot: fire once per button press, only when cooldown is ready
                if (isHoldingTrigger && !isFiring)
                {
                    if (CanFire())
                    {
                        Shoot(shootDirection);
                        isFiring = true;
                    }
                }
                else if (!isHoldingTrigger)
                {
                    isFiring = false;
                }
                break;
                
            case ShootingType.Automatic:
                // Automatic: fire continuously while holding, respecting cooldown
                if (isHoldingTrigger)
                {
                    if (CanFire())
                    {
                        Shoot(shootDirection);
                    }
                }
                break;
                
            case ShootingType.Burst:
                // Burst: fire burst on trigger pull, only when cooldown is ready
                if (isHoldingTrigger && !isFiring)
                {
                    if (CanFire())
                    {
                        StartCoroutine(BurstFire(shootDirection));
                        isFiring = true;
                    }
                }
                
                if (!isHoldingTrigger)
                {
                    isFiring = false;
                }
                break;
        }
    }
    
    /// <summary>
    /// Check if fire cooldown has reached 0
    /// </summary>
    private bool CanFire()
    {
        return fireCooldown <= 0;
    }
    
    /// <summary>
    /// Coroutine for burst fire mode
    /// </summary>
    private IEnumerator BurstFire(Vector2 shootDirection)
    {
        for (int i = 0; i < burstCount; i++)
        {
            Shoot(shootDirection);
            
            if (i < burstCount - 1) // Don't wait after the last shot
            {
                yield return new WaitForSeconds(burstDelay);
            }
        }
        
        // Set cooldown after burst completes
        float fireInterval = 1f / weapon.fireRate;
        fireCooldown = fireInterval;
    }

    private void Shoot(Vector2 shootDirection)
    {
        if (firePoints == null || firePoints.Length == 0) return;

        // Set fire cooldown
        float fireInterval = 1f / weapon.fireRate;
        fireCooldown = fireInterval;
        
        Vector2 dir = shootDirection.normalized;
        
        // Spara da tutti i punti di fuoco
        foreach (Transform firePoint in firePoints)
        {
            if (firePoint == null) continue;
            
            SpawnFromPool(firePoint.position, dir, true);
            SpawnVisualsServerRpc(firePoint.position, dir);
        }
    }

    // Metodo unico per estrarre dal pool
    private void SpawnFromPool(Vector2 pos, Vector2 dir, bool isOwner)
    {
        Bullet bullet = bulletPool.bulletPool.Get(); 
        
        bullet.transform.position = pos;

        bullet.Initialize(dir, isOwner, this, bulletPool.bulletPool, OwnerClientId , weapon.bulletConfig , weapon.range);
    }

    // --- RPCs per la visualizzazione sugli altri client ---

    [ServerRpc]
    private void SpawnVisualsServerRpc(Vector2 pos, Vector2 dir)
    {
        SpawnVisualsClientRpc(pos, dir);
    }

    [ClientRpc]
    private void SpawnVisualsClientRpc(Vector2 pos, Vector2 dir)
    {
        if (IsOwner) return;

        SpawnFromPool(pos, dir, false); 
    }

    // --- Gestione Danno ---
    public void ReportHit(ulong targetId)
    {
        if(!IsOwner) return;
        ApplyDamageServerRpc(targetId);
    }

    [ServerRpc]
    private void ApplyDamageServerRpc(ulong targetId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(targetId, out var client))
        {
            if (client.PlayerObject != null)
            {
                if (client.PlayerObject.TryGetComponent(out TankHealthManager healthManager))
                {
                    float damage = weapon.bulletConfig.damage;
                    if(healthManager.Invulnerable)
                    {
                        Debug.Log($"Target {targetId} is invulnerable. No damage applied.");
                        return;
                    }
                    healthManager.TakeDamage(damage);
                    
                    Debug.Log($"Applied {damage} damage to client {targetId}. Health: {healthManager.healthNetwork.Value}, Shield: {healthManager.shieldNetwork.Value}");
                }
                else
                {
                    Debug.LogWarning($"TankHealthManager not found on target {targetId}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Target client {targetId} not found");
        }
    }

    public void ResetShootingState()
    {
        StopAllCoroutines(); 

        isFiring = false;
        fireCooldown = 0f;
        
        Debug.Log("Shooting System Reset");
    }
}