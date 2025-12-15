using Unity.Netcode;
using UnityEngine;

public class ShootingSystem : NetworkBehaviour
{
    public BulletPool bulletPool;
    private Transform[] firePoints;
    private WeaponConfig weapon;
    public void InitWeapon(WeaponConfig weapon, int ammoCount, Transform[] weaponFirePoints)
    {
        this.weapon = weapon;
        firePoints = weaponFirePoints;
        bulletPool.InitializePool(weapon.bulletConfig.bulletPrefab, ammoCount);
    }

    public void Shoot(Vector2 shootDirection)
    {
        if (!IsOwner) return;
        
        if (firePoints == null || firePoints.Length == 0) return;

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

        bullet.Initialize(dir, isOwner, this, bulletPool.bulletPool, OwnerClientId , weapon.bulletConfig);
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
        Debug.Log($"Colpito {targetId}");
    }
}