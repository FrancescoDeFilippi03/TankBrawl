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


    public NetworkVariable<PlayerState> playerState = new NetworkVariable<PlayerState>(PlayerState.Initialize,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );
    public enum PlayerState { 
        Initialize,
        Idle,
        Moving,
        Dead 
    }
    
    public NetworkVariable<TankConfigData> playerNetworkConfigData = new NetworkVariable<TankConfigData>(
        new TankConfigData()
        ,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );
    public NetworkVariable<TankScoreData> NetScore = new NetworkVariable<TankScoreData>();
    
    public TankPlayerData tankPlayerData;
    

    // Input
    private TankInput tankInput;
    public TankInput TankInput => tankInput;


    //movement variables
    private Vector2 movementInput;
    public Vector2 MovementInput => movementInput;

    private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;
    


    public override void OnNetworkSpawn()
    {
        playerState.OnValueChanged += OnPlayerStateChanged;
        playerNetworkConfigData.OnValueChanged += OnConfigDataChanged;

        stateFactory = new TankStateFactory(this);
        tankInput = new TankInput();

        rb = GetComponent<Rigidbody2D>();
        tankPlayerData = GetComponent<TankPlayerData>();

        if (IsOwner)
        {
            OwnerInit();
        }
            
        
        currentState = stateFactory.GetState(playerState.Value);
        currentState.Enter();
        
    }


    void OwnerInit()
    {
        playerNetworkConfigData.Value = new TankDataBuilder()
            .WithTeam(TeamManager.Instance.GetPlayerTeam(OwnerClientId))
            .WithLoadout()
            .SetReady()
            .Build();

            
        tankInput.Enable();
    }

    public override void OnNetworkDespawn()
    {
        playerState.OnValueChanged -= OnPlayerStateChanged;
        playerNetworkConfigData.OnValueChanged -= OnConfigDataChanged;

        if (IsOwner)
        {
            tankInput.Disable();
        }
    }

    private void OnPlayerStateChanged(PlayerState previousValue, PlayerState newValue)
    {
        if(IsOwner) return;
        currentState.ChangeState(stateFactory.GetState(newValue));
    }

    private void OnConfigDataChanged(TankConfigData previousValue, TankConfigData newValue)
    {
        // Handle any logic needed when the tank's config data changes
        tankPlayerData.Init(playerNetworkConfigData.Value);

    }

    private void Update()
    {
        currentState?.Update();
        currentState?.CheckStateChange();

        // Capture movement input
        if (!IsOwner) return;
        movementInput = tankInput.Tank.Movement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdate();
        currentState?.CheckStateChange();
    }


    
}
