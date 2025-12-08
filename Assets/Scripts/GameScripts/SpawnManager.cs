using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(TeamManager))]
public class SpawnManager : NetworkBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform[] redTeamSpawns;
    public Transform[] RedTeamSpawns => redTeamSpawns;
    [SerializeField] private Transform[] blueTeamSpawns;
    public Transform[] BlueTeamSpawns => blueTeamSpawns;
    [SerializeField] private NetworkObject tankPrefab;

    private TeamManager teamManager;
    public TeamManager TeamManager => teamManager;
    public NetworkObject TankPrefab => tankPrefab;


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            teamManager = GetComponent<TeamManager>();
        }
    }

    public void SpawnRedTeam()
    {
        int spawnIndex = 0;
        foreach(ulong clientId in teamManager.RedTeamPlayers.Value)
        {
            Transform spawnPoint = RedTeamSpawns[spawnIndex % RedTeamSpawns.Length];
            SpawnTankForPlayer(clientId, TeamManager.Team.Red, spawnPoint);
            spawnIndex++;
        }
    }

    public void SpawnBlueTeam()
    {
        int spawnIndex = 0;
        foreach(ulong clientId in teamManager.BlueTeamPlayers.Value)
        {
            Transform spawnPoint = BlueTeamSpawns[spawnIndex % BlueTeamSpawns.Length];
            SpawnTankForPlayer(clientId, TeamManager.Team.Blue, spawnPoint);
            spawnIndex++;
        }
    }


    void SpawnTankForPlayer(ulong clientId, TeamManager.Team team , Transform spawnPoint)
    {
        NetworkObject tank = Instantiate(tankPrefab, spawnPoint.position, Quaternion.identity);
        tank.SpawnAsPlayerObject(clientId, true);

        if (tank.TryGetComponent<TankPlayer>(out var tankPlayer))
        {
            tankPlayer.Initialize(team == TeamManager.Team.Red ? TankPlayer.TeamColor.Red : TankPlayer.TeamColor.Blue);
        }
    }
}
