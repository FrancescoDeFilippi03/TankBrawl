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

        GameMainUI.Instance.Hide();
        TankMainUI.Instance.Hide();

        elapsedTime = 0f;

        GameOverUI.Instance.Show();

        var winnerTeam = gameManager.RedTeamScore.Value > gameManager.BlueTeamScore.Value ? TeamColor.Red :
                         gameManager.BlueTeamScore.Value > gameManager.RedTeamScore.Value ? TeamColor.Blue 
                         : TeamColor.None;


        GameOverUI.Instance.SetWinnerText(winnerTeam);
        
        gameManager.gameOverTimer.OnValueChanged += GameOverUI.Instance.UpdateTimerBackToLobby;
    
        if (!gameManager.GetIsServer) return;
            gameManager.gameOverTimer.Value = gameOverDuration;
        
    }

    public override void Exit()
    {
        Debug.Log("Exited Game Over State");

        GameOverUI.Instance.Hide();
        gameManager.gameOverTimer.OnValueChanged -= GameOverUI.Instance.UpdateTimerBackToLobby;
        
        
    }

    public override void Update()
    {
        if (!gameManager.GetIsServer) return;
        
        elapsedTime += Time.deltaTime;
        gameManager.gameOverTimer.Value = Mathf.Max(0f, gameOverDuration - elapsedTime);    

        if (elapsedTime >= gameOverDuration)
        {
            gameManager.CurrentGameState.Value = GameManager.GameState.ExitGame;
        }
    }   
}
