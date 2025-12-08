using UnityEngine;

public class GameStateFactory
{
    private GameManager gameManager;

    public GameStateFactory(GameManager manager)
    {
        gameManager = manager;
    }

    public WaitingState WaitingState()
    {
        return new WaitingState(gameManager , this);
    }

    public AssigningTeamsState AssigningTeamsState()
    {
        return new AssigningTeamsState(gameManager, this);
    }

    public IntroState IntroState()
    {
        return new IntroState(gameManager, this);
    }

    public InGameState InGameState()
    {
        return new InGameState(gameManager, this);
    }

    public GameOverState GameOverState()
    {
        return new GameOverState(gameManager, this);
    }


    public GameStateBase GetState(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.WaitingForPlayers:
                return WaitingState();
            case GameManager.GameState.AssigningTeams:
                return AssigningTeamsState();
            case GameManager.GameState.Intro:
                return IntroState();
            case GameManager.GameState.InGame:
                return InGameState();
            case GameManager.GameState.GameOver:
                return GameOverState();
            default:
                Debug.LogError($"No GameStateBase found for state: {state}");
                return null;
        }
    }


}
