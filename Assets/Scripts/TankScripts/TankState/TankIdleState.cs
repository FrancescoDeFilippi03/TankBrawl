using UnityEngine;

public class TankIdleState : TankBaseState
{
    
    public TankIdleState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");
        // Idle state logic here
    }

    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}
