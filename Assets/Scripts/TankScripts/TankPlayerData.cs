using UnityEngine;

public class TankPlayerData : MonoBehaviour
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
    

    public void InitializeTankElements(TankConfigData configData)
    {
        tankBase   = TankRegistry.Instance.GetBase(configData.BaseId);
        tankTurret = TankRegistry.Instance.GetTurret(configData.TurretId);
        tankWeapon = TankRegistry.Instance.GetWeapon(configData.WeaponId);
        tankBullet = TankRegistry.Instance.GetBullet(configData.BulletId);
    }
    public void UpdateTankSprites(TankConfigData configData)
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
}
