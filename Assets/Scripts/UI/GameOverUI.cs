using UnityEngine;
using UnityEngine.UIElements;

public class GameOverUI : UI
{
    Label winnerLabel;
    Label backToLobbyTimerLabel;
    public static GameOverUI Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        winnerLabel = root.Q<Label>("WinnerLabel");
        backToLobbyTimerLabel = root.Q<Label>("BackToLobbyTimerLabel");

        Hide();
    }

    public void UpdateTimerBackToLobby(float previousTime, float remainingTime)
    {
        int seconds = Mathf.CeilToInt(remainingTime);
        backToLobbyTimerLabel.text = $"RETURNING TO LOBBY IN {seconds}...";
    }

    public void SetWinnerText(TeamColor winningTeam)
    {
        if (winningTeam == TeamColor.Red)
        {
            winnerLabel.text = "TEAM RED WINS!";
            winnerLabel.style.color = Color.red;
        }
        else if (winningTeam == TeamColor.Blue)
        {
            winnerLabel.text = "TEAM BLUE WINS!";
            winnerLabel.style.color = Color.blue;
        }
        else
        {
            winnerLabel.text = "IT'S A DRAW!";
            winnerLabel.style.color = Color.white;
        }
    }


}
