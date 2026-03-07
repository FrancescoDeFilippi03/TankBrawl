using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UIElements;
public class LobbyUI : UI
{
    Label lobbyCode;
    Button startGameButton;

    List<VisualElement> playerContainers = new List<VisualElement>(6);

    protected override void OnEnable()
    {
        base.OnEnable();

        lobbyCode = root.Q<Label>("LobbyCode");
        startGameButton = root.Q<Button>("StartGameButton");

        if(SessionManager.Instance != null && SessionManager.Instance.CurrentSession != null)
        {
            lobbyCode.text = SessionManager.Instance.CurrentSession.Code;
            startGameButton.style.display = SessionManager.Instance.CurrentSession.IsHost ? DisplayStyle.Flex : DisplayStyle.None;          
        }

        for (int i = 0; i < 6; i++)
        {
            string containerName = i % 2 == 0 ? "RedPlayer" + (i / 2 + 1) : "BluePlayer" + (i / 2 + 1);
            Debug.Log($"Looking for container: {containerName}");
            var container = root.Q<VisualElement>(containerName);
            playerContainers.Add(container);
        }

        
        for (int i = 0; i < 6; i++)
        {
           playerContainers[i].Q<Label>("NameLabel").text = TeamManager.Instance.tankConfigs[i].PlayerName.ToString();
        }

    }

    void Start()
    {

    }
}