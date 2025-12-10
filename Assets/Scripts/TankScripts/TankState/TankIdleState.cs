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

    public override void Update()
    {
       CheckStateChange();
    }

    public override void CheckStateChange()
    {
        // Idle state might not need FixedUpdate logic
        if(tank.MovementInput.magnitude > 0.1f)
        {
            tank.playerState.Value = TankStateManager.PlayerState.Moving;
            ChangeState(tank.StateFactory.Moving());
        }
    }
}
