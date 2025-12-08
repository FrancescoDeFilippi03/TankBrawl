using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "TankElements/Weapon")]
public class Weapon : ScriptableObject
{
    public float range;
    public float fireRate;
    public float ammo;
    public Sprite weaponSprite;
}
