using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameOverState : GameStateBase
{
    private float gameOverDuration = 5f;
    private float elapsedTime = 0f;
    
    public GameOverState(GameManager manager, GameStateFactory factory) : base(manager, factory)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entered Game Over State");
        elapsedTime = 0f;
    }

    public override void Exit()
    {
        Debug.Log("Exited Game Over State");
    }

    public override void Update()
    {
        if (!gameManager.GetIsServer) return;

        elapsedTime += Time.deltaTime;
        
        if (elapsedTime >= gameOverDuration)
        {
            gameManager.CurrentGameState.Value = GameManager.GameState.ExitGame;
        }
    }   
}
