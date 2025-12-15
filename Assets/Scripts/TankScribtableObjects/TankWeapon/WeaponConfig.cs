using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "TankElements/Weapon")]
public class WeaponConfig : ScriptableObject
{
    //statistics
    public string weaponName;
    public float range;
    public float fireRate;
    public int ammo;

    public Sprite weaponSpriteRed;
    public Sprite weaponSpriteBlue;
    public Sprite crosshairSprite;
    public ShootingType shootingType;
    public BulletConfig bulletConfig;
    public GameObject weaponVisualPrefab; 
}

public enum ShootingType
{
    Single,
    Burst,
    Automatic
} 