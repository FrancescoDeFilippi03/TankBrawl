using UnityEngine;

public class TankRegistry : MonoBehaviour 
{
    public static TankRegistry Instance { get; private set; }
    [SerializeField] private TankConfig[] tankConfigs;
    private void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return;
        }
        Instance = this;
    }

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