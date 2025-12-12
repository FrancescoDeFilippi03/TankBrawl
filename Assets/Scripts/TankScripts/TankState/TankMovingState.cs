using UnityEngine;

public class TankMovingState : TankBaseState
{
    public TankMovingState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        tank.TankAnimator.SetBool("isMoving", true);
    }

    public override void Exit()
    {
        tank.TankAnimator.SetBool("isMoving", false);
    }
    public override void Update()
    {
        CheckStateChange();
    }
    public override void FixedUpdate()
    {
        MoveTank();
    }

    public override void CheckStateChange()
    {
        if(!tank.IsOwner) return;
        
        if(tank.MovementInput.magnitude <= 0.1f)
        {
            tank.playerState.Value = TankStateManager.PlayerState.Idle;
            ChangeState(tank.StateFactory.Idle());
        }
    }

    private Vector2 currentVelocity = Vector2.zero;

    private void MoveTank()
    {
        tank.SmoothedMovementInput = Vector2.SmoothDamp(
            tank.SmoothedMovementInput,
            tank.MovementInput,
            ref currentVelocity,
            1f / tank.MovementSmoothing
        );

        if (tank.SmoothedMovementInput.magnitude > 0.01f)
        {
            Vector2 movement = tank.TankPlayerData.Speed * Time.fixedDeltaTime * tank.SmoothedMovementInput;
            tank.Rb.MovePosition(tank.Rb.position + movement);
        }

        if (tank.MovementInput.magnitude > 0.1f)
        {
            HandleRotation(tank.MovementInput);
        }
    }

    private void HandleRotation(Vector2 inputDirection)
    {
        float targetAngle = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        Quaternion currentRotation = Quaternion.Euler(0, 0, tank.Rb.rotation);

        Quaternion newRotation = Quaternion.Slerp(
            currentRotation,
            targetRotation,
            tank.RotationSmoothing * Time.fixedDeltaTime
        );

        tank.Rb.MoveRotation(newRotation.eulerAngles.z);
    }

    
}