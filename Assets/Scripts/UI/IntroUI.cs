using UnityEngine;
using UnityEngine.UIElements;

public class IntroUI : UI
{
    public static IntroUI Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    Label timerLabel;

    protected override void OnEnable()
    {
        base.OnEnable();

        if(root == null) return;

        timerLabel = root.Q<Label>("StartingTimerLabel");

        Hide();
    }

    public void UpdateTimer(float previousTime, float remainingTime)
    {
        int seconds = Mathf.CeilToInt(remainingTime);
        timerLabel.text = seconds.ToString();
    }

}
