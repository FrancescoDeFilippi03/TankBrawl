using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "MainMenu"; 

    void Start()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}