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
    private IObjectPool<Bullet> pool;
    [SerializeField] private TrailRenderer trail;
    public void Initialize(Vector2 dir, float distance , IObjectPool<Bullet> originPool,float bulletSpeed)
    {
        direction = dir;
        pool = originPool;
        maxRange = distance;
        speed = bulletSpeed;
        traveledDistance = 0f;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
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