using UnityEngine;
using Unity.Netcode;
public enum TeamColor
{
    Red,
    Blue,
    None
}

public class TeamManager : NetworkBehaviour
{
    public static TeamManager Instance;

    public NetworkList<TankConfigData> tankConfigs = new();

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
    
    private void OnClientDisconnected(ulong clientId)
    {
        foreach (var config in tankConfigs)
        {
            if (config.ClientId == clientId)
            {
                tankConfigs.Remove(config);
                Debug.Log($"Removed TankConfigData for disconnected ClientId: {clientId}");
                break;
            }
        }
    }

    public void RegisterMyLoadout(TankConfigData myData)    
    {
        RegisterPlayerServerRpc(myData);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void RegisterPlayerServerRpc(TankConfigData data)
    {
        tankConfigs.Add(data);
        Debug.Log($"Registered TankConfigData for PlayerId: {data.PlayerId} on ClientId: {data.ClientId} with Team: {data.Team}");
    }
    

    public TankConfigData GetTankConfigDataForClient(ulong clientId)
    {
        foreach (var config in tankConfigs)
        {
            if (config.ClientId == clientId)
            {
                return config;
            }
        }
        return default;
    }
}
