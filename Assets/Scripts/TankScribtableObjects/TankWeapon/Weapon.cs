using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "TankElements/Weapon")]
public class Weapon : ScriptableObject
{
    public float range;
    public float fireRate;
    public int ammo;
    public Sprite weaponSpriteRed;
    public Sprite weaponSpriteBlue;
    public ShootingType shootingType;
    public GameObject bulletPrefab;
    public GameObject weaponVisualPrefab; 
}

public enum ShootingType
{
    Single,
    Burst,
    Automatic
} 