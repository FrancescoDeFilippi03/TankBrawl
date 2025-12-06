using UnityEditor;
using UnityEngine;

// This attribute tells Unity to use this visualizer for the TeamManager script
[CustomEditor(typeof(TeamManager))]
public class TeamManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 1. Draw the default stuff (public variables, etc.)
        DrawDefaultInspector();

        // 2. Get a reference to the actual script
        TeamManager manager = (TeamManager)target;

        GUILayout.Space(10);
        GUILayout.Label("Live Team Status", EditorStyles.boldLabel);
        GUILayout.BeginVertical("box");

        // 3. Check if the dictionary exists and has players
        if (manager.PlayerTeams != null && manager.PlayerTeams.Count > 0)
        {
            GUILayout.Label($"Total Players: {manager.PlayerTeams.Count}");
            GUILayout.Space(5);

            foreach (var player in manager.PlayerTeams)
            {
                DrawPlayerRow(player.Key, player.Value);
            }
        }
        else
        {
            GUILayout.Label("Waiting for connections...");
        }

        GUILayout.EndVertical();

        // 4. Force the inspector to redraw constantly while the game is running
        // This ensures you see updates the moment a player joins.
        if (Application.isPlaying)
        {
            Repaint();
        }
    }

    private void DrawPlayerRow(ulong clientId, int teamId)
    {
        GUILayout.BeginHorizontal();
        
        // Client ID Label
        GUILayout.Label($"Client ID: {clientId}", GUILayout.Width(100));

        // Team Color Logic
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        string teamName = "";

        if (teamId == 0)
        {
            style.normal.textColor = Color.blue;
            teamName = "TEAM A";
        }
        else
        {
            style.normal.textColor = Color.red;
            teamName = "TEAM B";
        }

        // Team Label
        GUILayout.Label(teamName, style);

        GUILayout.EndHorizontal();
    }
}