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
        
        if (tank.IsOwner)
        {
            var configData = TeamManager.Instance.GetTankConfigDataForClient(tank.OwnerClientId);
            Transform spawnPoint = SpawnManager.Instance.GetSpawnPointForTeam(configData.Team, tank.OwnerClientId);
            
            tank.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            tank.PlayerController.ResetPlayer();

            tank.RequestHealthResetServerRpc();
        }

        if (tank.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.simulated = true;
        }
        tank.PlayerController.TankVisuals.SetAlpha(0f);
    }

    public override void Update()
    {
        localTimer -= Time.deltaTime;
        if (tank.IsOwner && localTimer <= 0)
        {
            Debug.Log("Respawn complete, transitioning to Idle State");
            tank.playerState.Value = TankStateManager.PlayerState.Idle;
            ChangeState(tank.StateFactory.Idle());
        }
        
        if (localTimer > 0)
        {
            float fadeProgress = 1f - (localTimer / respawnDuration);
            float alpha = Mathf.Clamp01(fadeProgress);
            tank.PlayerController.TankVisuals.SetAlpha(alpha);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Respawn State");
        tank.PlayerController.TankVisuals.SetAlpha(1f);

        tank.PlayerController.TankHealthManager.Invulnerable = false;
        
        if (tank.IsOwner)
        {
            tank.PlayerController.SetInputActive(true); 
        }
    }
}
