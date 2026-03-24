using Unity.Netcode;
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

        if (IsOwner) tank.PlayerController.SetInputActive(false);
        
        Tank.SetInvincibleServerRpc(true);
        
        timeInDeadState = 0f;

        deadAnimDuration = Tank.deathAnimation != null ? Tank.deathAnimation.length : 1.0f;

        Tank.SetAlpha(0f);

        Tank.SpawnDeathEffectServerRpc(Tank.transform.position);
    }

    public override void Update()
    {
        if (!IsOwner) return;
        
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
    }
}