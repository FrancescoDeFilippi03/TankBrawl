
public class TankStateFactory
{
    
    private TankStateManager tankStateManager;

    public TankStateFactory(TankStateManager tankStateManager)
    {
        this.tankStateManager = tankStateManager;
    }


    public TankIdleState Idle()
    {
        return new TankIdleState(tankStateManager);
    }

    public TankMovingState Moving()
    {
        return new TankMovingState(tankStateManager);
    }

    public TankDashingState Dashing()
    {
        return new TankDashingState(tankStateManager);
    }

    public TankDeadState Dead()
    {
        return new TankDeadState(tankStateManager);
    }

    public TankRespawnState Respawn()
    {
        return new TankRespawnState(tankStateManager);
    }

    //forse non necessario
    public TankBaseState GetState(TankStateManager.PlayerState state)
    {
        switch (state)
        {
            case TankStateManager.PlayerState.Idle:
                return Idle();
            case TankStateManager.PlayerState.Moving:
                return Moving();
            case TankStateManager.PlayerState.Dashing:
                return Dashing();
            case TankStateManager.PlayerState.Dead:
                return Dead();
            case TankStateManager.PlayerState.Respawn:
                return Respawn();
            default:
                return null;
        }
    }

}
