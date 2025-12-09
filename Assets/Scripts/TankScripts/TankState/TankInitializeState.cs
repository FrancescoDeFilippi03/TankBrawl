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


        if (tank.IsOwner)
        {
            tank.playerNetworkConfigData.Value = new TankDataBuilder()
            .WithTeam(TeamManager.Instance.GetPlayerTeam(tank.OwnerClientId))
            .WithLoadout()
            .Build();

            tank.tankPlayerData.InitializeTankElements(tank.playerNetworkConfigData.Value);
            tank.tankPlayerData.UpdateTankSprites(tank.playerNetworkConfigData.Value);
        }
        



        tank.playerState.Value = TankStateManager.PlayerState.Idle;
    }

    public override void Exit()
    {
        Debug.Log("Exiting Initialize State");
    }

}
