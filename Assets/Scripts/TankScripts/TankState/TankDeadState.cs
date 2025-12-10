using UnityEngine;

public class TankDeadState : TankBaseState
{
    public TankDeadState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Dead State");
        // Dead state logic here
    }

    public override void Exit()
    {
        Debug.Log("Exiting Dead State");
    }
}