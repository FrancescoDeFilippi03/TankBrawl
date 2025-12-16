using UnityEngine;

public class TankIdleState : TankBaseState
{
    
    public TankIdleState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Update()
    {
       CheckStateChange();
    }

    public override void CheckStateChange()
    {        
        if(!tank.IsOwner) return;
        if(tank.PlayerController.MovementInput.magnitude > 0.1f)
        {
            tank.playerState.Value = TankStateManager.PlayerState.Moving;
            ChangeState(tank.StateFactory.Moving());
        }


        if(tank.PlayerController.TankHealthManager.Health <= 0)
        {
            tank.playerState.Value = TankStateManager.PlayerState.Dead;
            ChangeState(tank.StateFactory.Dead());
        }
    }
}
