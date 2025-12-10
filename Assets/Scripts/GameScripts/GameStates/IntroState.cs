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
        Debug.Log("Entered Intro State - Starting 5 second countdown");
    }

    public override void Exit()
    {
        Debug.Log("Exited Intro State");
    }

    public override void Update()
    {
        if (!gameManager.GetIsServer) return;

        elapsedTime += Time.deltaTime;
        
        if (elapsedTime >= introDuration )
        {
            Debug.Log("Intro complete! Transitioning to InGame");
            gameManager.CurrentGameState.Value = GameManager.GameState.InGame;
        }
    }
}
