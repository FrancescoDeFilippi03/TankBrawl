using UnityEngine;

public class TankMovingState : TankBaseState
{
    public TankMovingState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        Tank.TankAnimator.SetBool("isMoving", true);
    }

    public override void Exit()
    {
        Tank.TankAnimator.SetBool("isMoving", false);
    }
    public override void Update()
    {
        base.Update();
        CheckStateChange();
        Tank.HandleTurretRotation(PlayerController.AimInput);
    }
    public override void FixedUpdate()
    {
        Tank.MoveTank(PlayerController.MovementInput);
        Tank.HandleRotation(PlayerController.MovementInput);
    }
    public override void CheckStateChange()
    {        
        if(PlayerController.MovementInput.magnitude <= 0.1f)
        {
            tank.playerState.Value = TankStateManager.PlayerState.Idle;
            ChangeState(tank.StateFactory.Idle());
        }

        if (PlayerController.IsDashing)
        {
            tank.playerState.Value = TankStateManager.PlayerState.Dashing;
            ChangeState(tank.StateFactory.Dashing());
        }
    }
}