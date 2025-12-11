using UnityEngine;

public class SpawningPlayersState : GameStateBase
{
    public SpawningPlayersState(GameManager manager, GameStateFactory factory) : base(manager, factory)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entered Spawning Players State");

        if (!gameManager.GetIsServer) return;
        SpawnManager.Instance.SpawnAllTanks();  

        Debug.Log("Players Spawned. Transitioning to Intro State.");
        gameManager.CurrentGameState.Value = GameManager.GameState.Intro;
        
    }

    public override void Exit()
    {
        Debug.Log("Exited Spawning Players State");
    }

    public override void Update()
    {
        // Update logic here
    }
}