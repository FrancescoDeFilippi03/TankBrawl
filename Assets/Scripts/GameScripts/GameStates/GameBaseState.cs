using UnityEngine;

public abstract class GameStateBase
{
    protected GameManager gameManager;
    protected GameStateFactory stateFactory;
    
    public GameStateBase(GameManager gameManager, GameStateFactory stateFactory)
    {
        this.stateFactory = stateFactory;
        this.gameManager = gameManager;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }


    public void ChangeState(GameStateBase newState)
    {
        Exit();
        
        gameManager.CurrentState = newState;
        
        gameManager.CurrentState.Enter();
    }
}
