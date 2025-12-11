using UnityEngine;
using UnityEngine.U2D.Animation;
using Unity.Cinemachine;
public class TankPlayerData : MonoBehaviour
{
    //Tank Elements
    [Header("Tank Elements")]
    private Bullet tankBullet;
    private Weapon tankWeapon;
    private Turret tankTurret;
    private Base  tankBase;
    
    [SerializeField] private SpriteRenderer baseSpriteRenderer;
    [SerializeField] private SpriteRenderer turretSpriteRenderer;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    [SerializeField] private SpriteLibrary baseSpriteLibraryLeft;
    [SerializeField] private SpriteLibrary baseSpriteLibraryRight;

/*     [SerializeField] private CinemachineCamera tankCamera;
    public CinemachineCamera TankCamera => tankCamera; */

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
            
            //baseSpriteRenderer.color = Color.red;
            
        }
        else
        {
            baseSpriteRenderer.sprite = tankBase.baseSpriteBlue;
            turretSpriteRenderer.sprite = tankTurret.turretSpriteBlue;

            //baseSpriteRenderer.color = Color.blue;
        }
        weaponSpriteRenderer.sprite = tankWeapon.weaponSprite;
        
        baseSpriteLibraryLeft.spriteLibraryAsset = tankBase.trackSpriteLibraryAsset;
        baseSpriteLibraryRight.spriteLibraryAsset = tankBase.trackSpriteLibraryAsset;
    }

    public void InitStats()
    {
        speed = tankBase.speed;
        armor = tankBase.armor;
        health = tankBase.health;
    }

}
