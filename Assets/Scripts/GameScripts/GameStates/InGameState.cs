using UnityEngine;

public class InGameState : GameStateBase
{
    private float gameDuration = 10f;
    private float elapsedTime = 0f;

    public InGameState(GameManager manager, GameStateFactory factory) : base(manager, factory)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entered In-Game State");
    }

    public override void Exit()
    {
        Debug.Log("Exited In-Game State");
    }

    public override void Update()
    {
        Debug.Log("Game is in progress...");

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= gameDuration)
        {
            gameManager.CurrentGameState.Value = GameManager.GameState.GameOver;
        }
    }
}
