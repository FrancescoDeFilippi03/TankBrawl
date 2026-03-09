using UnityEngine;
using Unity.Netcode;
public enum TeamColor
{
    Red,
    Blue,
    None
}

public class SessionDataManager : NetworkBehaviour
{
    public static SessionDataManager Instance { get; private set; }

    public NetworkList<SessionPlayerData> Players;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Players = new NetworkList<SessionPlayerData>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
        
        if (IsClient)
        {
            SendLocalPlayerDataToServer();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void SendLocalPlayerDataToServer()
    {
        var localData = PlayerDataManager.Instance;
        RegisterPlayerServerRpc(NetworkManager.Singleton.LocalClientId,localData.PlayerName, localData.SelectedTankIndex);
    }

    [Rpc(SendTo.Server)]
    private void RegisterPlayerServerRpc(ulong clientId, string playerName, int tankId)
    {
        TeamColor assignedTeam = (Players.Count % 2 == 0) ? TeamColor.Red : TeamColor.Blue;

        SessionPlayerData newPlayer = new SessionPlayerData(clientId, playerName, tankId, assignedTeam);
        
        Players.Add(newPlayer);
        Debug.Log($"Giocatore {playerName} (Client: {clientId}) registrato nel Team {assignedTeam}.");
    }


    private void OnClientDisconnected(ulong clientId)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].ClientId == clientId)
            {
                Players.RemoveAt(i);
                break;
            }
        }
    }

    public SessionPlayerData GetPlayerData(ulong clientId)
    {
        foreach (var player in Players)
        {
            if (player.ClientId == clientId) return player;
        }
        return default;
    }

}
