using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using Unity.Netcode;

public class Bullet : MonoBehaviour 
{
    // Movement
    private Vector2 direction;
    private float speed;
    private float traveledDistance = 0f;
    private float maxRange = 0f;
    
    // Ownership & References
    private bool isOwnedByLocalPlayer;
    private ShootingSystem shootingSystem;
    private IObjectPool<Bullet> pool;
    private ulong ownerClientId;


    public void Initialize(Vector2 dir, bool isOwner, ShootingSystem sys, IObjectPool<Bullet> originPool, ulong ownerId, WeaponData weaponData)
    {
        direction = dir;
        isOwnedByLocalPlayer = isOwner;
        shootingSystem = sys;
        pool = originPool;
        ownerClientId = ownerId;
        maxRange = weaponData.range;
        speed = weaponData.bulletSpeed;
        traveledDistance = 0f;

        gameObject.name = isOwner ? $"Bullet_Owner_{ownerId}" : $"Bullet_Visual_{ownerId}";
    }

    void Update()
    {
        float distance = speed * Time.deltaTime;
        transform.Translate(distance * direction);
        
        traveledDistance += distance;
        if (traveledDistance >= maxRange)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit a networked object
        if (!other.TryGetComponent(out NetworkObject netObj))
        {
            return;
        }

        Debug.Log($"Bullet owned by {ownerClientId} hit {netObj.name} (Owner: {netObj.OwnerClientId})");
        
        // Don't hit yourself
        if (ownerClientId == netObj.OwnerClientId && other.GetComponent<TankPlayerController>() != null)
        {
            return;
        }

        // Don't hit teammates (same tag)
        if (other.gameObject.CompareTag(shootingSystem.gameObject.tag))
        {
            ReturnToPool();
            return;
        }

        // Only the bullet owner reports hits to the server
        if (isOwnedByLocalPlayer)
        {
            shootingSystem.ReportHit(netObj.NetworkObjectId);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        traveledDistance = 0f;
        pool?.Release(this);
    }
}