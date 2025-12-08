using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "TankElements/Bullet")]
public class Bullet : ScriptableObject
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
