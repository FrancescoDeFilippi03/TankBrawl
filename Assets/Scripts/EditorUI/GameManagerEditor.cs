using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using UnityEditor.UIElements;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private GameManager gameManager;
    private TeamManager teamManager;
    private VisualElement root;

    // UI References
    private Label gameStateLabel;
    private VisualElement redTeamContainer;
    private VisualElement blueTeamContainer;
    private Button startMatchButton; // Riferimento al nuovo bottone

    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        gameManager = (GameManager)target;
        teamManager = gameManager.GetComponent<TeamManager>();

        InspectorElement.FillDefaultInspector(root, serializedObject, this);

        // Styling
        var separator = new Box();
        separator.style.height = 2;
        separator.style.backgroundColor = Color.gray;
        separator.style.marginTop = 10;
        separator.style.marginBottom = 10;
        root.Add(separator);

        CreateGameStateSection();
        
        // --- NUOVA SEZIONE: Controlli Host ---
        //CreateHostControls();
        // -------------------------------------

        CreateTeamsSection();
        CreateDebugActions();

        root.schedule.Execute(UpdateUIState).Every(500);

        return root;
    }

    /* private void CreateHostControls()
    {
        var container = new GroupBox();
        container.style.marginTop = 10;
        container.style.marginBottom = 10;
        container.style.backgroundColor = new StyleColor(new Color(0.15f, 0.15f, 0.15f));

        var label = new Label("HOST CONTROLS");
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        label.style.alignSelf = Align.Center;
        container.Add(label);

        startMatchButton = new Button(() => 
        {
            if (Application.isPlaying && gameManager.IsServer)
            {
                gameManager.StartMatch();
            }
        }) { text = "START MATCH (Assign Teams)" };
        
        startMatchButton.style.height = 35;
        startMatchButton.style.backgroundColor = new StyleColor(new Color(0.2f, 0.6f, 0.2f)); // Verde
        startMatchButton.style.marginTop = 5;
        startMatchButton.style.unityFontStyleAndWeight = FontStyle.Bold;
        
        container.Add(startMatchButton);
        root.Add(container);
    }
 */
    private void CreateGameStateSection()
    {
        // (Codice identico a prima per lo stato)
        var statusBox = new Box();
        statusBox.style.paddingTop = 10;
        statusBox.style.paddingBottom = 10;
        statusBox.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f));
        
        gameStateLabel = new Label("STATE: ---");
        gameStateLabel.style.fontSize = 16;
        gameStateLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        gameStateLabel.style.alignSelf = Align.Center;
        gameStateLabel.style.color = Color.white;

        statusBox.Add(gameStateLabel);
        root.Add(statusBox);
    }

    private void CreateTeamsSection()
    {
        // (Mantieni la tua logica di visualizzazione team qui)
        // ... (Codice identico al precedente per redTeamContainer/blueTeamContainer) ...
        // Per brevità, riporto solo la struttura base, incolla il tuo codice CreateTeamsSection precedente qui
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        redTeamContainer = new VisualElement(); 
        blueTeamContainer = new VisualElement();
        // ... Aggiungi contenitori e label come prima ...
        container.Add(redTeamContainer); // (Placeholder)
        container.Add(blueTeamContainer); // (Placeholder)
        root.Add(container);
    }

    private void CreateDebugActions()
    {
        // (Opzionale: mantieni i tuoi debug actions)
    }

    private void UpdateUIState()
    {
        if (!Application.isPlaying || gameManager == null) return;
        if (teamManager == null) teamManager = gameManager.GetComponent<TeamManager>();

        // 1. Aggiorna Label Stato
        UpdateGameStateLabel();

        // 2. GESTIONE VISIBILITÀ BOTTONE START
        // Il bottone appare SOLO se sei il Server E lo stato è WaitingForPlayers
        bool canStart = NetworkManager.Singleton.IsServer && 
                        gameManager.CurrentGameState.Value == GameManager.GameState.WaitingForPlayers;
        
        if (startMatchButton != null)
        {
            startMatchButton.SetEnabled(canStart);
            startMatchButton.style.display = canStart ? DisplayStyle.Flex : DisplayStyle.None;
            // Opzionale: cambia testo se non ci sono abbastanza player
            startMatchButton.text = canStart ? "START MATCH" : "Waiting...";
        }

        // 3. Aggiorna Liste (come prima)
        // UpdateTeamLists(); // Assicurati di includere il metodo UpdateTeamLists dal codice precedente
    }

    private void UpdateGameStateLabel()
    {
        var state = gameManager.CurrentGameState.Value;
        gameStateLabel.text = $"STATE: {state.ToString().ToUpper()}";
        // Colori...
    }
}