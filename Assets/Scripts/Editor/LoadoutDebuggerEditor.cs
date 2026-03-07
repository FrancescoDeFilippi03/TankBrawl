using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(LoadoutDebugger))]
public class LoadoutDebuggerEditor : Editor
{
    private LoadoutDebugger debugger;
    private VisualElement root;
    private Label statusLabel;

    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        debugger = (LoadoutDebugger)target;

        // --- Header Section ---
        CreateHeader();

        // --- Default Inspector (Fields) ---
        // Visualizza i campi pubblici (BaseId, TurretId, etc.) automaticamente
        var inspectorParams = new VisualElement();
        inspectorParams.style.marginTop = 10;
        inspectorParams.style.marginBottom = 10;
        InspectorElement.FillDefaultInspector(inspectorParams, serializedObject, this);
        root.Add(inspectorParams);

        // --- Actions Section ---
        CreateActionsSection();

        // --- File Info Section ---
        CreateFileInfoSection();

        return root;
    }

    private void CreateHeader()
    {
        var box = new Box();
        box.style.paddingTop = 10;
        box.style.paddingBottom = 10;
        box.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f));
        box.style.borderBottomColor = Color.gray;
        box.style.borderBottomWidth = 1;

        var title = new Label("LOADOUT MANAGER");
        title.style.fontSize = 14;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.alignSelf = Align.Center;
        title.style.color = new StyleColor(new Color(1f, 0.8f, 0.4f)); // Oro/Arancione

        box.Add(title);
        root.Add(box);
    }

    private void CreateActionsSection()
    {
        var container = new GroupBox();
        container.style.backgroundColor = new StyleColor(new Color(0.15f, 0.15f, 0.15f));
        container.style.marginTop = 5;

        var label = new Label("Disk Operations");
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        label.style.marginBottom = 5;
        container.Add(label);

        // Save Button
        var saveBtn = new Button(() =>
        {
            debugger.SaveToDisk();
            EditorWindow.focusedWindow?.ShowNotification(new GUIContent("Loadout Saved!"));
            UpdateStatus("Last Action: Saved to JSON");
        })
        { text = "SAVE Loadout to Disk" };
        
        saveBtn.style.height = 30;
        saveBtn.style.backgroundColor = new StyleColor(new Color(0.2f, 0.6f, 0.2f)); // Verde
        saveBtn.style.marginBottom = 5;
        container.Add(saveBtn);

        // Load Button
        var loadBtn = new Button(() =>
        {
            Undo.RecordObject(debugger, "Load Loadout"); // Permette il CTRL+Z
            debugger.LoadFromDisk();
            serializedObject.Update(); // Aggiorna l'editor con i nuovi valori
            UpdateStatus("Last Action: Loaded from JSON");
        })
        { text = "LOAD Loadout from Disk" };
        
        loadBtn.style.height = 30;
        loadBtn.style.backgroundColor = new StyleColor(new Color(0.2f, 0.4f, 0.7f)); // Blu
        container.Add(loadBtn);

        root.Add(container);
    }

    private void CreateFileInfoSection()
    {
        var box = new Box();
        box.style.marginTop = 10;
        box.style.paddingTop = 5;
        box.style.paddingLeft = 5;
        
        // Path Label
        var pathLabel = new Label($"Path: {Application.persistentDataPath}/user_loadout.json");
        pathLabel.style.fontSize = 10;
        pathLabel.style.whiteSpace = WhiteSpace.Normal; // Permette a capo se il percorso è lungo
        pathLabel.style.color = Color.gray;
        box.Add(pathLabel);

        // Open Folder Button
        var openFolderBtn = new Button(() =>
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath + "/user_loadout.json");
        })
        { text = "Open File Location" };
        openFolderBtn.style.marginTop = 5;
        box.Add(openFolderBtn);

        // Status Feedback
        statusLabel = new Label("Ready");
        statusLabel.style.marginTop = 5;
        statusLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
        statusLabel.style.color = Color.white;
        statusLabel.style.alignSelf = Align.Center;
        box.Add(statusLabel);

        root.Add(box);
    }

    private void UpdateStatus(string message)
    {
        if (statusLabel != null)
        {
            statusLabel.text = message;
        }
    }
}