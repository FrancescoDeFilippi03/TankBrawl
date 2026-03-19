using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameMainUI : UI
{
    public static GameMainUI Instance { get; private set; }
    private Label redTeamScoreLabel;
    private Label blueTeamScoreLabel;
    private Label gameTimerLabel;

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

        redTeamScoreLabel = root.Q<Label>("RedTeamScoreLabel");
        blueTeamScoreLabel = root.Q<Label>("BlueTeamScoreLabel");
        gameTimerLabel = root.Q<Label>("GameTimerLabel");
    }

    public void UpdateTimer(float previousTime, float remainingTime)
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        gameTimerLabel.text = $"{minutes:00}:{seconds:00}";
    }

    public void UpdateRedTeamScore(int previousScore, int score)
    {
        redTeamScoreLabel.text = score.ToString();
    }

    public void UpdateBlueTeamScore(int previousScore, int score)
    {
        blueTeamScoreLabel.text = score.ToString();
    }

    
}
