using UnityEngine;

public class IntroState : GameStateBase
{
    private float elapsedTime = 0f;
    private readonly float introDuration = 5f;
    public IntroState(GameManager manager, GameStateFactory factory) : base(manager, factory)
    {
    }

    public override void Enter()
    {
        elapsedTime = 0f;

        

        if (gameManager.GetIsServer)
        {
            gameManager.introTimer.Value = introDuration;
        }
        

        IntroUI.Instance.Show();
        gameManager.introTimer.OnValueChanged += IntroUI.Instance.UpdateTimer;
        
    }

    public override void Exit()
    {
        IntroUI.Instance.Hide();
        gameManager.introTimer.OnValueChanged -= IntroUI.Instance.UpdateTimer;
    }

    public override void Update()
    {
        if (!gameManager.GetIsServer) return;
        elapsedTime += Time.deltaTime;
        gameManager.introTimer.Value = Mathf.Max(0f, introDuration - elapsedTime);

        if (elapsedTime >= introDuration)
            gameManager.CurrentGameState.Value = GameManager.GameState.InGame;

    }
}
