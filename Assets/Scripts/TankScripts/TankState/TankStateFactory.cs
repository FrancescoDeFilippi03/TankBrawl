public class TankStateFactory 
{
    
    private TankStateManager tankStateManager;

    public TankStateFactory(TankStateManager tankStateManager)
    {
        this.tankStateManager = tankStateManager;
    }

    public TankInitializeState TankInitializeState()
    {
        return new TankInitializeState(tankStateManager);
    }


    //forse non necessario
    public TankBaseState GetState(TankStateManager.PlayerState state)
    {
        switch (state)
        {
            case TankStateManager.PlayerState.Initialize:
                return TankInitializeState();
            case TankStateManager.PlayerState.Idle:
                //return IdleState();
            default:
                return null;
        }
    }



}
