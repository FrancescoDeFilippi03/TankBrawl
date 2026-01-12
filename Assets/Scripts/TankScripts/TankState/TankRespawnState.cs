using UnityEngine;

public class TankRespawnState : TankBaseState
{
    private readonly float respawnDuration = 3f;
    private float localTimer;
    
    public TankRespawnState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Respawn State");
        localTimer = respawnDuration;
        
        if (IsOwner)
        {
            var configData = TeamManager.Instance.GetTankConfigDataForClient(tank.OwnerClientId);
            Transform spawnPoint = SpawnManager.Instance.GetSpawnPointForTeam(configData.Team, tank.OwnerClientId);
            
            tank.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            PlayerController.ResetPlayer();
            
            // Reset health immediately on respawn
            Tank.ResetHealth();
        }

        if (tank.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.simulated = true;
        }


        Tank.SetAlpha(0f);
    }

    public override void Update()
    {
        localTimer -= Time.deltaTime;
        if (IsOwner && localTimer <= 0)
        {
            Debug.Log("Respawn complete, transitioning to Idle State");
            tank.playerState.Value = TankStateManager.PlayerState.Idle;
            ChangeState(tank.StateFactory.Idle());
        }
        
        if (localTimer > 0)
        {
            float fadeProgress = 1f - (localTimer / respawnDuration);
            float alpha = Mathf.Clamp01(fadeProgress);
            Tank.SetAlpha(alpha);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Respawn State");
        Tank.SetAlpha(1f);
        
        if (IsOwner)
        {
            PlayerController.SetInputActive(true);
        }
    }
}
