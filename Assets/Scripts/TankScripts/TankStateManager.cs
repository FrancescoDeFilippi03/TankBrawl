using System;
using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(TankPlayerData))]
public class TankStateManager : NetworkBehaviour
{

    private TankBaseState currentState;
    public TankBaseState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    private TankStateFactory stateFactory;
    public TankStateFactory StateFactory
    {
        get { return stateFactory; }
    }


    public NetworkVariable<PlayerState> playerState = new NetworkVariable<PlayerState>(PlayerState.Initialize);
    public enum PlayerState { 
        Initialize,
        Idle,
        Moving,
        Dead 
    }
    
    public NetworkVariable<TankConfigData> playerNetworkConfigData = new NetworkVariable<TankConfigData>();
    public NetworkVariable<TankScoreData> NetScore = new NetworkVariable<TankScoreData>();
    
    public TankPlayerData tankPlayerData;
    

    public override void OnNetworkSpawn()
    {
        playerState.OnValueChanged += OnPlayerStateChanged;

        // Initialize State Factory and set initial state
        stateFactory = new TankStateFactory(this);
        currentState = stateFactory.TankInitializeState();
        currentState.Enter();
        
    }

    public override void OnNetworkDespawn()
    {
        playerState.OnValueChanged -= OnPlayerStateChanged;
    }

    private void OnPlayerStateChanged(PlayerState previousValue, PlayerState newValue)
    {
        currentState.ChangeState(stateFactory.GetState(newValue));
    }

    private void Update()
    {
        currentState?.Update();
    }



}
