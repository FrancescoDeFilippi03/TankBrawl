using System;
using UnityEngine;
using UnityEngine.UIElements;

public class HangarUI : UI
{
    public static HangarUI Instance { get; private set; }
    
    /*  
        private VisualElement lobbyModal;
        private VisualElement modalContent;
        
        private Button startGameButton;
        private Button hostGameButton;
        private Button joinLobbyButton;
        private Button closeModalButton; 
        private TextField modalPlayerNameField;
        private TextField lobbyCodeField;
    */

    //HANGAR
    private Button backMainMenu;

    //MODAL
    private VisualElement tankSelectionModal; 
    private VisualElement tankModalContent;
    private Button changeTankButton;
    private Button closeTankModalButton;
    private Button[] tankButtons = new Button[16];
    private VisualElement[] tankVisuals = new VisualElement[16];
    
    private VisualElement tankVisualContainer;
    
    private int selectedTankIndex = 0;
    
    // Events
    /* public event Action<string> OnHostGame;
    public event Action<string, string> OnJoinLobby; */
    public event Action<int> OnTankSelected;
    
    [SerializeField] private Sprite[] trackSpritesA = new Sprite[8]; // Track_1_A fino a Track_8_A
    [SerializeField] private Sprite[] trackSpritesB = new Sprite[8]; // Track_1_B fino a Track_8_B

    [SerializeField] private Texture2D[] burstFrames = new Texture2D[5]; // 5 frame per animazione burst

    private IVisualElementScheduledItem trackAnimationScheduler;
    private IVisualElementScheduledItem burstAnimationScheduler;
    private bool isAnimatingTracks = false;

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
        
        Show();

        if (root == null) return;
        
        /* lobbyModal = root.Q<VisualElement>("LobbyModal");
        modalContent = root.Q<VisualElement>("ModalContent");
        startGameButton = root.Q<Button>("StartGameButton");
        hostGameButton = root.Q<Button>("HostGameButton");
        joinLobbyButton = root.Q<Button>("JoinLobbyButton");
        closeModalButton = root.Q<Button>("CloseModalButton");
        modalPlayerNameField = root.Q<TextField>("ModalPlayerNameField");
        lobbyCodeField = root.Q<TextField>("LobbyCodeField"); */

        backMainMenu = root.Q<Button>("BackGameButton");
        
        tankSelectionModal = root.Q<VisualElement>("TankSelectionModal");
        tankModalContent = root.Q<VisualElement>("TankModalContent");
        changeTankButton = root.Q<Button>("ChangeTankButton");
        closeTankModalButton = root.Q<Button>("CloseTankModalButton");
        

        for (int i = 0; i < 16; i++)
        {
            tankButtons[i] = root.Q<Button>($"Tank{i + 1}Button");
        }

        tankVisualContainer = root.Q<VisualElement>("TankPartsContainer");
        
        for (int i = 0; i < 16; i++)
        {
            tankVisuals[i] = tankVisualContainer.Q<VisualElement>($"MainTankVisual{i + 1}");
        }
        
        if(PlayerDataManager.Instance != null){
            selectedTankIndex = PlayerDataManager.Instance.SelectedTankIndex;
            
        }

        LoadTankVisual(selectedTankIndex);

        /* 
        if (startGameButton != null)
            startGameButton.clicked += ShowLobbyModal;
        
        if (hostGameButton != null)
            hostGameButton.clicked += OnHostGameButtonClicked;
        
        if (joinLobbyButton != null)
            joinLobbyButton.clicked += OnJoinLobbyButtonClicked;
        
        if (closeModalButton != null)
            closeModalButton.clicked += HideLobbyModal;
         */


        if (changeTankButton != null)
            changeTankButton.clicked += ShowTankSelectionModal;
        
        if (closeTankModalButton != null)
            closeTankModalButton.clicked += HideTankSelectionModal;
            
        if (backMainMenu != null)
            backMainMenu.clicked += OnBackMainMenuClicked;
       
        
        for (int i = 0; i < tankButtons.Length; i++)
        {
            if (tankButtons[i] != null)
            {
                int tankIndex = i;
                tankButtons[i].clicked += () => SelectTank(tankIndex);
            }
        }

        changeTankButton.RegisterCallback<MouseEnterEvent>((evt) => OnTankButtonHover(selectedTankIndex, true));
        changeTankButton.RegisterCallback<MouseLeaveEvent>((evt) => OnTankButtonHover(selectedTankIndex, false));
        
        UpdateTankButtonHighlight();
    }
    
    void OnDisable()
    {
        /* if (startGameButton != null)
            startGameButton.clicked -= ShowLobbyModal;
        
        if (hostGameButton != null)
            hostGameButton.clicked -= OnHostGameButtonClicked;
        
        if (joinLobbyButton != null)
            joinLobbyButton.clicked -= OnJoinLobbyButtonClicked;
        
        if (closeModalButton != null)
            closeModalButton.clicked -= HideLobbyModal; */
        
        if (changeTankButton != null)
            changeTankButton.clicked -= ShowTankSelectionModal;
        
        if (closeTankModalButton != null)
            closeTankModalButton.clicked -= HideTankSelectionModal;

        if (backMainMenu != null)
            backMainMenu.clicked -= OnBackMainMenuClicked;
       
        
        for (int i = 0; i < tankButtons.Length; i++)
        {
            if (tankButtons[i] != null)
            {
                int tankIndex = i;
                tankButtons[i].clicked -= () => SelectTank(tankIndex);
            }
        }
    }

    private void OnBackMainMenuClicked()
    {
        this.Hide();
        if(LoaderUI.Instance != null){
            LoaderUI.Instance.LoadScreenScene("MainMenu");
        }
    }

    /* 

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
    } */


    private void ShowTankSelectionModal()
    {
        if (tankSelectionModal == null) return;
        
        tankSelectionModal.style.display = DisplayStyle.Flex;
        
        if (tankModalContent != null)
        {
            tankModalContent.style.scale = new Scale(new Vector3(0.7f, 0.7f, 1f));
            tankModalContent.style.opacity = 0f;
            
            tankModalContent.schedule.Execute(() =>
            {
                tankModalContent.style.scale = new Scale(Vector3.one);
                tankModalContent.style.opacity = 1f;
            }).StartingIn(10);
        }
    }
    
    private void HideTankSelectionModal()
    {
        if (tankSelectionModal == null) return;
        

        if (tankModalContent != null)
        {
            tankModalContent.style.scale = new Scale(new Vector3(0.7f, 0.7f, 1f));
            tankModalContent.style.opacity = 0f;
            
            tankSelectionModal.schedule.Execute(() =>
            {
                tankSelectionModal.style.display = DisplayStyle.None;
            }).StartingIn(300);
        }
        else
        {
            tankSelectionModal.style.display = DisplayStyle.None;
        }
    }
    
    private void SelectTank(int tankIndex)
    {
        selectedTankIndex = tankIndex;
        UpdateTankButtonHighlight();
        HideTankSelectionModal();
        OnTankSelected?.Invoke(tankIndex);

        LoadTankVisual(tankIndex);
        
        Debug.Log($"Tank {tankIndex + 1} selected!");
        

        if(PlayerDataManager.Instance != null){
            PlayerDataManager.Instance.SelectedTankIndex = selectedTankIndex;
        }

    }
    
    private void LoadTankVisual(int tankIndex)
    {
        for (int i = 0; i < tankVisuals.Length; i++)
        {
            if (tankVisuals[i] != null)
            {
                tankVisuals[i].style.display = DisplayStyle.None;
            }
        }
        
        if (tankVisuals[tankIndex] != null)
        {
            tankVisuals[tankIndex].style.display = DisplayStyle.Flex;
        }
    }
    
    private void OnTankButtonHover(int tankIndex, bool isHovering)
    {
        var mainTankVisual = tankVisuals[tankIndex];
        if (mainTankVisual == null) return;
        
        var trackLeft = mainTankVisual.Q<VisualElement>("TrackLeft");
        var trackRight = mainTankVisual.Q<VisualElement>("TrackRight");

        var burstLeft = trackLeft.Q<VisualElement>("BurstLeft");
        var burstRight = trackRight.Q<VisualElement>("BurstRight");

        if (trackLeft == null || trackRight == null) return;
        
        int trackNumber = tankIndex / 2;
        
        if (isHovering)
        {
            trackAnimationScheduler?.Pause();
            burstAnimationScheduler?.Pause();
            
            isAnimatingTracks = true;
            bool useFrameB = false;
            int burstFrameIndex = 0;
            
            trackAnimationScheduler = mainTankVisual.schedule.Execute(() =>
            {
                if (!isAnimatingTracks) return;
                
                Sprite trackSprite = useFrameB ? trackSpritesB[trackNumber] : trackSpritesA[trackNumber];
                useFrameB = !useFrameB;
                
                if (trackSprite != null)
                {
                    trackLeft.style.backgroundImage = new StyleBackground(trackSprite);
                    trackRight.style.backgroundImage = new StyleBackground(trackSprite);
                }
            }).Every(200);
            
            if (burstLeft != null && burstRight != null)
            {
                Debug.Log("Burst Animation");
                burstLeft.style.display = DisplayStyle.Flex;
                burstRight.style.display = DisplayStyle.Flex;
                
                if (burstFrames != null && burstFrames.Length > 0)
                {
                    burstAnimationScheduler = mainTankVisual.schedule.Execute(() =>
                    {
                        if (!isAnimatingTracks) return;
                        
                        if (burstFrames[burstFrameIndex] != null)
                        {
                            burstLeft.style.backgroundImage = new StyleBackground(burstFrames[burstFrameIndex]);
                            burstRight.style.backgroundImage = new StyleBackground(burstFrames[burstFrameIndex]);
                        }
                        
                        burstFrameIndex = (burstFrameIndex + 1) % burstFrames.Length;
                    }).Every(80);
                }
            }
        }
        else
        {
            isAnimatingTracks = false;
            trackAnimationScheduler?.Pause();
            burstAnimationScheduler?.Pause();
            
            Sprite trackSprite = trackSpritesA[trackNumber];
            if (trackSprite != null)
            {
                trackLeft.style.backgroundImage = new StyleBackground(trackSprite);
                trackRight.style.backgroundImage = new StyleBackground(trackSprite);
            }

            if (burstLeft != null && burstRight != null)
            {
                burstLeft.style.display = DisplayStyle.None;
                burstRight.style.display = DisplayStyle.None;
            }
        }
    }
    
    
    private void UpdateTankButtonHighlight()
    {
        for (int i = 0; i < tankButtons.Length; i++)
        {
            if (tankButtons[i] != null)
            {
                if (i == selectedTankIndex)
                {
                    tankButtons[i].AddToClassList("selected");
                }
                else
                {
                    tankButtons[i].RemoveFromClassList("selected");
                }
            }
        }
    }
    
    /* 
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
    } */
    
    public void UpdateTankStats(int health, int speed, int damage, int armor)
    {
        if (root == null) return;
        
        root.Q<Label>("HealthValue").text = health.ToString();
        root.Q<Label>("SpeedValue").text = speed.ToString();
        root.Q<Label>("DamageValue").text = damage.ToString();
        root.Q<Label>("ArmorValue").text = armor.ToString();
    }
    
    public int GetSelectedTankIndex()
    {
        return selectedTankIndex;
    }
}
