using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;
public class ExitGame : GameStateBase
{
    public ExitGame(GameManager gameManager, GameStateFactory stateFactory) : base(gameManager, stateFactory)
    {
    }


    public override void Enter()
    {
        Debug.Log("Entering ExitGame State");
        Exit();
    }

    public override async void Exit()
    {
        Debug.Log("Exiting ExitGame State");

    }

    public override void Update()
    {

    }
}