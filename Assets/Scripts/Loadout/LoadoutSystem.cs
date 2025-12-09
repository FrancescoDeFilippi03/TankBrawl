using UnityEngine;
using System.IO;

[System.Serializable]
public struct PlayerLoadoutData
{
    public int BaseId;
    public int TurretId;
    public int WeaponId;
    public int BulletId;
}

public static class LoadoutSystem
{
    private static string Path => Application.persistentDataPath + "/user_loadout.json";

    public static void SaveLoadout(int baseId, int turretId, int weaponId, int bulletId)
    {
        PlayerLoadoutData data = new PlayerLoadoutData
        {
            BaseId = baseId,
            TurretId = turretId,
            WeaponId = weaponId,
            BulletId = bulletId
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path, json);
        Debug.Log("Loadout salvato: " + Path);
    }

    public static PlayerLoadoutData LoadLoadout()
    {
        if (!File.Exists(Path))
        {
            Debug.LogWarning("Nessun loadout trovato. Uso default.");
            return new PlayerLoadoutData(); // Torna tutto a 0
        }

        try
        {
            string json = File.ReadAllText(Path);
            return JsonUtility.FromJson<PlayerLoadoutData>(json);
        }
        catch
        {
            Debug.LogError("Errore lettura loadout. Uso default.");
            return new PlayerLoadoutData();
        }
    }
}