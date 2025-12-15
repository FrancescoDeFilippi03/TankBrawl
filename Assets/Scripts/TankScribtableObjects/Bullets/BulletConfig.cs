using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "TankElements/Bullet")]
public class BulletConfig : ScriptableObject
{
    //statistics
    public string bulletName;
    public float speed;
    public float damage;


    public GameObject bulletPrefab;
}
