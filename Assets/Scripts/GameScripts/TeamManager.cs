using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TeamManager : NetworkBehaviour
{
    public enum Team
    {
        Red,
        Blue
    }

    public NetworkVariable<List<ulong>> RedTeamPlayers = new NetworkVariable<List<ulong>>(new List<ulong>());
    public NetworkVariable<List<ulong>> BlueTeamPlayers = new NetworkVariable<List<ulong>>(new List<ulong>());

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

}
