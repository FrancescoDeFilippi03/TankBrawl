using UnityEngine;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;

public class GameManager : NetworkBehaviour
{

    [SerializeField] private Transform[] redTeamSpawns;
    [SerializeField] private Transform[] blueTeamSpawns;

    [SerializeField] private NetworkObject tankPrefab;

    public enum GameState
    {   
        WaitingForPlayers,
        Intro,
        InGame,
        GameOver
    }

    public NetworkVariable<GameState> CurrentGameState = new NetworkVariable<GameState>(GameState.WaitingForPlayers);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        }

        CurrentGameState.OnValueChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState previous, GameState current)
    {
        Debug.Log($"Game State changed from {previous} to {current}");
        // Handle state-specific logic here

        switch (current)
        {
            case GameState.WaitingForPlayers:
                // Logic for waiting state
                break;
            case GameState.Intro:
                // Logic for intro state
                ShowIntroSequence();
                break;
            case GameState.InGame:
                // Logic for in-game state
                PlayingGame();
                break;
            case GameState.GameOver:
                // Logic for game over state
                NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
                NetworkManager.Singleton.SceneManager.LoadScene("Lobby", UnityEngine.SceneManagement.LoadSceneMode.Single);
                break;
        }
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        CurrentGameState.Value = GameState.Intro;
    }


    private void ShowIntroSequence()
    {
        Debug.Log("Showing Intro Sequence...");
        Invoke(nameof(StartMainGame), 5f);
    }

    private void StartMainGame()
    {
        if (IsServer)
        {
            CurrentGameState.Value = GameState.InGame;
            AssignAutoTeam();
        }
    }

    private void PlayingGame()
    {
        Debug.Log("Game is now in progress!");
        Invoke(nameof(EndGame), 10f);
    }

    private void EndGame()
    {
        if (IsServer)
        {
            CurrentGameState.Value = GameState.GameOver;
            
        }
    }
    private void AssignAutoTeam()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            
            
            Vector3 spawnPosition;
            // Logica semplice pari/dispari per 3v3
            // ID 0, 2, 4 -> Red
            // ID 1, 3, 5 -> Blue
            if (client.ClientId % 2 == 0)
            {
                spawnPosition = redTeamSpawns[client.ClientId / 2].position;
                
            }
            else
            {
                spawnPosition = blueTeamSpawns[client.ClientId / 2].position;
            }
            NetworkObject tank = Instantiate(tankPrefab , spawnPosition, Quaternion.identity);
            tank.SpawnWithOwnership(client.ClientId, true);
            
            TankPlayer tankPlayer = tank.GetComponent<TankPlayer>();
            tankPlayer.InitializeServerRpc((client.ClientId % 2 == 0) ? TankPlayer.TeamColor.Red : TankPlayer.TeamColor.Blue);
        }

    }

   /*  NetworkObject tank = Instantiate(tankPrefab);
        TankPlayer tankPlayer = tank.GetComponent<TankPlayer>();
        Vector3 spawnPosition;
        // Logica semplice pari/dispari per 3v3
        // ID 0, 2, 4 -> Red
        // ID 1, 3, 5 -> Blue
        if (OwnerClientId % 2 == 0)
        {
            tankPlayer.Initialize(TankPlayer.TeamColor.Red);
            spawnPosition = redTeamSpawns[OwnerClientId / 2].position;
        }
        else
        {
            tankPlayer.Initialize(TankPlayer.TeamColor.Blue);
            spawnPosition = blueTeamSpawns[OwnerClientId / 2].position;
        }

        tank.SpawnWithOwnership(OwnerClientId, true); */
}