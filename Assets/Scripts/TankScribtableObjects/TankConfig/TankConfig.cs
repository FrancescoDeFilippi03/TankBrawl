using UnityEngine;

[CreateAssetMenu(fileName = "TankConfig", menuName = "TankElements/TankConfig")]
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


    //movement stats
    [Header("Movement Stats")]
    public float moveSpeed;
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;
    public float rotationSpeed;


    //weapon stats
    [Header("Weapon Stats")]
    public WeaponData weaponData;

}

public enum ShootingType
{
    Single,
    Burst,
    Automatic
}
[System.Serializable]
    public struct WeaponData
    {
        [Header("Shooting Mechanics")]
        public ShootingType shootingType;
        [Tooltip("Shots per second")]
        public float fireRate;
        public float range;
        public float damage;
        public float bulletSpeed;

        [Header("Burst Fire")]
        [Tooltip("Number of shots in a single burst")]
        public int burstCount;
        [Tooltip("Delay between shots in a burst")]
        public float burstDelay;

        [Header("Resources")]
        public GameObject bulletPrefab;
        public int ammoCapacity;
        public float reloadTime;
        public Sprite crosshairSprite;

        [Header("Alternate Fire Points")]
        public bool isAlternateFirePoints;
    }