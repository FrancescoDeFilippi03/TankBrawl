using UnityEngine;

public class TankMovingState : TankBaseState
{
    public TankMovingState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Moving State");
        // Moving state logic here
    }

    public override void Exit()
    {
        Debug.Log("Exiting Moving State");
    }


    
}