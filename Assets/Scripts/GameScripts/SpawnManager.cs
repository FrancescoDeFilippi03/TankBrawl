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
            SpawnTankForPlayer(clientId,spawnPoint);
            spawnIndex++;
        }
    }

    public void SpawnBlueTeam()
    {
        int spawnIndex = 0;
        foreach(ulong clientId in teamManager.BlueTeamPlayers.Value)
        {
            Transform spawnPoint = BlueTeamSpawns[spawnIndex % BlueTeamSpawns.Length];
            SpawnTankForPlayer(clientId,spawnPoint);
            spawnIndex++;
        }
    }


    void SpawnTankForPlayer(ulong clientId, Transform spawnPoint)
    {
        NetworkObject tank = Instantiate(tankPrefab, spawnPoint.position, Quaternion.identity);
        tank.SpawnAsPlayerObject(clientId, true);
    }
}
