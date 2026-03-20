using Unity.Netcode;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ShootingSystem : NetworkBehaviour
{
    public event Action<int, int> OnAmmoChanged; // current, max
    
    public BulletPool bulletPool;
    public Transform[] firePoints;
    [SerializeField] private Image reloadIndicator;
    //reloading
    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer = 0f;

    // Fire rate control
    private float fireCooldown = 0f;
    private WeaponData currentWeapon;
    private int currentFirePointIndex = 0;
    private bool hasFiredThisPress = false;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => currentWeapon.ammoCapacity;
    public WeaponData CurrentWeapon => currentWeapon;

    private TeamColor teamColor;

    public void InitializeWeapon(WeaponData weaponData , TeamColor teamColor)
    {
        currentWeapon = weaponData;
        currentFirePointIndex = 0;
        this.teamColor = teamColor;


        currentAmmo = currentWeapon.ammoCapacity;
        isReloading = false;
        reloadTimer = 0f;
        
        if (currentWeapon.bulletPrefab != null)
        {
            bulletPool.InitializePool(currentWeapon.bulletPrefab, currentWeapon.ammoCapacity , teamColor);
        }

        reloadIndicator.fillAmount = 0f;
    }
    
    void Update()
    {
        if (!IsOwner) return;
        
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

        UpdateReloading();
    }

    public void TryShoot(Vector2 shootDirection, bool isHoldingTrigger)
    {
        if (!IsOwner || currentWeapon.bulletPrefab == null) return;

        if (!isHoldingTrigger)
        {
            hasFiredThisPress = false;
            return;
        }

        if (!CanFire()) return;

        switch (currentWeapon.shootingType)
        {
            case ShootingType.Single:
                if (!hasFiredThisPress)
                {
                    Shoot(shootDirection);
                    fireCooldown = 1f / currentWeapon.fireRate;
                    hasFiredThisPress = true;
                }
                break;
            
            case ShootingType.Automatic:
                Shoot(shootDirection);
                fireCooldown = 1f / currentWeapon.fireRate;
                break;
                
            case ShootingType.Burst:
                StartCoroutine(BurstFire(shootDirection));
                fireCooldown = 1f / currentWeapon.fireRate;
                break;
        }
    }
    
    private bool CanFire()
    {
        return fireCooldown <= 0;
    }
    private IEnumerator BurstFire(Vector2 shootDirection)
    {
        for (int i = 0; i < currentWeapon.burstCount; i++)
        {
            Shoot(shootDirection);
            
            if (i < currentWeapon.burstCount - 1) 
            {
                yield return new WaitForSeconds(currentWeapon.burstDelay);
            }
        }
    }

    /* private void Shoot(Vector2 shootDirection)
    {
        if (firePoints == null || firePoints.Length == 0 || currentWeapon.bulletPrefab == null) return;

        if (currentAmmo <= 0)
        {
            StartReloading();
            return;
        }

        Vector2 dir = shootDirection.normalized;

        if (currentWeapon.isAlternateFirePoints)
        {
            Transform firePoint = firePoints[currentFirePointIndex];
            if (firePoint != null)
            {
                SpawnBulletLocally(firePoint.position, dir);
                SpawnVisualsServerRpc(firePoint.position, dir);
            }
            
            currentFirePointIndex = (currentFirePointIndex + 1) % firePoints.Length;
        }
        else
        {
            foreach (Transform firePoint in firePoints)
            {
                if (firePoint == null) continue;
                
                SpawnBulletLocally(firePoint.position, dir);
                SpawnVisualsServerRpc(firePoint.position, dir);
            }
        }
        
        currentAmmo--;
        OnAmmoChanged?.Invoke(currentAmmo, currentWeapon.ammoCapacity);
    } */

    void Shoot(Vector2 shootDirection)
    {
        int bulletLayer = (teamColor == TeamColor.Red) ? LayerMask.NameToLayer("BulletRed") : LayerMask.NameToLayer("BulletBlue");
        Debug.Log("Team color: " + teamColor + " , bullet layer: " + LayerMask.LayerToName(bulletLayer));
        
        int mask = Physics2D.GetLayerCollisionMask(bulletLayer);

        Debug.Log($"Shooting with layer {LayerMask.LayerToName(bulletLayer)} and mask {mask}");

        Vector2 origin = firePoints[0].position;
        Vector2 dir = shootDirection.normalized;
        ShootServerRpc(origin, dir, mask , currentWeapon.range);
    }

    [ServerRpc]
    private void ShootServerRpc(Vector2 origin, Vector2 dir, int mask = 0 , float range = 0f)
    {

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, range, mask);

        if(hit.collider != null)
        {
            if (hit.collider.TryGetComponent<IDamageble>(out var damageble))
            {
                damageble.TakeDamage(currentWeapon.damage);
                Debug.Log($"Raycast hit {hit.collider.gameObject.name} and applied {currentWeapon.damage} damage");
            }
        }
        float length = hit.collider != null ? hit.distance : range;

        SpawnBulletVisualClientRpc(origin, dir , length);
        currentAmmo--;
        OnAmmoChanged?.Invoke(currentAmmo, currentWeapon.ammoCapacity);
        
        /* 
        Debug.Log($"Raycast hit {hit.collider?.gameObject.name ?? "nothing"} at distance: {hit.distance} , {currentWeapon.range}");
        Debug.DrawRay(origin, dir * length, Color.red, 1f); */
    }

    [ClientRpc]
    void SpawnBulletVisualClientRpc(Vector2 pos, Vector2 dir, float distance)
    {
        if (currentWeapon.bulletPrefab == null || bulletPool == null) return;
        
        Bullet bullet = bulletPool.bulletPool.Get(); 
        bullet.transform.position = pos;
        bullet.Initialize(dir, distance, bulletPool.bulletPool, currentWeapon);
    }

    
    void UpdateReloading()
    {
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            reloadIndicator.fillAmount = 1f - (reloadTimer / currentWeapon.reloadTime);
            if (reloadTimer <= 0f)
            {
                currentAmmo = currentWeapon.ammoCapacity;
                isReloading = false;
                reloadIndicator.fillAmount = 0f;
                OnAmmoChanged?.Invoke(currentAmmo, currentWeapon.ammoCapacity);
            }
        }
    }

    public void StartReloading()
    {
        if (isReloading || currentAmmo == currentWeapon.ammoCapacity) return;

        isReloading = true;
        reloadTimer = currentWeapon.reloadTime;
    }

/*     //multiplayer bullet spawning
    private void SpawnBulletLocally(Vector2 pos, Vector2 dir)
    {
        if (currentWeapon.bulletPrefab == null || bulletPool == null) return;
        
        Bullet bullet = bulletPool.bulletPool.Get(); 
        bullet.transform.position = pos;
        bullet.Initialize(dir, true, this, bulletPool.bulletPool, OwnerClientId, currentWeapon);
    }

    private void SpawnBulletVisual(Vector2 pos, Vector2 dir)
    {
        if (currentWeapon.bulletPrefab == null || bulletPool == null) return;
        
        Bullet bullet = bulletPool.bulletPool.Get(); 
        bullet.transform.position = pos;
        bullet.Initialize(dir, false, this, bulletPool.bulletPool, OwnerClientId, currentWeapon);
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
        SpawnBulletVisual(pos, dir);
    }

 */  
 /*    public void ReportHit(ulong targetId)
    {
        if(!IsOwner) return;
        ApplyDamageServerRpc(targetId);
    }

    [ServerRpc]
    private void ApplyDamageServerRpc(ulong targetId)
    {
        if (currentWeapon.bulletPrefab == null) return;

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetId, out var targetObject))
        {
            if (targetObject.TryGetComponent<IDamageble>(out var damageble))
            {
                if(targetObject.TryGetComponent<Tank>(out var tank))
                {
                    tank.RecordDamageFrom(OwnerClientId);
                }

                damageble.TakeDamage(currentWeapon.damage);
                Debug.Log($"[Server] Applied {currentWeapon.damage} damage to {targetObject.name}");

                
            }
        }
    }
 */
    public void ResetShootingState()
    {
        StopAllCoroutines();
        fireCooldown = 0f;
        currentFirePointIndex = 0;
        hasFiredThisPress = false;
        
        Debug.Log("Shooting System Reset");
    }
}