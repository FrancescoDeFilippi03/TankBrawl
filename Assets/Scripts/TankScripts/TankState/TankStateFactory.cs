
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

    public TankDeadState Dead()
    {
        return new TankDeadState(tankStateManager);
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
            case TankStateManager.PlayerState.Dead:
                return Dead();
            default:
                return null;
        }
    }

}
