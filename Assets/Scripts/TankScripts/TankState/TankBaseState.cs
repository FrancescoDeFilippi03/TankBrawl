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
    protected void ChangeState(TankBaseState newState)
    {
        Exit();
        
        tank.CurrentState = newState;
        
        tank.CurrentState.Enter();
    }
}
