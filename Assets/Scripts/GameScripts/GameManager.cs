using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class GameManager : NetworkBehaviour
{
    public enum GameState
    {   
        WaitingForPlayers,
        AssigningTeams,
        SpawningPlayers,
        Intro,
        InGame,
        GameOver
    }

    private GameStateBase currentState;

    public GameStateBase CurrentState 
    {
        get => currentState;
        set => currentState = value;
    }

    private GameStateFactory stateFactory;

    public bool GetIsServer => IsServer;

    public NetworkVariable<GameState> CurrentGameState = new NetworkVariable<GameState>(GameState.WaitingForPlayers);

    public static GameManager Instance;

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
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        }

        stateFactory = new GameStateFactory(this);

        currentState = stateFactory.WaitingState();
        currentState.Enter();

        CurrentGameState.OnValueChanged += OnGameStateChanged;
    }

    

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
        }
        CurrentGameState.OnValueChanged -= OnGameStateChanged;
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        SpawnManager.Instance.SpawnAllTanks();

        CurrentGameState.Value = GameState.Intro;
    }

    private void OnGameStateChanged(GameState previous, GameState current)
    {
        currentState.ChangeState(stateFactory.GetState(current));
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void StartMainGame()
    {
        if (IsHost) CurrentGameState.Value = GameState.InGame;
    }

    

}