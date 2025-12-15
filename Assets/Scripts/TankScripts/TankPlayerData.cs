using Unity.Netcode;
using UnityEngine;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(ShootingSystem))]
public class TankPlayerData : NetworkBehaviour
{
    //Tank Elements
    
    //private BulletConfig tankBullet;
    private Weapon tankWeapon;
    private Base  tankBase;
    private ShootingSystem shootingSystem;
    private GameObject weaponInstance;
    public ShootingSystem ShootingSystem => shootingSystem;

    [Header("Tank Elements")]
    [SerializeField] private SpriteRenderer baseSpriteRenderer;
    [SerializeField] private SpriteLibrary trackSpriteLibraryLeft;
    [SerializeField] private SpriteLibrary trackSpriteLibraryRight;


    [Header("Tank Prefab Transforms")]
    [SerializeField] private Transform weaponPivotTransform;
    public Transform WeaponPivotTransform => weaponPivotTransform;

    [Header("Tank Stats")]
    private float speed;
    public float Speed => speed;

    private float armor;
    public float Armor => armor;

    private float health;
    public float Health => health;

    public void Init(TankConfigData configData)
    {
        InitTankElements(configData);
        InstatiateWeaponPrefab(configData);
        InitTankBaseSprites(configData);
        InitStats();
        InitTags(configData);

        shootingSystem = GetComponent<ShootingSystem>();
        
        Transform[] firePoints = weaponInstance.GetComponent<WeaponFirePoints>().firePoints;
        
        shootingSystem.InitWeapon(
            tankWeapon,
            tankWeapon.ammo,
            firePoints
        );
    }

    void InstatiateWeaponPrefab(TankConfigData configData)
    {
        if (tankWeapon.weaponVisualPrefab == null) return;
        
        weaponInstance = Instantiate(
            tankWeapon.weaponVisualPrefab,
            weaponPivotTransform.position,
            Quaternion.identity,
            weaponPivotTransform
        );

        SpriteRenderer weaponSprite = weaponInstance.GetComponentInChildren<SpriteRenderer>();
        if (weaponSprite != null)
        {
            weaponSprite.sprite = configData.Team == TeamColor.Red 
                ? tankWeapon.weaponSpriteRed 
                : tankWeapon.weaponSpriteBlue;
        }
    }

    public void InitTankElements(TankConfigData configData)
    {
        tankBase   = TankRegistry.Instance.GetBase(configData.BaseId);
        tankWeapon = TankRegistry.Instance.GetWeapon(configData.WeaponId);
    }
    public void InitTankBaseSprites(TankConfigData configData)
    {
        if (configData.Team == TeamColor.Red)
        {
            baseSpriteRenderer.sprite = tankBase.baseSpriteRed;
        }
        else
        {
            baseSpriteRenderer.sprite = tankBase.baseSpriteBlue;
        }
       
        trackSpriteLibraryLeft.spriteLibraryAsset = tankBase.trackSpriteLibraryAsset;
        trackSpriteLibraryRight.spriteLibraryAsset = tankBase.trackSpriteLibraryAsset;
    }

    public void InitStats()
    {
        speed = tankBase.speed;
        armor = tankBase.armor;
        health = tankBase.health;
    }

    void InitTags(TankConfigData configData)
    {
         if (configData.Team == TeamColor.Red)
        {
            gameObject.tag = "Red";
        }
        else
        {
            gameObject.tag = "Blue";
        }
    }


}
