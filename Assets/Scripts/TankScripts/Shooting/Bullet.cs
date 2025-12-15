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


    public void Initialize(Vector2 dir, bool isOwner, ShootingSystem sys, IObjectPool<Bullet> originPool, ulong ownerId, BulletConfig config)
    {
        direction = dir;
        amIOwner = isOwner;
        system = sys;
        pool = originPool;
        OwnerClientId = ownerId;
        bulletConfig = config;

        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle,direction);

        this.gameObject.name = $"BulletOwner_{ownerId}";


        StartCoroutine(DeactivateRoutine(3.0f));
    }

    void Update()
    {
        transform.Translate(bulletConfig.speed * Time.deltaTime * direction);
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
    private IEnumerator DeactivateRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        StopAllCoroutines(); 
        
        pool?.Release(this);
    }
}