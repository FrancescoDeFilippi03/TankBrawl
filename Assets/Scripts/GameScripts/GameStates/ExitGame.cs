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
        Debug.Log("Entering ecit state");

        if (!gameManager.GetIsServer) return;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            var playerObj = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
            if (playerObj != null)
                playerObj.Despawn(true);
        }

        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public override void Exit()
    {
        Debug.Log("Exiting ExitGame State");

    }

    public override void Update()
    {

    }
}