using UnityEngine;

public class TankInitializeState : TankBaseState
{
    public TankInitializeState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Initialize State");
        
    }

    public override void Exit()
    {
        Debug.Log("Exiting Initialize State");
        // Cleanup logic here
    }

    public override void Update()
    {
        // Update logic here
    }
}
