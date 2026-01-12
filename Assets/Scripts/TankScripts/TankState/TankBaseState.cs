public abstract class TankBaseState
{
    
    protected TankStateManager tank;

    //references for easier access
    protected Tank Tank => tank.Tank;
    protected TankPlayerController PlayerController => tank.PlayerController;
    protected bool IsOwner => tank.IsOwner;

    public TankBaseState(TankStateManager tankStateManager)
    {
        tank = tankStateManager;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { 
        CheckDeath();
    }
    public virtual void FixedUpdate() { 
    }
    public virtual void CheckStateChange() { }
    
    protected void CheckDeath()
    {
        if (!IsOwner) return;
        if (this is TankDeadState || this is TankRespawnState) return;
        
        if (Tank.Health.Value <= 0)
        {
            tank.playerState.Value = TankStateManager.PlayerState.Dead;
            ChangeState(tank.StateFactory.Dead());
        }
    }
    
    public void ChangeState(TankBaseState newState)
    {
        if (!IsOwner) return;
        
        if (tank.CurrentState != null && tank.CurrentState.GetType() == newState.GetType()) return;
        
        Exit();

        tank.CurrentState = newState;
        tank.CurrentState.Enter();
    }

}
