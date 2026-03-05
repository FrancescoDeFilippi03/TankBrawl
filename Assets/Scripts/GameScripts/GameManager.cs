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
        SpawningPlayers,
        Intro,
        InGame,
        GameOver , 
        ExitGame
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


    //score for teams
    public NetworkVariable<int> RedTeamScore = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public NetworkVariable<int> BlueTeamScore = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public NetworkVariable<TeamColor> controllingTeam = new NetworkVariable<TeamColor>(TeamColor.None,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public NetworkVariable<float> gameTimer = new NetworkVariable<float>(10f,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    [SerializeField] public GameMainUI gameMainUI;


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
        if (ConnectionManager.Instance != null)
        {
            ConnectionManager.Instance.OnSceneLoadedEvent += OnSceneLoaded;
        }

        stateFactory = new GameStateFactory(this);

        currentState = stateFactory.WaitingState();
        currentState.Enter();

        CurrentGameState.OnValueChanged += OnGameStateChanged;
    }

    

    public override void OnNetworkDespawn()
    {
        if (ConnectionManager.Instance != null)
        {
            ConnectionManager.Instance.OnSceneLoadedEvent -= OnSceneLoaded;
        }

        CurrentGameState.OnValueChanged -= OnGameStateChanged;
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        currentState.ChangeState(stateFactory.SpawningPlayersState());
    }

    private void OnGameStateChanged(GameState previous, GameState current)
    {
        currentState.ChangeState(stateFactory.GetState(current));
    }

    private void Update()
    {
        currentState?.Update();
    }
}