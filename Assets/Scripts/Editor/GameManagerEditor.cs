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
    private Label redScoreLabel;
    private Label blueScoreLabel;
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
        CreateScoreSection();

        CreateTeamsSection();
        CreateDebugActions();

        root.schedule.Execute(UpdateUIState).Every(500);

        return root;
    }
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

    private void CreateScoreSection()
    {
        var scoreBox = new Box();
        scoreBox.style.paddingTop = 10;
        scoreBox.style.paddingBottom = 10;
        scoreBox.style.marginTop = 10;
        scoreBox.style.backgroundColor = new StyleColor(new Color(0.15f, 0.15f, 0.15f));

        var titleLabel = new Label("TEAM SCORES");
        titleLabel.style.fontSize = 14;
        titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        titleLabel.style.alignSelf = Align.Center;
        titleLabel.style.marginBottom = 10;
        titleLabel.style.color = Color.yellow;
        scoreBox.Add(titleLabel);

        var scoresContainer = new VisualElement();
        scoresContainer.style.flexDirection = FlexDirection.Row;
        scoresContainer.style.justifyContent = Justify.SpaceAround;

        // Red Team Score
        var redScoreContainer = new VisualElement();
        redScoreContainer.style.alignItems = Align.Center;
        redScoreContainer.style.paddingLeft = 10;
        redScoreContainer.style.paddingRight = 10;

        var redLabel = new Label("RED TEAM");
        redLabel.style.color = new Color(1f, 0.3f, 0.3f);
        redLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        redLabel.style.fontSize = 12;
        redScoreContainer.Add(redLabel);

        redScoreLabel = new Label("0");
        redScoreLabel.style.fontSize = 32;
        redScoreLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        redScoreLabel.style.color = Color.red;
        redScoreContainer.Add(redScoreLabel);

        // Blue Team Score
        var blueScoreContainer = new VisualElement();
        blueScoreContainer.style.alignItems = Align.Center;
        blueScoreContainer.style.paddingLeft = 10;
        blueScoreContainer.style.paddingRight = 10;

        var blueLabel = new Label("BLUE TEAM");
        blueLabel.style.color = new Color(0.3f, 0.6f, 1f);
        blueLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        blueLabel.style.fontSize = 12;
        blueScoreContainer.Add(blueLabel);

        blueScoreLabel = new Label("0");
        blueScoreLabel.style.fontSize = 32;
        blueScoreLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        blueScoreLabel.style.color = Color.cyan;
        blueScoreContainer.Add(blueScoreLabel);

        scoresContainer.Add(redScoreContainer);
        scoresContainer.Add(blueScoreContainer);
        scoreBox.Add(scoresContainer);

        root.Add(scoreBox);
    }

    private void CreateTeamsSection()
    {
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

        UpdateGameStateLabel();

        UpdateScoreLabels();

        // Il bottone appare SOLO se sei il Server E lo stato è WaitingForPlayers
        bool canStart = NetworkManager.Singleton.IsServer && 
                        gameManager.CurrentGameState.Value == GameManager.GameState.WaitingForPlayers;
        
        if (startMatchButton != null)
        {
            startMatchButton.SetEnabled(canStart);
            startMatchButton.style.display = canStart ? DisplayStyle.Flex : DisplayStyle.None;
            startMatchButton.text = canStart ? "START MATCH" : "Waiting...";
        }

    }

    private void UpdateGameStateLabel()
    {
        var state = gameManager.CurrentGameState.Value;
        gameStateLabel.text = $"STATE: {state.ToString().ToUpper()}";
    }

    private void UpdateScoreLabels()
    {
        if (redScoreLabel != null && blueScoreLabel != null)
        {
            redScoreLabel.text = gameManager.RedTeamScore.Value.ToString();
            blueScoreLabel.text = gameManager.BlueTeamScore.Value.ToString();
        }
    }
}