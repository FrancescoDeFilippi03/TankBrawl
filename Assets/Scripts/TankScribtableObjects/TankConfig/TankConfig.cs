using UnityEngine;

[CreateAssetMenu(fileName = "TankConfig", menuName = "TankElements/TankConfig")]
public class TankConfig : ScriptableObject
{
    
    //name
    public string tankName;

    //health stats
    public float maxHealth;
    public float maxShield;
    public float shieldRegenRate;


    //movement stats
    public float moveSpeed;
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;
    public float rotationSpeed;


    //weapon stats
    public float damage;
    public float fireRate;
    public float fireRange;
    public int ammoCapacity;
    public float reloadTime;
    public int burstCount;
    public Sprite crosshairSprite;
    public ShootingType shootingType;


    //bullet stats
    public float bulletSpeed;
    public float bulletDamage;
    public GameObject bulletPrefab;


    //prefabs
    public GameObject redTankPrefab;
    public GameObject blueTankPrefab;
    
}

public enum ShootingType
{
    Single,
    Automatic,
    Burst
}