using Unity.Netcode;
using UnityEngine;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(ShootingSystem))]
public class TankPlayerData : NetworkBehaviour
{
    //Tank Elements
    private WeaponConfig tankWeapon;
    public WeaponConfig TankWeapon => tankWeapon;
    private GameObject weaponInstance;
    private BaseConfig  tankBase;
    public BaseConfig TankBase => tankBase;
    private GameObject baseInstance;


    Transform[] firePoints;
    public Transform[] FirePoints => firePoints;
    
    private ShootingSystem shootingSystem;
    public ShootingSystem ShootingSystem => shootingSystem;


    
    

    [Header("Tank Elements")]
    [SerializeField] private SpriteRenderer baseSpriteRenderer;
    [SerializeField] private SpriteLibrary trackSpriteLibraryLeft;
    [SerializeField] private SpriteLibrary trackSpriteLibraryRight;


    [Header("Tank Prefab Transforms")]
    [SerializeField] private Transform weaponPivotTransform;
    public Transform WeaponPivotTransform => weaponPivotTransform;

    private Transform crosshairTransform;
    public Transform CrosshairTransform => crosshairTransform;

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
        InstantiateTankBase(configData);
        InitStats();
        InitTags(configData);

        shootingSystem = GetComponent<ShootingSystem>();
        
        firePoints = weaponInstance.GetComponent<WeaponFirePoints>().firePoints;
        
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
    public void InstantiateTankBase(TankConfigData configData)
    {


        baseInstance = Instantiate(
            tankBase.baseVisualPrefab,
            this.transform
        );

        baseSpriteRenderer = baseInstance.GetComponent<SpriteRenderer>();
        trackSpriteLibraryLeft = baseInstance.transform.Find("Track_Left").GetComponent<SpriteLibrary>();
        trackSpriteLibraryRight = baseInstance.transform.Find("Track_Right").GetComponent<SpriteLibrary>();
        
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
