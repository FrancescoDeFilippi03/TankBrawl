using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Lobby"; 

    void Start()
    {
        // Carica la scena successiva.
        // I manager (NetworkManager, SessionManager) rimarranno vivi 
        // perché sono impostati come DontDestroyOnLoad.
        SceneManager.LoadScene(nextSceneName);
    }
}