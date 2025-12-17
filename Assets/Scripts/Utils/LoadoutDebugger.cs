using UnityEngine;

// Questo script serve come interfaccia per l'editor, 
// dato che LoadoutSystem è statico.
public class LoadoutDebugger : MonoBehaviour
{
    [Header("Current Configuration")]
    public int baseId;
    public int turretId;
    public int weaponId;
    public int bulletId;

    public void SaveToDisk()
    {
        LoadoutSystem.SaveLoadout(baseId, weaponId);
    }

    public void LoadFromDisk()
    {
        var data = LoadoutSystem.LoadLoadout();
        baseId = data.BaseId;
        weaponId = data.WeaponId;
    }
}