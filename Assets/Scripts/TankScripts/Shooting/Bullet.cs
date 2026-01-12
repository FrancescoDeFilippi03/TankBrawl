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
    [SerializeField] private TrailRenderer trail;

    public void Initialize(Vector2 dir, bool isOwner, ShootingSystem sys, 
                            IObjectPool<Bullet> originPool, ulong ownerId, 
                            WeaponData weaponData)
    {
        direction = dir;
        isOwnedByLocalPlayer = isOwner;
        shootingSystem = sys;
        pool = originPool;
        ownerClientId = ownerId;
        maxRange = weaponData.range;
        speed = weaponData.bulletSpeed;
        traveledDistance = 0f;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        gameObject.name = isOwner ? $"Bullet_Owner_{ownerId}" : $"Bullet_Visual_{ownerId}";
    }

    void Update()
    {
        float distance = speed * Time.deltaTime;
        transform.position += (Vector3)(distance * direction);
        
        traveledDistance += distance;
        if (traveledDistance >= maxRange)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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

    protected virtual void ReturnToPool()
    {
        traveledDistance = 0f;
        
        if (trail != null)
        {
            trail.Clear();
        }
        
        pool?.Release(this);
    }

    
}