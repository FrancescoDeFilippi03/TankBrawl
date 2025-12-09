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
    public NetworkVariable<List<ulong>> RedTeamPlayers = new NetworkVariable<List<ulong>>(new List<ulong>());
    public NetworkVariable<List<ulong>> BlueTeamPlayers = new NetworkVariable<List<ulong>>(new List<ulong>());

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
            RedTeamPlayers.Value.Clear();
            BlueTeamPlayers.Value.Clear();
        }
    }

    public Task InitializeTeams()
    {   
        RedTeamPlayers.Value.Clear();
        BlueTeamPlayers.Value.Clear();

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            AssignAutoTeam(clientId);
        }
        return Task.CompletedTask;
    }

    public void AssignAutoTeam(ulong playerId)
    {
        if (RedTeamPlayers.Value.Count <= BlueTeamPlayers.Value.Count)
        {
            RedTeamPlayers.Value.Add(playerId);
            Debug.Log($"Player {playerId} assigned to Red Team");
        }
        else
        {
            BlueTeamPlayers.Value.Add(playerId);
            Debug.Log($"Player {playerId} assigned to Blue Team");
        }
    }

    public TeamColor GetPlayerTeam(ulong playerId)
    {
        if (RedTeamPlayers.Value.Contains(playerId))
        {
            return TeamColor.Red;
        }
        else if (BlueTeamPlayers.Value.Contains(playerId))
        {
            return TeamColor.Blue;
        }
        else
        {
            throw new System.Exception($"Player {playerId} is not assigned to any team.");
        }
    }

}
