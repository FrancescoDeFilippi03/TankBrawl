using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum TeamColor
{
    Red,
    Blue
}

public class TeamManager : NetworkBehaviour
{
    public NetworkList<ulong> RedTeamPlayers = new NetworkList<ulong>();
    public NetworkList<ulong> BlueTeamPlayers = new NetworkList<ulong>();

    public static TeamManager Instance;


    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            RedTeamPlayers.Clear();
            BlueTeamPlayers.Clear();
        }
    }

    public override void OnNetworkDespawn()
    {

        RedTeamPlayers?.Dispose();
        BlueTeamPlayers?.Dispose();
    }

    public void InitializeTeams()
    {   
        RedTeamPlayers.Clear();
        BlueTeamPlayers.Clear();

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            AssignAutoTeam(clientId);
        }
    }

    public void AssignAutoTeam(ulong playerId)
    {
        if (RedTeamPlayers.Count <= BlueTeamPlayers.Count)
        {
            RedTeamPlayers.Add(playerId);
            Debug.Log($"Player {playerId} assigned to Red Team");
        }
        else
        {
            BlueTeamPlayers.Add(playerId);
            Debug.Log($"Player {playerId} assigned to Blue Team");
        }
    }

    public TeamColor GetPlayerTeam(ulong playerId)
    {
        if (RedTeamPlayers.Contains(playerId))
        {
            return TeamColor.Red;
        }
        else if (BlueTeamPlayers.Contains(playerId))
        {
            return TeamColor.Blue;
        }
        else
        {
            throw new System.Exception($"Player {playerId} is not assigned to any team.");
        }
    }

}
