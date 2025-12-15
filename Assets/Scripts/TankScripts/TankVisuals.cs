using UnityEngine;
using UnityEngine.U2D.Animation;

public class TankVisuals : MonoBehaviour
{
    [Header("Tank Elements")]
    private SpriteRenderer baseSpriteRenderer;
    private SpriteLibrary trackSpriteLibraryLeft;
    private SpriteLibrary trackSpriteLibraryRight;

    private GameObject weaponInstance;
    public GameObject WeaponInstance => weaponInstance;
    
    private GameObject baseInstance;

    [SerializeField] private GameObject weaponPivot;
    [SerializeField] private TankPlayerData tankPlayerData;

    public void InitializeVisuals(TankConfigData configData)
    {
        InstantiateTankBase(configData);
        InstantiateWeaponPrefab(configData);
    }

    public void InstantiateWeaponPrefab(TankConfigData configData)
    {
        if (tankPlayerData.TankWeapon.weaponVisualPrefab == null) return;
        
        weaponInstance = Instantiate(
            tankPlayerData.TankWeapon.weaponVisualPrefab,
            weaponPivot.transform
        );

        SpriteRenderer weaponSprite = weaponInstance.GetComponentInChildren<SpriteRenderer>();
        if (weaponSprite != null)
        {
            weaponSprite.sprite = configData.Team == TeamColor.Red 
                ? tankPlayerData.TankWeapon.weaponSpriteRed 
                : tankPlayerData.TankWeapon.weaponSpriteBlue;
        }
    }

    public void InstantiateTankBase(TankConfigData configData)
    {


        baseInstance = Instantiate(
            tankPlayerData.TankBase.baseVisualPrefab,
            this.transform
        );

        baseSpriteRenderer = baseInstance.GetComponent<SpriteRenderer>();
        trackSpriteLibraryLeft = baseInstance.transform.Find("Track_Left").GetComponent<SpriteLibrary>();
        trackSpriteLibraryRight = baseInstance.transform.Find("Track_Right").GetComponent<SpriteLibrary>();
        
        if (configData.Team == TeamColor.Red)
        {
            baseSpriteRenderer.sprite = tankPlayerData.TankBase.baseSpriteRed;
        }
        else
        {
            baseSpriteRenderer.sprite = tankPlayerData.TankBase.baseSpriteBlue;
        }
       
        trackSpriteLibraryLeft.spriteLibraryAsset = tankPlayerData.TankBase.trackSpriteLibraryAsset;
        trackSpriteLibraryRight.spriteLibraryAsset = tankPlayerData.TankBase.trackSpriteLibraryAsset;
    }
}
