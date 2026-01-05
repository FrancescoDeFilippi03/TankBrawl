public abstract class TankBaseState
{
    
    protected TankStateManager tank;
    public TankBaseState(TankStateManager tankStateManager)
    {
        tank = tankStateManager;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void CheckStateChange() { }
    public void ChangeState(TankBaseState newState)
    {
        if (tank.CurrentState != null && tank.CurrentState.GetType() == newState.GetType()) return;
        
        Exit();

        tank.CurrentState = newState;
        tank.CurrentState.Enter();
    }
}
