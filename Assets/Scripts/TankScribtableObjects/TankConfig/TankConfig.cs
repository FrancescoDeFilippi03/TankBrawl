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
    public WeaponData weaponData;

}