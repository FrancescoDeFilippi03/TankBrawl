using UnityEngine;

public class InGameState : GameStateBase
{
    private float gameDuration = 180f;
    private float elapsedTime = 0f;

    public InGameState(GameManager manager, GameStateFactory factory) : base(manager, factory)
    {
    }

    public override void Update()
    {

        if (!gameManager.GetIsServer ) return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= gameDuration)
        {
            gameManager.CurrentGameState.Value = GameManager.GameState.GameOver;
        }
    }
}
