using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TankDashingState : TankBaseState
{
    
    public TankDashingState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
    
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        base.Update();
        CheckStateChange();
    }

    public override void FixedUpdate()
    {
       base.FixedUpdate();
       Tank.Dash(PlayerController.MovementInput);   
    }

    public override void CheckStateChange()
    {
        if (!PlayerController.IsDashing)
        {
            if (PlayerController.MovementInput.magnitude > 0.1f)
            {
                tank.playerState.Value = TankStateManager.PlayerState.Moving;
                ChangeState(tank.StateFactory.Moving());
            }
            else
            {
                tank.playerState.Value = TankStateManager.PlayerState.Idle;
                ChangeState(tank.StateFactory.Idle());
            }
        }
    }
} 