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


    public enum TeamColor { Red, Blue }
    [Header("Team Settings")]
    public NetworkVariable<TeamColor> Team = new NetworkVariable<TeamColor>();


    public enum PlayerState { 
        Initialize,
        Idle,
        Moving,
        Dead 
    }
    [Header("Player States")]
    public NetworkVariable<PlayerState> playerState = new NetworkVariable<PlayerState>(PlayerState.Initialize);

    public override void OnNetworkSpawn()
    {
        stateFactory = new TankStateFactory(this);

        currentState = stateFactory.TankInitializeState();
        currentState.Enter();
    }
}
