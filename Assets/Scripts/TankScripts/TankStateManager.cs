using System;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;


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
        Idle,
        Moving,
        Dashing,
        Dead , 
        Respawn
    }    

    [SerializeField] private TankPlayerController playerController;
    public TankPlayerController PlayerController => playerController;
    
    [SerializeField] private Tank tank;
    public Tank Tank => tank;
    
    public override void OnNetworkSpawn()
    {
        playerState.OnValueChanged += OnPlayerStateChanged;

        stateFactory = new TankStateFactory(this);

        currentState = stateFactory.GetState(playerState.Value);
        currentState.Enter();
        
        // Subscribe to Tank events
        if (IsOwner && tank != null)
        {
            tank.OnDeath += HandleTankDeath;
        }
    }
    public override void OnNetworkDespawn()
    {
        playerState.OnValueChanged -= OnPlayerStateChanged;
        
        // Unsubscribe from Tank events
        if (IsOwner && tank != null)
        {
            tank.OnDeath -= HandleTankDeath;
        }
    }
    
    private void HandleTankDeath()
    {
        if (!IsOwner) return;
        
        playerState.Value = PlayerState.Dead;
        currentState.ChangeState(stateFactory.Dead());
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
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }


    


    
}
