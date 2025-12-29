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
    private float traveledDistance = 0f;
    private float maxRange = 0f;

    float speed;


    public void Initialize(Vector2 dir, bool isOwner, ShootingSystem sys, IObjectPool<Bullet> originPool, ulong ownerId,float range , float speed)
    {
        direction = dir;
        amIOwner = isOwner;
        system = sys;
        pool = originPool;
        OwnerClientId = ownerId;
        maxRange = range;
        this.speed = speed;

        this.gameObject.name = $"BulletOwner_{ownerId}";
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * direction);
        traveledDistance += speed * Time.deltaTime;
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
            Debug.Log($"Bullet owned by {OwnerClientId} hit object owned by {netObj.name} with owner {netObj.OwnerClientId}");
           //ignoro se colpisco me stesso
            if (OwnerClientId == netObj.OwnerClientId && other.GetComponent<TankPlayerController>() != null)
            {
                return;
            }

            //ignoro se colpisco un mio compagno di squadra o me stesso
            if (other.gameObject.CompareTag(system.gameObject.tag))
            {
                ReturnToPool();
                return;
            }

            //se sono il proprietario del proiettile, segnalo il colpo
            if (amIOwner)
            {
                system.ReportHit(netObj.NetworkObjectId);
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