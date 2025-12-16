using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using Unity.Netcode;

public class Bullet : MonoBehaviour 
{
    private Vector2 direction;
    private bool amIOwner;     
    private ShootingSystem system; 
    private IObjectPool<Bullet> pool;
    ulong OwnerClientId;
    private BulletConfig bulletConfig;

    private float traveledDistance = 0f;
    private float maxRange = 0f;


    public void Initialize(Vector2 dir, bool isOwner, ShootingSystem sys, IObjectPool<Bullet> originPool, ulong ownerId, BulletConfig config , float range)
    {
        direction = dir;
        amIOwner = isOwner;
        system = sys;
        pool = originPool;
        OwnerClientId = ownerId;
        bulletConfig = config;
        maxRange = range;

        this.gameObject.name = $"BulletOwner_{ownerId}";
    }

    void Update()
    {
        transform.Translate(bulletConfig.speed * Time.deltaTime * direction);
        traveledDistance += bulletConfig.speed * Time.deltaTime;
        if (traveledDistance >= maxRange)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        /* if (other.CompareTag("Wall"))
        {
            ReturnToPool();
            return;
        } */

        //itero sui network object per vedere se ho colpito un tank
        if (other.TryGetComponent(out NetworkObject netObj))
        {
            //ignoro se colpisco me stesso
            if (OwnerClientId == netObj.OwnerClientId)
            {
                return;
            }

            //ignoro se colpisco un mio compagno di squadra
            if (other.gameObject.CompareTag(system.gameObject.tag))
            {
                ReturnToPool();
                return;
            }

            //se sono il proprietario del proiettile, segnalo il colpo
            if (amIOwner)
            {
                system.ReportHit(netObj.OwnerClientId);
            }


            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        //StopAllCoroutines(); 
        traveledDistance = 0f;
        
        pool?.Release(this);
    }
}