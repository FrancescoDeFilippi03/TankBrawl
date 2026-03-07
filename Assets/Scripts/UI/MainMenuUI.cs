using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : UI
{

    //MODAL
    private Button startGameButton;
    private Button hostGameButton;
    private Button joinLobbyButton;
    private Button closeModalButton;

    private VisualElement lobbyModal;
    private VisualElement modalContent;
    private TextField modalPlayerNameField;
    private TextField lobbyCodeField;

    private Button hangarButton;

    // Events
    public event Action<string> OnHostGame;
    public event Action<string, string> OnJoinLobby;

    protected override void OnEnable()
    {
        base.OnEnable();

        //MODAL
        lobbyModal = root.Q<VisualElement>("LobbyModal");
        modalContent = root.Q<VisualElement>("ModalContent");
        startGameButton = root.Q<Button>("StartGameButton");
        hostGameButton = root.Q<Button>("HostGameButton");
        joinLobbyButton = root.Q<Button>("JoinLobbyButton");
        closeModalButton = root.Q<Button>("CloseModalButton");
        modalPlayerNameField = root.Q<TextField>("ModalPlayerNameField");
        lobbyCodeField = root.Q<TextField>("LobbyCodeField");

        if (startGameButton != null)
            startGameButton.clicked += ShowLobbyModal;
        
        if (hostGameButton != null)
            hostGameButton.clicked += OnHostGameButtonClicked;
        
        if (joinLobbyButton != null)
            joinLobbyButton.clicked += OnJoinLobbyButtonClicked;
        
        if (closeModalButton != null)
            closeModalButton.clicked += HideLobbyModal;


        //TEXT EVENT
        if(modalPlayerNameField != null)
        {
            modalPlayerNameField.value = PlayerDataManager.Instance.PlayerName;
            modalPlayerNameField.RegisterValueChangedCallback(OnPlayerNameChanged);
        }

        if(lobbyCodeField != null)
        {
            lobbyCodeField.value = PlayerDataManager.Instance.LobbyCodeField;
            lobbyCodeField.RegisterValueChangedCallback(OnLobbyCodeFieldChanged);
        }

        //HANGAR
        hangarButton = root.Q<Button>("HangarButton");

        if(hangarButton != null)
            hangarButton.clicked += OnHangarButtonClicked;
    }

    void OnDisable()
    {
        if (startGameButton != null)
            startGameButton.clicked -= ShowLobbyModal;
        
        if (hostGameButton != null)
            hostGameButton.clicked -= OnHostGameButtonClicked;
        
        if (joinLobbyButton != null)
            joinLobbyButton.clicked -= OnJoinLobbyButtonClicked;
        
        if (closeModalButton != null)
            closeModalButton.clicked -= HideLobbyModal;

        if(modalPlayerNameField != null)
            modalPlayerNameField.UnregisterValueChangedCallback(OnLobbyCodeFieldChanged);

        if(hangarButton != null)
            hangarButton.clicked -= OnHangarButtonClicked;
    }


    private void ShowLobbyModal()
    {
        if (lobbyModal == null) return;
        
        lobbyModal.style.display = DisplayStyle.Flex;
        
        if (modalContent != null)
        {
            modalContent.style.scale = new Scale(new Vector3(0.7f, 0.7f, 1f));
            modalContent.style.opacity = 0f;
            
            modalContent.schedule.Execute(() =>
            {
                modalContent.style.scale = new Scale(Vector3.one);
                modalContent.style.opacity = 1f;
            }).StartingIn(10);
        }
    }

    private void HideLobbyModal()
    {
        if (lobbyModal == null) return;
        
        if (modalContent != null)
        {
            modalContent.style.scale = new Scale(new Vector3(0.7f, 0.7f, 1f));
            modalContent.style.opacity = 0f;
            
            lobbyModal.schedule.Execute(() =>
            {
                lobbyModal.style.display = DisplayStyle.None;
            }).StartingIn(300);
        }
        else
        {
            lobbyModal.style.display = DisplayStyle.None;
        }
    }

    private void OnHostGameButtonClicked()
    {
        string playerName = modalPlayerNameField?.value ?? "Player";
        HideLobbyModal();
        OnHostGame?.Invoke(playerName);
    }
    
    private void OnJoinLobbyButtonClicked()
    {
        string playerName = modalPlayerNameField?.value ?? "Player";
        string lobbyCode = lobbyCodeField?.value ?? "";
        
        if (string.IsNullOrEmpty(lobbyCode))
        {
            Debug.LogWarning("Lobby code is required to join a lobby!");
            return;
        }
        
        HideLobbyModal();
        OnJoinLobby?.Invoke(playerName, lobbyCode);
    }

    private void OnHangarButtonClicked()
    {
       LoaderUI.Instance.StartCoroutine(LoaderUI.Instance.LoadScreenScene("Hangar"));
    }

    private void OnPlayerNameChanged(ChangeEvent<string> evt)
    {
        PlayerDataManager.Instance.PlayerName = evt.newValue;
    }

    private void OnLobbyCodeFieldChanged(ChangeEvent<string> evt)
    {
        PlayerDataManager.Instance.LobbyCodeField = evt.newValue;
    }
}