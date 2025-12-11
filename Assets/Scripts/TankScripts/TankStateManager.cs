using System;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;


[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(TankPlayerData))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class TankStateManager : NetworkBehaviour
{
    //This class will manage only the states of the tank
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

    public NetworkVariable<PlayerState> playerState = new NetworkVariable<PlayerState>(PlayerState.Idle, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );
    public enum PlayerState { 
        Initialize,
        Idle,
        Moving,
        Dead 
    }    
    private TankPlayerData tankPlayerData;
    public TankPlayerData TankPlayerData => tankPlayerData;

    // Input
    private TankInput tankInput;
    public TankInput TankInput => tankInput;


    //movement variables
    private Vector2 movementInput;
    public Vector2 MovementInput => movementInput;

    private Vector2 smoothedMovementInput;
    public Vector2 SmoothedMovementInput
    {
        get => smoothedMovementInput;
        set => smoothedMovementInput = value;
    }

    [SerializeField] private float movementSmoothing = 5f;
    public float MovementSmoothing => movementSmoothing;

    [SerializeField] private float rotationSmoothing = 7f;
    public float RotationSmoothing => rotationSmoothing;

    private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;
    
    private Animator tankAnimator;
    public Animator TankAnimator => tankAnimator;

    public override void OnNetworkSpawn()
    {
        playerState.OnValueChanged += OnPlayerStateChanged;

        stateFactory = new TankStateFactory(this);
        tankInput = new TankInput();
        rb = GetComponent<Rigidbody2D>();
        tankPlayerData = GetComponent<TankPlayerData>();
        tankAnimator = GetComponent<Animator>();

        if (IsOwner) tankInput.Enable();

        tankPlayerData.Init(TeamManager.Instance.GetTankConfigDataForClient(OwnerClientId));
       
        currentState = stateFactory.GetState(playerState.Value);
        currentState.Enter();
    }
    public override void OnNetworkDespawn()
    {
        playerState.OnValueChanged -= OnPlayerStateChanged;

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
    private void Update()
    {
        currentState?.Update();
        currentState?.CheckStateChange();

        if (!IsOwner) return;
        movementInput = tankInput.Tank.Movement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

}
