using UnityEngine;

public class TankPlayerData : MonoBehaviour
{
    //Tank Elements
    private WeaponConfig tankWeapon;
    public WeaponConfig TankWeapon => tankWeapon;
    private BaseConfig  tankBase;
    public BaseConfig TankBase => tankBase;

    public void InitTankElements(TankConfigData configData)
    {
        tankBase   = TankRegistry.Instance.GetBase(configData.BaseId);
        tankWeapon = TankRegistry.Instance.GetWeapon(configData.WeaponId);

        InitTags(configData);
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
