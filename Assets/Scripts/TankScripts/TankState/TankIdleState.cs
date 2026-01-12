using UnityEngine;

public class TankIdleState : TankBaseState
{
    
    public TankIdleState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Update()
    {
        
        base.Update();

        CheckStateChange();
        Tank.HandleTurretRotation(PlayerController.AimInput);
    }

    public override void FixedUpdate()
    {
    }

    public override void CheckStateChange()
    {
        if(PlayerController.MovementInput.magnitude > 0.1f)
        {
            tank.playerState.Value = TankStateManager.PlayerState.Moving;
            ChangeState(tank.StateFactory.Moving());
        }

        if (PlayerController.IsDashing)
        {
            tank.playerState.Value = TankStateManager.PlayerState.Dashing;
            ChangeState(tank.StateFactory.Dashing());
        }
    }
}
