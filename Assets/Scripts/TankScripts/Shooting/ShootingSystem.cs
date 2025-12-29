using Unity.Netcode;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class ShootingSystem : NetworkBehaviour
{
    public BulletPool bulletPool;
    private Transform[] firePoints;
    private bool isAlternateFirePoints = false;
    private Transform lastUsedFirePoint;

    // Fire rate control
    private float fireCooldown = 0f;
    private bool isFiring = false;
    
    // Burst control
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstDelay = 0.1f;

    private ShootingType shootingType;
    float fireRate;
    float range;
    float damage;
    GameObject bulletPrefab;

    

    public void InitializeWeapon(ShootingType shootingType, float fireRate,float range, float damage,GameObject bulletPrefab ,int ammoCapacity ,
    Transform[] firePoints = null , bool isAlternateFirePoints = false)
    {
        this.firePoints = firePoints;
        this.isAlternateFirePoints = isAlternateFirePoints;
        this.shootingType = shootingType;
        this.fireRate = fireRate;
        this.range = range;
        this.damage = damage;
        this.bulletPrefab = bulletPrefab;

        lastUsedFirePoint = firePoints != null && firePoints.Length > 0 ? firePoints[0] : null;
        
        bulletPool.InitializePool(bulletPrefab, ammoCapacity);

    }
    
    void Update()
    {
        if (!IsOwner) return;
        
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    public void TryShoot(Vector2 shootDirection, bool isHoldingTrigger)
    {
        if (!IsOwner) return;

        switch (shootingType)
        {
            case ShootingType.Single:
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
                if (isHoldingTrigger)
                {
                    if (CanFire())
                    {
                        Shoot(shootDirection);
                    }
                }
                break;
                
            case ShootingType.Burst:
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
    
    private bool CanFire()
    {
        return fireCooldown <= 0;
    }
    private IEnumerator BurstFire(Vector2 shootDirection)
    {
        for (int i = 0; i < burstCount; i++)
        {
            Shoot(shootDirection);
            
            if (i < burstCount - 1) 
            {
                yield return new WaitForSeconds(burstDelay);
            }
        }
        
        float fireInterval = 1f / fireRate;
        fireCooldown = fireInterval;
    }

    private void Shoot(Vector2 shootDirection)
    {
        if (firePoints == null || firePoints.Length == 0) return;

        float fireInterval = 1f / fireRate;
        fireCooldown = fireInterval;
        
        Vector2 dir = shootDirection.normalized;
                
        foreach (Transform firePoint in firePoints)
        {
            if (firePoint == null) continue;
            
            if (isAlternateFirePoints)
            {
                if (lastUsedFirePoint == firePoint)
                {
                    continue;
                }
                lastUsedFirePoint = firePoint;
            }

            SpawnFromPool(firePoint.position, dir, true);
            SpawnVisualsServerRpc(firePoint.position, dir);
        }
    }

    private void SpawnFromPool(Vector2 pos, Vector2 dir, bool isOwner)
    {
        Bullet bullet = bulletPool.bulletPool.Get(); 
        
        bullet.transform.position = pos;

        bullet.Initialize(dir, isOwner, this, bulletPool.bulletPool, OwnerClientId , damage , range);
    }

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

    public void ReportHit(ulong targetId)
    {
        if(!IsOwner) return;
        ApplyDamageServerRpc(targetId);
    }

    [ServerRpc]
    private void ApplyDamageServerRpc(ulong targetId)
    {
        Debug.Log($"Server applying damage to object ID {targetId}");

        
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetId, out var targetObject))
        {
            Debug.Log($"Applying damage to object {targetObject.name}");
            if (targetObject.TryGetComponent<IDamageble>(out var damageble))
            {
                damageble.TakeDamage(damage);
            }
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