using System;
using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(TankPlayerData))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
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

    //change from Initialize to Idle state
    public NetworkVariable<PlayerState> playerState = new NetworkVariable<PlayerState>(PlayerState.Idle, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );
    public enum PlayerState { 
        Initialize,
        Idle,
        Moving,
        Dead 
    }
    
    /* public NetworkVariable<TankConfigData> playerNetworkConfigData = new NetworkVariable<TankConfigData>(
        new TankConfigData()
        ,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    ); */
    public NetworkVariable<TankScoreData> NetScore = new NetworkVariable<TankScoreData>();
    
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
       // playerNetworkConfigData.OnValueChanged += OnConfigDataChanged;

        stateFactory = new TankStateFactory(this);
        tankInput = new TankInput();
        rb = GetComponent<Rigidbody2D>();
        tankPlayerData = GetComponent<TankPlayerData>();
        tankAnimator = GetComponent<Animator>();

        if (IsOwner)
        {
            OwnerInit();
        }
        
        
        currentState = stateFactory.GetState(playerState.Value);
        currentState.Enter();
    }


    void OwnerInit()
    {
        /* playerNetworkConfigData.Value = new TankDataBuilder()
            .WithTeam(TeamManager.Instance.GetPlayerTeam(OwnerClientId))
            .WithLoadout()
            .Build(); */

            
        tankInput.Enable();
    }

    public override void OnNetworkDespawn()
    {
        playerState.OnValueChanged -= OnPlayerStateChanged;
        //playerNetworkConfigData.OnValueChanged -= OnConfigDataChanged;

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

   /*  private void OnConfigDataChanged(TankConfigData previousValue, TankConfigData newValue)
    {
        Debug.Log($"[Tank] OnConfigDataChanged - ClientId: {OwnerClientId}, IsOwner: {IsOwner}, Team: {newValue.Team}");
        
        // Non-owner: quando arrivano i dati dal owner, inizializza gli sprite
        if (!IsOwner)
        {
            Debug.Log($"[Tank] Inizializzazione sprite per tank remoto ClientId: {OwnerClientId}");
            tankPlayerData.Init(newValue);
        }
    }
 */
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
    }

    
}
