using UnityEngine;
public class TankDashingState : TankBaseState
{
    float dashTime;
    float dashTimer = 0f;
    readonly AnimationCurve dashCurve = AnimationCurve.EaseInOut(0.25f, 1f, 1f, 0.25f);
    public TankDashingState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Dashing State");
        // Implement dash logic here
        tank.PlayerController.isDashing = true;
        tank.PlayerController.TankAnimator.SetBool("isDashing", true);
        dashTime = tank.PlayerController.TankMovementManager.TrackConfig.dashDuration;
    }

    public override void Update()
    {
        dashTimer += Time.deltaTime;
        if (dashTimer >= dashTime)
        {
            tank.playerState.Value = TankStateManager.PlayerState.Moving;
            ChangeState(tank.StateFactory.Moving());
        }
    }

    public override void FixedUpdate()
    {
       dashTimer += Time.fixedDeltaTime;
        float dashProgress = dashTimer / dashTime;
        if (dashProgress <= 1f)
        {
            float speedMultiplier = dashCurve.Evaluate(dashProgress);
            Vector2 dashDirection = tank.PlayerController.MovementInput.normalized;
            if (dashDirection.magnitude <= 0.01f)
            {
                float angleRad = (tank.PlayerController.Rb.rotation - 90f) * Mathf.Deg2Rad;
                dashDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            }
            // Just multiply direction by speedMultiplier
            tank.PlayerController.TankMovementManager.Dash(dashDirection * speedMultiplier, tank.PlayerController.Rb);
            tank.PlayerController.TankMovementManager.HandleRotation(dashDirection, tank.PlayerController.Rb);
        }
        else
        {
            tank.playerState.Value = TankStateManager.PlayerState.Moving;
            ChangeState(tank.StateFactory.Moving());
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Dashing State");
        // Implement logic to end dash here
        tank.PlayerController.TankAnimator.SetBool("isDashing", false);

        tank.PlayerController.isDashing = false;
    }
}