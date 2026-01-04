using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    private GameObject currentBulletPrefab;
    public IObjectPool<Bullet> bulletPool;
    private Transform containerTransform;
    
    public void InitializePool(GameObject prefab, int defaultCapacity = 20)
    {
        if (prefab == null)
        {
            Debug.LogError("BulletPool: Cannot initialize with null prefab!");
            return;
        }
        
        currentBulletPrefab = prefab;

        GameObject container = new($"Pool_{this.name}");
        containerTransform = container.transform;

        bulletPool = new ObjectPool<Bullet>(
            CreateBullet, 
            OnGetBullet, 
            OnReleaseBullet, 
            OnDestroyBullet, 
            true, 
            defaultCapacity, 
            100 
        );
    }
    private Bullet CreateBullet()
    {
        GameObject bulletObj = Instantiate(currentBulletPrefab, containerTransform);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        
        if (bullet == null)
        {
            Debug.LogError($"BulletPool: Prefab {currentBulletPrefab.name} doesn't have a Bullet component!");
            Destroy(bulletObj);
            return null;
        }
        
        bulletObj.SetActive(false);
        return bullet;
    }

    private void OnGetBullet(Bullet bullet)
    {
        if (bullet != null)
        {
            bullet.gameObject.SetActive(true);
        }
    }

    private void OnReleaseBullet(Bullet bullet)
    {
        if (bullet != null)
        {
            bullet.gameObject.SetActive(false);
        }
    }
    
    private void OnDestroyBullet(Bullet bullet)
    {
        if (bullet != null)
        {
            Destroy(bullet.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (containerTransform != null)
        {
            Destroy(containerTransform.gameObject);
        }
    }
}