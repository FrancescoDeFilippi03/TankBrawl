using UnityEngine;

public class TankInitializeState : TankBaseState
{
    public TankInitializeState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Initialize State");
        // Initialization logic here


        if (!tank.IsOwner) return;
        
        tank.tankPlayerData.Init(tank.playerNetworkConfigData.Value);


        tank.playerState.Value = TankStateManager.PlayerState.Idle;

        tank.CurrentState = tank.StateFactory.Idle();
        tank.CurrentState.Enter();
    }

    public override void Exit()
    {
        Debug.Log("Exiting Initialize State");
    }

    

}
