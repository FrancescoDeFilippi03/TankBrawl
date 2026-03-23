using UnityEngine;

[CreateAssetMenu(fileName = "TankConfig", menuName = "Tank/TankConfig")]
public class TankConfig : ScriptableObject
{
    
    //name
    [Header("General Info")]
    public string tankName;

    //health stats
    [Header("Health Stats")]
    public float maxHealth;
    public float maxShield;
    public float shieldRegenRate;
    [Tooltip("Seconds after last damage before shield starts regenerating")]
    public float shieldRegenDelay = 3f;


    //movement stats
    [Header("Movement Stats")]
    
   /*  public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;
    public float rotationSpeed; */
    public int weight;

    //weapon stats
    [Header("Weapon Stats")]
    public Weapon weaponData;
    

    public float damangeMultiplier = 1f;
    public float fireRateMultiplier = 1f;
    public float projectileSpeedMultiplier = 1f;
    public float rangeMultiplier = 1f;
    public float reloadTimeMultiplier = 1f;
    public float ammoCapacityMultiplier = 1f;

    //movement info 

    //weapon info scaled based on tank config
    public float Damage => weaponData.damage * damangeMultiplier;
    public float FireRate => weaponData.fireRate * fireRateMultiplier;
    public float ProjectileSpeed => weaponData.bulletSpeed * projectileSpeedMultiplier;
    public float Range => weaponData.range * rangeMultiplier;
    public float ReloadTime => weaponData.reloadTime * reloadTimeMultiplier;
    public int AmmoCapacity => Mathf.RoundToInt(weaponData.ammoCapacity * ammoCapacityMultiplier);

    //utils 
    public GameObject BulletPrefab => weaponData.bulletPrefab;
    public ShootingType ShootingType => weaponData.shootingType;
    public int BurstCount => weaponData.burstCount;
}
public enum ShootingType
{
    Single,
    Burst,
    Automatic
}