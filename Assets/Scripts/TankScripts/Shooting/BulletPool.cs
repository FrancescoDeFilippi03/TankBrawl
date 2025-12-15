using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    GameObject currentBulletPrefab;
    public IObjectPool<Bullet> bulletPool; 
    private Transform containerTransform;
    public void InitializePool(GameObject prefab, int defaultCapacity = 20)
    {
        currentBulletPrefab = prefab;

        GameObject container = new GameObject($"Pool_{this.name}");
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
        GameObject bulletObj = Instantiate(currentBulletPrefab,containerTransform);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bulletObj.SetActive(false);
        return bullet;
    }

    // Quando prendiamo dal pool (Get)
    private void OnGetBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    // Quando restituiamo al pool (Release)
    private void OnReleaseBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }
    private void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    private void OnDestroy()
    {
        if (containerTransform != null)
        {
            Destroy(containerTransform.gameObject);
        }
    }
}