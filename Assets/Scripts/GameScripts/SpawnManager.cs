using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform[] redTeamSpawns;
    public Transform[] RedTeamSpawns => redTeamSpawns;
    [SerializeField] private Transform[] blueTeamSpawns;
    public Transform[] BlueTeamSpawns => blueTeamSpawns; 
    [SerializeField] private NetworkObject tankPrefab;

    public NetworkObject TankPrefab => tankPrefab;

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

    Transform GetSpawnPointForTeam(TeamColor team, ulong clientId)
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
    
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!NetworkLifecycle.ClientIdToPlayerId.TryGetValue(clientId, out string playerId))
            {
                Debug.LogError($"Impossibile trovare PlayerId per il client {clientId}");
                continue; 
            }

            TankConfigData configData = SessionManager.Instance.GetTankConfigDataForPlayer(playerId);

            Transform spawnPoint = GetSpawnPointForTeam(configData.Team, clientId);
            
            NetworkObject tankNetworkObject = Instantiate(tankPrefab, spawnPoint.position, spawnPoint.rotation);
            
            tankNetworkObject.GetComponent<TankPlayerData>().Init(configData);

            tankNetworkObject.SpawnAsPlayerObject(clientId, true);

            var stateManager = tankNetworkObject.GetComponent<TankStateManager>();
            stateManager.SetNetworkConfig(configData);
        }
    }
}
