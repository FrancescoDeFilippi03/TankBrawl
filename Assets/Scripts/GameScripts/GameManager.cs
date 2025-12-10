using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public enum GameState
    {   
        WaitingForPlayers,
        AssigningTeams,
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
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
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
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }
        CurrentGameState.OnValueChanged -= OnGameStateChanged;
    }


    private void OnGameStateChanged(GameState previous, GameState current)
    {
        //Debug.Log($"Game State: {previous} -> {current}");
        currentState.ChangeState(stateFactory.GetState(current));
    }

    private void Update()
    {
        //Debug.Log($"Current State: {currentState.GetType().Name}");
        currentState?.Update();
    }

    // HOST CONTROL: Metodo pubblico per il bottone dell'Editor (da cambiare in Futuro)
    public void StartMatch()
    {
        if (IsHost && CurrentGameState.Value == GameState.WaitingForPlayers)
        {
            CurrentGameState.Value = GameState.AssigningTeams;
        }
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        //Inizia partita automaticamente quando la scena di gioco è caricata
    }

    public void StartMainGame()
    {
        if (IsHost) CurrentGameState.Value = GameState.InGame;
    }

    

}