using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : UI
{
    Button startButton;
    protected override void OnEnable()
    {
        base.OnEnable();

        startButton = root.Q<Button>("StartBTN");        

        startButton.clicked += OnStartButtonClicked;
    }
    private void OnStartButtonClicked()
    {
        Hide();

        LoaderUI.Instance.StartCoroutine(LoaderUI.Instance.LoadScreenScene("Lobby"));
    }
}