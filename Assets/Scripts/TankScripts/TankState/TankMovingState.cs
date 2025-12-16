using UnityEngine;

public class TankMovingState : TankBaseState
{
    public TankMovingState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        tank.PlayerController.TankAnimator.SetBool("isMoving", true);
    }

    public override void Exit()
    {
        tank.PlayerController.TankAnimator.SetBool("isMoving", false);
    }
    public override void Update()
    {
        CheckStateChange();
    }
    public override void FixedUpdate()
    {
        
        tank.PlayerController.MoveTank();
    }
    public override void CheckStateChange()
    {        
        if(!tank.IsOwner) return;
        if(tank.PlayerController.MovementInput.magnitude <= 0.1f)
        {
            tank.playerState.Value = TankStateManager.PlayerState.Idle;
            ChangeState(tank.StateFactory.Idle());
        }

    }

    

    

    
}