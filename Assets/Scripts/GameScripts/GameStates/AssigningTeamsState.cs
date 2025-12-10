using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class AssigningTeamsState : GameStateBase
{
    public AssigningTeamsState(GameManager manager, GameStateFactory factory) : base(manager, factory)
    {
        
    }

    public override async void Enter()
    {
        Debug.Log("Entered Assigning Teams State");
        await AssignTeams();
    }

    public override void Exit()
    {
        Debug.Log("Exited Assigning Teams State");

    }

    public async Task AssignTeams()
    {
        if (!gameManager.GetIsServer) return;
        TeamManager.Instance.InitializeTeams();

        await Task.Delay(2000);

        Debug.Log("Teams Assigned. Transitioning to Intro State.");
        gameManager.CurrentGameState.Value = GameManager.GameState.SpawningPlayers;
    }

    
    
}
