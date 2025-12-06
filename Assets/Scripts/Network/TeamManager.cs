using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance { get; private set; }

    //(0 for Team A, 1 for Team B)
    public Dictionary<ulong, int> PlayerTeams = new Dictionary<ulong, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }
        Instance = this;

        
    }

    public async Task InitializeAsync()
    {
        if (NetworPlayerManager.Instance != null)
        {
            NetworPlayerManager.Instance.PlayerConnected += AssignTeam;
            NetworPlayerManager.Instance.PlayerDisconnected += HandlePlayerDisconnected;
            NetworPlayerManager.Instance.PlayerListCleared += ResetTeams;
        }
    }

    void OnDestroy()
    {
        if (NetworPlayerManager.Instance != null)
        {
            NetworPlayerManager.Instance.PlayerConnected -= AssignTeam;
            NetworPlayerManager.Instance.PlayerDisconnected -= HandlePlayerDisconnected;
            NetworPlayerManager.Instance.PlayerListCleared -= ResetTeams;
        }
    }

    public void ResetTeams()
    {
        // When Host starts a new game, ensure list is empty
        if (NetworkManager.Singleton.IsServer)
        {
            PlayerTeams.Clear();
        }
    }

    public void AssignTeam(ulong clientId)
    {
        // Simple logic: Even clientIds to Team 0, Odd clientIds to Team 1
        int team = (clientId % 2 == 0) ? 0 : 1;
        PlayerTeams[clientId] = team;
        Debug.Log($"Assigned Client {clientId} to Team {team}");
    }

    private void HandlePlayerDisconnected(ulong clientId)
    {
        if (PlayerTeams.ContainsKey(clientId))
        {
            PlayerTeams.Remove(clientId);
            Debug.Log($"Removed Client {clientId} from Team list.");
        }
    }
    

}