using UnityEngine;

public class InGameState : GameStateBase
{
    private readonly float gameDuration = 180f;
    private float elapsedTime = 0f;
    private readonly float scorePerSecond = 5f;
    private float redScoreAccumulator = 0f;
    private float blueScoreAccumulator = 0f;
    private readonly float pointToWin = 100f;
    
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

        CheckAreaControl();
        CheckWinCondition();
    }


    void CheckAreaControl()
    {
        TeamColor controllingTeam = gameManager.controllingTeam.Value;
        
        if (controllingTeam == TeamColor.Red)
        {
            redScoreAccumulator += scorePerSecond * Time.deltaTime;
            if (redScoreAccumulator >= scorePerSecond)
            {
                int pointsToAdd = Mathf.FloorToInt(redScoreAccumulator);
                gameManager.RedTeamScore.Value += pointsToAdd;
                redScoreAccumulator -= pointsToAdd;
            }
        }
        else if (controllingTeam == TeamColor.Blue)
        {
            blueScoreAccumulator += scorePerSecond * Time.deltaTime;
            if (blueScoreAccumulator >= scorePerSecond)
            {
                int pointsToAdd = Mathf.FloorToInt(blueScoreAccumulator);
                gameManager.BlueTeamScore.Value += pointsToAdd;
                blueScoreAccumulator -= pointsToAdd;
            }
        }
    }


    void CheckWinCondition()
    {        
        if (gameManager.RedTeamScore.Value >= pointToWin)
        {
            gameManager.CurrentGameState.Value = GameManager.GameState.GameOver;
        }
        else if (gameManager.BlueTeamScore.Value >= pointToWin)
        {
            gameManager.CurrentGameState.Value = GameManager.GameState.GameOver;
        }
    }
}
