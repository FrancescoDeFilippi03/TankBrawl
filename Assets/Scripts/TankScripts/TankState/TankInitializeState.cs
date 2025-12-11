public class TankInitializeState : TankBaseState
{
    public TankInitializeState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        
        if (tank.IsOwner)
        {
            //tank.TankPlayerData.Init(tank.playerNetworkConfigData.Value);
            TransitionToIdle();
        }
    }

    private void TransitionToIdle()
    {
        tank.playerState.Value = TankStateManager.PlayerState.Idle;
        
        tank.CurrentState = tank.StateFactory.Idle();
        tank.CurrentState.Enter();
    }

}
