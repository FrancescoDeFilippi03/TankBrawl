using System;
using Unity.Netcode;
using UnityEngine;

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
    
    public NetworkVariable<TankConfigData> playerConfigData = new NetworkVariable<TankConfigData>();
    public NetworkVariable<TankScoreData> NetScore = new NetworkVariable<TankScoreData>();
    
    //Tank Elements
    [Header("Tank Elements")]
    public Bullet tankBullet;
    public Weapon tankWeapon;
    public Turret tankTurret;
    public Base   tankBase;

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
        
    }

    private void Update()
    {
        currentState?.Update();
    }



}
