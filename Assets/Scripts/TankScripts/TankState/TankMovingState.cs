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


    public override void FixedUpdate()
    {
        MoveTank();
        CheckStateChange();
    }

    public override void CheckStateChange()
    {
        if(tank.MovementInput.magnitude <= 0.1f)
        {
            tank.playerState.Value = TankStateManager.PlayerState.Idle;
            ChangeState(tank.StateFactory.Idle());
        }
    }

    private void MoveTank()
    {
        Vector2 movement = tank.MovementInput.normalized * tank.TankPlayerData.Speed * Time.fixedDeltaTime;
        tank.Rb.MovePosition(tank.Rb.position + movement);
    }

    
}