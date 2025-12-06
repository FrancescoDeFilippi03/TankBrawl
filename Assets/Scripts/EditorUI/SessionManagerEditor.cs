using Unity.Services.Multiplayer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(SessionManager))]
public class SessionManagerEditor : Editor
{
    private SessionManager manager;
    private VisualElement root;
    
    // UI References
    private TextField joinCodeInput;
    private Button hostButton;
    private Button joinButton;
    private Button startGameButton;
    private Button leaveButton;
    private Label statusLabel;
    private Label sessionCodeLabel; // To display the code when hosting
    
    // Player List Container
    private VisualElement playerListContainer;

    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        manager = (SessionManager)target;

        // Styling
        root.style.marginTop = 10;
        root.style.marginBottom = 10;

        // 1. Status Section
        CreateStatusSection();

        // 2. Session Info (Code & Host Status)
        CreateSessionInfoSection();

        // 3. Actions (Host/Join)
        CreateActionSection();

        // 4. Player List
        CreatePlayerListSection();
        
        // 5. Footer (Leave)
        CreateFooter();

        // Schedule updates (500ms is a good balance for Editor UI)
        root.schedule.Execute(UpdateUIState).Every(500);

        return root;
    }

    private void CreateStatusSection()
    {
        var box = new Box();
        box.style.paddingTop = 5;
        box.style.paddingBottom = 5;
        box.style.marginBottom = 5;
        box.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 0.5f));

        var title = new Label("Multiplayer Session Manager");
        title.style.fontSize = 14;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.alignSelf = Align.Center;

        statusLabel = new Label("Status: Not Initialized");
        statusLabel.style.alignSelf = Align.Center;
        statusLabel.style.marginTop = 5;

        box.Add(title);
        box.Add(statusLabel);
        root.Add(box);

        // Warning for Play Mode
        var playModeWarning = new HelpBox("Enter PLAY MODE to use Unity Services.", HelpBoxMessageType.Warning);
        playModeWarning.SetEnabled(!Application.isPlaying);
        playModeWarning.style.display = Application.isPlaying ? DisplayStyle.None : DisplayStyle.Flex; 
        root.Add(playModeWarning);
    }

    private void CreateSessionInfoSection()
    {
        // This section only shows up when we are in a session
        var container = new GroupBox();
        container.name = "SessionInfo";
        container.style.display = DisplayStyle.None; // Hide by default
        container.style.marginTop = 5;

        sessionCodeLabel = new Label("Join Code: ---");
        sessionCodeLabel.style.fontSize = 12;
        sessionCodeLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        sessionCodeLabel.style.color = new StyleColor(new Color(1f, 0.8f, 0.4f)); // Gold color
        sessionCodeLabel.style.alignSelf = Align.Center;

        var copyButton = new Button(() => 
        {
            if (manager.CurrentSession != null)
            {
                GUIUtility.systemCopyBuffer = manager.CurrentSession.Code;
                Debug.Log("Join Code copied to clipboard!");
            }
        }) { text = "Copy Code" };

        container.Add(sessionCodeLabel);
        container.Add(copyButton);
        root.Add(container);
    }

    private void CreateActionSection()
    {
        // Host Button
        hostButton = new Button(async () => { await manager.StartSessionAsHost(); });
        hostButton.text = "Start Session (Host)";
        hostButton.style.height = 30;
        root.Add(hostButton);

        // Join Section
        var joinBox = new VisualElement();
        joinBox.style.marginTop = 10;
        joinBox.style.flexDirection = FlexDirection.Row;

        joinCodeInput = new TextField();
        joinCodeInput.label = "Code:";
        joinCodeInput.style.flexGrow = 1;
        
        joinButton = new Button(async () => 
        {
            if (!string.IsNullOrEmpty(joinCodeInput.value)) 
                await manager.JoinSessionAsClient(joinCodeInput.value);
        }) { text = "Join" };
        joinButton.style.width = 60;

        joinBox.Add(joinCodeInput);
        joinBox.Add(joinButton);
        root.Add(joinBox);

        // Start Game Button
        startGameButton = new Button(async () => { await manager.StartGame(); });
        startGameButton.text = "Start Game";
        startGameButton.style.height = 30;
        startGameButton.style.marginTop = 10;
        startGameButton.style.backgroundColor = new StyleColor(new Color(0.2f, 0.6f, 0.2f));
        root.Add(startGameButton);
    }

    private void CreatePlayerListSection()
    {
        var foldout = new Foldout();
        foldout.text = "Player Properties (Live)";
        foldout.value = true;
        foldout.style.marginTop = 15;

        var bg = new Box();
        bg.style.backgroundColor = new StyleColor(new Color(0.15f, 0.15f, 0.15f));
        bg.style.minHeight = 50;
        
        playerListContainer = new VisualElement();
        playerListContainer.style.paddingLeft = 5;
        playerListContainer.style.paddingTop = 5;
        
        bg.Add(playerListContainer);
        foldout.Add(bg);
        root.Add(foldout);
    }

    private void CreateFooter()
    {
        leaveButton = new Button(async () => { await manager.LeaveSession(); });
        leaveButton.text = "Leave Session";
        leaveButton.style.marginTop = 15;
        leaveButton.style.backgroundColor = new StyleColor(new Color(0.6f, 0.2f, 0.2f));
        root.Add(leaveButton);
    }

    // This runs every 500ms
    private void UpdateUIState()
    {
        if (!Application.isPlaying) return;

        bool inSession = manager.CurrentSession != null;
        bool isInitialized = true; // SessionManager is always initialized in this context

        // 1. Update Status Text
        if (inSession)
        {
            string type = manager.CurrentSession.IsHost ? "HOST" : "CLIENT";
            statusLabel.text = $"Status: Connected as {type}";
            
            // Show Session Info Box
            root.Q("SessionInfo").style.display = DisplayStyle.Flex;
            sessionCodeLabel.text = $"Join Code: {manager.CurrentSession.Code}";
        }
        else
        {
            statusLabel.text = isInitialized ? "Status: Ready" : "Status: Initializing...";
            root.Q("SessionInfo").style.display = DisplayStyle.None;
        }

        // 2. Toggle Buttons
        hostButton.SetEnabled(!inSession && isInitialized);
        joinButton.SetEnabled(!inSession && isInitialized);
        joinCodeInput.SetEnabled(!inSession && isInitialized);
        startGameButton.SetEnabled(inSession && manager.CurrentSession.IsHost);
        leaveButton.SetEnabled(inSession);

        // 3. Refresh Player List (CRITICAL FIX: Uses ISession only)
        RefreshPlayerList();
    }

    private void RefreshPlayerList()
    {
        if (playerListContainer == null) return;
        playerListContainer.Clear();

        if (manager.CurrentSession == null)
        {
            playerListContainer.Add(new Label("No active session."));
            return;
        }

        // Iterate safely over the generic Players list (works for Host AND Client)
        foreach (var player in manager.CurrentSession.Players)
        {
            var pBox = new Box();
            pBox.style.marginBottom = 5;
            pBox.style.borderBottomColor = new StyleColor(Color.gray);
            pBox.style.borderBottomWidth = 1;

            string role = (manager.CurrentSession.IsHost && manager.CurrentSession.CurrentPlayer.Id == player.Id) ? "[HOST]" : "";
            string me = (manager.CurrentSession.CurrentPlayer.Id == player.Id) ? "(YOU)" : "";
            
            var lbl = new Label($"ID: {player.Id.Substring(0, 8)}... {role} {me}");
            lbl.style.unityFontStyleAndWeight = FontStyle.Bold;
            pBox.Add(lbl);

            // Display Properties (Team, etc.)
            if (player.Properties != null)
            {
                foreach (var prop in player.Properties)
                {
                    var propLbl = new Label($"   • {prop.Key}: {prop.Value.Value}");
                    propLbl.style.color = new StyleColor(new Color(0.7f, 1f, 0.7f));
                    pBox.Add(propLbl);
                }
            }

            playerListContainer.Add(pBox);
        }
    }
}