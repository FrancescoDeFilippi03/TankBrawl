using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;

[RequireComponent(typeof(TeamManager))]
public class GameManager : NetworkBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform[] redTeamSpawns;
    [SerializeField] private Transform[] blueTeamSpawns;
    [SerializeField] private NetworkObject tankPrefab;

    private TeamManager teamManager;

    public enum GameState
    {   
        WaitingForPlayers,
        AssigningTeams,
        Intro,
        InGame,
        GameOver
    }

    public NetworkVariable<GameState> CurrentGameState = new NetworkVariable<GameState>(GameState.WaitingForPlayers);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            teamManager = GetComponent<TeamManager>();
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        }
        CurrentGameState.OnValueChanged += OnGameStateChanged;
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }
        CurrentGameState.OnValueChanged -= OnGameStateChanged;
    }

    private async void OnGameStateChanged(GameState previous, GameState current)
    {
        Debug.Log($"Game State: {current}");

        switch (current)
        {
            case GameState.WaitingForPlayers:
                break; 

            case GameState.AssigningTeams:
                await AssignTeams();
                CurrentGameState.Value = GameState.Intro; 
                break;

            case GameState.Intro:
                Invoke(nameof(StartMainGame), 5f);
                break;

            case GameState.InGame:
                Debug.Log("Game Started!");
                Invoke(nameof(EndGame), 10f);
                break;

            case GameState.GameOver:
                EndGameSequence();
                break;
        }
    }

    // HOST CONTROL: Metodo pubblico per il bottone dell'Editor
    public void StartMatch()
    {
        if (IsServer && CurrentGameState.Value == GameState.WaitingForPlayers)
        {
            CurrentGameState.Value = GameState.AssigningTeams;
        }
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        if (CurrentGameState.Value == GameState.WaitingForPlayers)
        {
            Debug.Log($"Client {clientId} loaded. Ready.");
        }
    }

    private void StartMainGame()
    {
        if (IsServer) CurrentGameState.Value = GameState.InGame;
    }

    private void EndGame()
    {
        if (IsServer) CurrentGameState.Value = GameState.GameOver;
    }

    // LOGICA DI USCITA PULITA
    private async void EndGameSequence()
    {
        if (SessionManager.Instance != null)
        {
            await SessionManager.Instance.CleanupAfterGame();
        }

        NetworkManager.Singleton.Shutdown();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }

    private async Task AssignTeams()
    {
        if (!IsServer) return;
        await teamManager.InitializeTeams();

        SpawnRedTeam();
        SpawnBlueTeam();
    }

    private void SpawnRedTeam()
    {
        foreach(ulong clientId in teamManager.RedTeamPlayers.Value)
        {
            int spawnIndex = (int)(clientId / 2);

            Transform spawnPoint = redTeamSpawns[spawnIndex];
            SpawnTankForPlayer(clientId, TeamManager.Team.Red, spawnPoint);
        }
    }

    private void SpawnBlueTeam()
    {
        foreach(ulong clientId in teamManager.BlueTeamPlayers.Value)
        {
            int spawnIndex = (int)(clientId / 2);
            Transform spawnPoint = blueTeamSpawns[spawnIndex];
            SpawnTankForPlayer(clientId, TeamManager.Team.Blue, spawnPoint);
        }
    }


    void SpawnTankForPlayer(ulong clientId, TeamManager.Team team , Transform spawnPoint)
    {
        NetworkObject tank = Instantiate(tankPrefab, spawnPoint.position, Quaternion.identity);
        tank.SpawnAsPlayerObject(clientId, true);

        TankPlayer tankPlayer = tank.GetComponent<TankPlayer>();
        if (tankPlayer != null)
        {
            tankPlayer.InitializeServerRpc(team == TeamManager.Team.Red ? TankPlayer.TeamColor.Red : TankPlayer.TeamColor.Blue);
        }
    }
}