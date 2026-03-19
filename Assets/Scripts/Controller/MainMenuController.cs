using Unity.Netcode;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private MainMenuUI mainMenuUI;

    private void OnEnable()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.OnHostGame += HandleHostGame;
            mainMenuUI.OnJoinLobby += HandleJoinLobby;
        }
    }

    private void OnDisable()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.OnHostGame -= HandleHostGame;
            mainMenuUI.OnJoinLobby -= HandleJoinLobby;
        }
    }

    private async void HandleHostGame(string playerName)
    {
        PlayerDataManager.Instance.PlayerName = playerName;
        await SessionManager.Instance.StartSessionAsHost();
        //LoaderUI.Instance.LoadScreenScene("Lobby");
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby" , UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private async void HandleJoinLobby(string playerName, string lobbyCode)
    {
        PlayerDataManager.Instance.PlayerName = playerName;
        await SessionManager.Instance.JoinSessionAsClient(lobbyCode);
        //LoaderUI.Instance.LoadScreenScene("Lobby");
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby" , UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
