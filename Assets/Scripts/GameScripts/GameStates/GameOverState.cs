using UnityEngine;

public class GameOverState : GameStateBase
{
    
    public GameOverState(GameManager manager, GameStateFactory factory) : base(manager, factory)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entered Game Over State");
        
        
    }

    public override void Exit()
    {
        Debug.Log("Exited Game Over State");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }

    public override void Update()
    {
        Debug.Log("Game Over State Updating...");
    }   
}
