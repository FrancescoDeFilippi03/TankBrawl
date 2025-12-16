using UnityEngine;

public class TankDeadState : TankBaseState
{

    private float timeInDeadState = 0f;
    private float deadAnimDuration = 1.0f;
    public TankDeadState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Dead State");
        // Dead state logic here

        //tank.PlayerController.TankAnimator.SetBool("isDead", true);

        if (tank.IsOwner) tank.PlayerController.SetInputActive(false);

        if(tank.TryGetComponent<Rigidbody2D>(out var rb)) rb.simulated = false;

        tank.PlayerController.TankHealthManager.Invulnerable = true;

        timeInDeadState = 0f;
    }

    public override void Update()
    {
        if (!tank.IsOwner) return;
        
        timeInDeadState += Time.deltaTime;

        if (timeInDeadState >= deadAnimDuration)
        {
            Debug.Log("Transitioning to Respawn State");
            tank.playerState.Value = TankStateManager.PlayerState.Respawn;
            ChangeState(tank.StateFactory.Respawn());
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Dead State");

        //tank.PlayerController.TankAnimator.SetBool("isDead", false);
    }
}