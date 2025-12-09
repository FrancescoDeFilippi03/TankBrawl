using UnityEngine;

public class TankInitializeState : TankBaseState
{
    public TankInitializeState(TankStateManager tankStateManager) : base(tankStateManager)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering Initialize State");
        // Initialization logic here
        
        TeamColor team = GameManager.Instance.TeamManager.GetPlayerTeam(tank.OwnerClientId);

        TankConfigData tankConfigData = new TankDataBuilder()
            .WithTeam(team)
            .WithLoadout()
            .Build();
        
        tank.playerConfigData.Value = tankConfigData;

        tank.playerState.Value = TankStateManager.PlayerState.Idle;
    }

    public override void Exit()
    {
        Debug.Log("Exiting Initialize State");
        // Cleanup logic here
    }

}
