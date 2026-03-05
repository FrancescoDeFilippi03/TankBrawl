using System;
using UnityEngine;

public class InGameState : GameStateBase
{
    private float elapsedOneSecond = 0f;
    private readonly float scorePerSecond = 5f;
    private float redScoreAccumulator = 0f;
    private float blueScoreAccumulator = 0f;


    
    public InGameState(GameManager manager, GameStateFactory factory) : base(manager, factory)
    {
    }


  

    public override void Enter()
    {
        redScoreAccumulator = 0f;
        blueScoreAccumulator = 0f;

        gameManager.gameTimer.OnValueChanged += gameManager.gameMainUI.UpdateTimer;
        gameManager.RedTeamScore.OnValueChanged += gameManager.gameMainUI.UpdateRedTeamScore;
        gameManager.BlueTeamScore.OnValueChanged += gameManager.gameMainUI.UpdateBlueTeamScore;

    }

    public override void Exit()
    {
        gameManager.gameTimer.OnValueChanged -= gameManager.gameMainUI.UpdateTimer;
        gameManager.RedTeamScore.OnValueChanged -= gameManager.gameMainUI.UpdateRedTeamScore;
        gameManager.BlueTeamScore.OnValueChanged -= gameManager.gameMainUI.UpdateBlueTeamScore;
    }

    public override void Update()
    {

        if (!gameManager.GetIsServer ) return;

        if (gameManager.gameTimer.Value <= 0f)
        {
            gameManager.CurrentGameState.Value = GameManager.GameState.GameOver;
            return;
        }

        elapsedOneSecond += Time.deltaTime;
        if (elapsedOneSecond >= 1f)
        {
            elapsedOneSecond = 0f;
            gameManager.gameTimer.Value = Mathf.Max(0f, gameManager.gameTimer.Value - 1f);
        }

        CheckAreaControl();
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

}
