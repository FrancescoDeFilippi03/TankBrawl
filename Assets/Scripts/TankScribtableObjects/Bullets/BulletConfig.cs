using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "TankElements/Bullet")]
public class BulletConfig : ScriptableObject
{
    public float speed;
    public float damage;
    public Sprite bulletSprite;

    public enum ShootingType
    {
        Single,
        Burst,
        Automatic
    }
}
