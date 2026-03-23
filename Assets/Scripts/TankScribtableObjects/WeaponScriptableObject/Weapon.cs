using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Tank/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("General Info")]
    public string weaponName;
    [Header("Shooting Mechanics")]
    public ShootingType shootingType;
    public float fireRate;
    public float range;
    public float weaponDamageMultiplier;
    public float bulletSpeedMultiplier;
    public int ammoCapacity;
    public float reloadTime;

    [Header("Burst Fire")]
    public int burstCount;
    public float burstDelay;

    public BulletConfig bulletConfig;
    public GameObject bulletPrefab => bulletConfig.bulletPrefab;
    public float damage => bulletConfig.damage * weaponDamageMultiplier;
    public float bulletSpeed => bulletConfig.speed * bulletSpeedMultiplier;
    public Texture2D crosshairTexture;

}
