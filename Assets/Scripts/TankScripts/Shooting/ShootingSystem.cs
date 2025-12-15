using Unity.Netcode;
using UnityEngine;

public class ShootingSystem : NetworkBehaviour
{
    public BulletPool bulletPool;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    public void InitWeapon(GameObject bulletPrefab, int ammoCount)
    {
        bulletPool.InitializePool(bulletPrefab, ammoCount);
    }

    public void Shoot(Vector2 shootDirection)
    {
        if (!IsOwner) return;

        Vector2 dir = shootDirection.normalized;
        SpawnFromPool(firePoint.position, dir, true);

        SpawnVisualsServerRpc(firePoint.position, dir);
        
    }

    // Metodo unico per estrarre dal pool
    private void SpawnFromPool(Vector2 pos, Vector2 dir, bool isOwner)
    {
        Bullet bullet = bulletPool.bulletPool.Get(); 
        
        bullet.transform.position = pos;

        Color myColor = Color.white;
        if (gameObject.CompareTag("Red")) 
        {
            myColor = Color.red;
        }
        else if (gameObject.CompareTag("Blue")) 
        {
            myColor = Color.blue;
        }
        else 
        {
            // DEBUG: Se vedi questo log, hai dimenticato di settare i Tag in TankPlayerData!
            Debug.LogWarning($"Tank {OwnerClientId} ha tag sconosciuto: {gameObject.tag}. Uso Blu di default.");
            myColor = Color.blue; 
        }

        bullet.Initialize(dir, bulletSpeed, isOwner, this, bulletPool.bulletPool, OwnerClientId , myColor);
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