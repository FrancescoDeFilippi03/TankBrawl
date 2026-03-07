using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }    
    public int SelectedTankIndex { get; set; } = 0;
    public string PlayerName { get; set; } = "Player";
    public string LobbyCodeField  { get; set; } = "";   
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

}