using UnityEngine;

public class WaitingState : GameStateBase
{
    public WaitingState(GameManager manager, GameStateFactory factory) : base(manager, factory)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entered Waiting State");
    }

    public override void Exit()
    {
        Debug.Log("Exited Waiting State");
    }

    public override void Update()
    {
       // Debug.Log("Waiting for players...");
    }

}
