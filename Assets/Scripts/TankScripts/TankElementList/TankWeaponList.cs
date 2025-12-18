using System.Collections.Generic;
using UnityEngine;

public class TankWeaponList : MonoBehaviour
{
    public List<GameObject> weapons = new List<GameObject>();

    public List<WeaponConfig> weaponConfigs = new List<WeaponConfig>();
    public void SetWeaponActive(int weaponId)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].SetActive(i == weaponId);
        }
        
    }

    public GameObject GetActiveWeapon()
    {
        foreach (var weapon in weapons)
        {
            if (weapon.activeSelf)
                return weapon;
        }
        return null;
    }

    public WeaponConfig GetActiveWeaponConfig()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].activeSelf)
                return weaponConfigs[i];
        }
        return null;
    }

    public WeaponConfig GetWeaponConfigById(int weaponId)
    {
        if (weaponId >= 0 && weaponId < weaponConfigs.Count)
        {
            return weaponConfigs[weaponId];
        }
        return null;
    }

    public Transform[] GetActiveWeaponFirePoints()
    {
        GameObject activeWeapon = GetActiveWeapon();
        if (activeWeapon != null)
        {
            WeaponFirePoints firePointsComponent = activeWeapon.GetComponent<WeaponFirePoints>();
            if (firePointsComponent != null)
            {
                return firePointsComponent.firePoints;
            }
        }
        return null;
    }
}
