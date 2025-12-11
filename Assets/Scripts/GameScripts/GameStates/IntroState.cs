using UnityEngine;

public class IntroState : GameStateBase
{
    private float introDuration = 5f;
    private float elapsedTime = 0f;
    public IntroState(GameManager manager, GameStateFactory factory) : base(manager, factory)
    {
    }

    public override void Enter()
    {
        elapsedTime = 0f;
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if (!gameManager.GetIsServer) return;

        elapsedTime += Time.deltaTime;
        
        if (elapsedTime >= introDuration )
        {
            gameManager.CurrentGameState.Value = GameManager.GameState.InGame;
        }
    }
}
