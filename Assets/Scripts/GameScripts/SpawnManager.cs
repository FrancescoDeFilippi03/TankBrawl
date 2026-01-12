using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform[] redTeamSpawns;
    public Transform[] RedTeamSpawns => redTeamSpawns;
    [SerializeField] private Transform[] blueTeamSpawns;
    public Transform[] BlueTeamSpawns => blueTeamSpawns; 
    [SerializeField] private NetworkObject[] redTankPrefab;
    [SerializeField] private NetworkObject[] blueTankPrefab;



    public static SpawnManager Instance;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Transform GetSpawnPointForTeam(TeamColor team, ulong clientId)
    {
        if(team == TeamColor.Red)
        {
            return RedTeamSpawns[(int)(clientId % (ulong)RedTeamSpawns.Length)];
        }
        else
        {
            return BlueTeamSpawns[(int)(clientId % (ulong)BlueTeamSpawns.Length)];
        }
    }    
    public void SpawnAllTanks()
    {
        if (!IsServer) return;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            TankConfigData configData = TeamManager.Instance.GetTankConfigDataForClient(clientId);
            
            Transform spawnPoint = GetSpawnPointForTeam(configData.Team, clientId);
            NetworkObject tankPrefab = (configData.Team == TeamColor.Red) ? redTankPrefab[configData.TankId] : blueTankPrefab[configData.TankId];
            NetworkObject tankInstance = Instantiate(tankPrefab, spawnPoint.position, spawnPoint.rotation);
            tankInstance.SpawnAsPlayerObject(clientId);
        }
    }
}