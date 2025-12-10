using UnityEngine;

public class TankPlayerData
{
    //Tank Elements
    [Header("Tank Elements")]
    public Bullet tankBullet;
    public Weapon tankWeapon;
    public Turret tankTurret;
    public Base  tankBase;
    
    [SerializeField] private SpriteRenderer baseSpriteRenderer;
    [SerializeField] private SpriteRenderer turretSpriteRenderer;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    

    private float speed;
    public float Speed => speed;

    private float armor;
    public float Armor => armor;

    private float health;
    public float Health => health;

    public void Init(TankConfigData configData)
    {
        InitTankElements(configData);
        InitTankSprites(configData);
        InitStats();
    }

    public void InitTankElements(TankConfigData configData)
    {
        tankBase   = TankRegistry.Instance.GetBase(configData.BaseId);
        tankTurret = TankRegistry.Instance.GetTurret(configData.TurretId);
        tankWeapon = TankRegistry.Instance.GetWeapon(configData.WeaponId);
        tankBullet = TankRegistry.Instance.GetBullet(configData.BulletId);
    }
    public void InitTankSprites(TankConfigData configData)
    {
        if (configData.Team == TeamColor.Red)
        {
            baseSpriteRenderer.sprite = tankBase.baseSpriteRed;
            turretSpriteRenderer.sprite = tankTurret.turretSpriteRed;
            
        }
        else
        {
            baseSpriteRenderer.sprite = tankBase.baseSpriteBlue;
            turretSpriteRenderer.sprite = tankTurret.turretSpriteBlue;
        }
        weaponSpriteRenderer.sprite = tankWeapon.weaponSprite;
    }

    public void InitStats()
    {
        speed = tankBase.speed;
        armor = tankBase.armor;
        health = tankBase.health;
    }
}
