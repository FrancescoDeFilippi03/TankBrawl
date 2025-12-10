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

    public void SpawnRedTeam()
    {
        int spawnIndex = 0;
        foreach(ulong clientId in TeamManager.Instance.RedTeamPlayers)
        {
            Transform spawnPoint = RedTeamSpawns[spawnIndex % RedTeamSpawns.Length];
            SpawnTankForPlayer(clientId,spawnPoint);
            spawnIndex++;
        }
    }

    public  void SpawnBlueTeam()
    {
        int spawnIndex = 0;
        foreach(ulong clientId in TeamManager.Instance.BlueTeamPlayers)
        {
            Transform spawnPoint = BlueTeamSpawns[spawnIndex % BlueTeamSpawns.Length];
            SpawnTankForPlayer(clientId,spawnPoint);
            spawnIndex++;
        }
    }


    void SpawnTankForPlayer(ulong clientId, Transform spawnPoint)
    {
        NetworkObject tankNetworkObject = Instantiate(tankPrefab, spawnPoint.position,spawnPoint.rotation);
        tankNetworkObject.SpawnWithOwnership(clientId, true);
    }
}
