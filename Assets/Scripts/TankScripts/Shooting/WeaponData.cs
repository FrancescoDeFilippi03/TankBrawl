using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "TankBrawl/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Shooting Mechanics")]
    public ShootingType shootingType = ShootingType.Single;
    [Tooltip("Shots per second")]
    public float fireRate = 2f;
    public float range = 50f;
    public float damage = 10f;
    public float bulletSpeed = 20f;

    [Header("Burst Fire")]
    [Tooltip("Number of shots in a single burst")]
    public int burstCount = 3;
    [Tooltip("Delay between shots in a burst")]
    public float burstDelay = 0.1f;

    [Header("Resources")]
    public GameObject bulletPrefab;
    public int ammoCapacity = 20;
    public Sprite crosshairSprite;

    [Header("Alternate Fire Points")]
    public bool isAlternateFirePoints = false;
}

public enum ShootingType
{
    Single,
    Automatic,
    Burst
}
