using System;
using System.Collections.Generic;
using Unity;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
public class LobbyUI : UI
{
    Label lobbyCode;
    Button startGameButton;
    Button quitGameButton;

    List<VisualElement> playerContainers = new List<VisualElement>(6);

    protected override void OnEnable()
    {
        base.OnEnable();

        lobbyCode = root.Q<Label>("LobbyCode");
        startGameButton = root.Q<Button>("StartGameButton");
        quitGameButton = root.Q<Button>("QuitGameButton");

        if(SessionManager.Instance != null && SessionManager.Instance.CurrentSession != null)
        {
            lobbyCode.text = SessionManager.Instance.CurrentSession.Code;
            startGameButton.style.display = SessionManager.Instance.CurrentSession.IsHost ? DisplayStyle.Flex : DisplayStyle.None;
        }
        startGameButton.clicked += OnStartGameButtonClicked;    
        quitGameButton.clicked += OnQuitGameButtonClicked;      


        for (int i = 0; i < 6; i++)
        {
            string containerName = i % 2 == 0 ? "RedPlayer" + (i / 2 + 1) : "BluePlayer" + (i / 2 + 1);
            var container = root.Q<VisualElement>(containerName);
            playerContainers.Add(container);
        }

        SessionDataManager.Instance.Players.OnListChanged += UpdateUIOnPlayersChanged;

        

        UpdateUI();
    }

    void OnDisable()
    {
        startGameButton.clicked -= OnStartGameButtonClicked;    
        quitGameButton.clicked -= OnQuitGameButtonClicked;      


        SessionDataManager.Instance.Players.OnListChanged -= UpdateUIOnPlayersChanged;


    }

    private async void OnQuitGameButtonClicked()
    {
        await SessionManager.Instance.LeaveSession();
        LoaderUI.Instance.LoadScreenScene("MainMenu");
    }

    private async void OnStartGameButtonClicked()
    {
        
        await SessionManager.Instance.StartGame();
        
    }

    private void UpdateUIOnPlayersChanged(NetworkListEvent<SessionPlayerData> changeEvent)
    {
        UpdateUI();
    }

    void SetSelectedTank(VisualElement partsContainer , int index)
    {
        partsContainer.Q<VisualElement>("TankContainer").Q<VisualElement>("TankPartsContainer").Q<VisualElement>("MainTankVisual" + (index + 1)).style.display = DisplayStyle.Flex;
    }

    void SetPlayerName(VisualElement nameLabel , string name)
    {
        nameLabel.Q<Label>("NameLabel").text = name;
    }
    void SetPlayerInfoVisible(VisualElement playerVisual)
    {
        playerVisual.style.display = DisplayStyle.Flex;
    }

    void UpdateUI()
    {
        
       for (int i = 0; i < SessionDataManager.Instance.Players.Count; i++)
        {
            SetPlayerName(playerContainers[i]  , SessionDataManager.Instance.Players[i].PlayerName.ToString());
            SetSelectedTank(playerContainers[i] , SessionDataManager.Instance.Players[i].TankId);
            SetPlayerInfoVisible(playerContainers[i]);
        }

    }


}