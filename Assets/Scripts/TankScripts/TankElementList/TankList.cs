using UnityEngine;

public class TankList : MonoBehaviour
{
    [SerializeField] private TankConfig[] tankConfigs;

    public TankConfig GetTankConfigById(int id)
    {
        if (id < 0 || id >= tankConfigs.Length)
        {
            Debug.LogError("Invalid TankConfig ID: " + id);
            return null;
        }
        return tankConfigs[id];
    }
}
