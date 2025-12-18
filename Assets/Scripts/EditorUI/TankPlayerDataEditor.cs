using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TankPlayerData))]
public class TankPlayerDataEditor : Editor
{
    private int hullIdInput;
    private int weaponIdInput;
    private int trackIdInput;

    public override void OnInspectorGUI()
    {
        // Disegna i riferimenti ai prefab (liste, controller, ecc.)
        DrawDefaultInspector();

        TankPlayerData script = (TankPlayerData)target;

        GUILayout.Space(15);
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.normal.textColor = Color.cyan;
        GUILayout.Label("NETWORK VARIABLE OVERRIDE", headerStyle);

        // Campi di input nell'Inspector
        hullIdInput = EditorGUILayout.IntField("New Hull ID", hullIdInput);
        weaponIdInput = EditorGUILayout.IntField("New Weapon ID", weaponIdInput);
        trackIdInput = EditorGUILayout.IntField("New Track ID", trackIdInput);

        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Apply & Sync"))
        {
            // Applichiamo i cambiamenti tramite i metodi creati nello script
            script.SetHull(hullIdInput);
            script.SetWeapon(weaponIdInput);
            script.SetTrack(trackIdInput);
            
            // Se siamo in Play Mode, Netcode farà scattare gli eventi OnValueChanged automaticamente
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(script);
                Debug.Log("Visuals updated in Editor Mode");
            }
        }

        if (GUILayout.Button("Reset to Defaults"))
        {
            hullIdInput = 0;
            weaponIdInput = 0;
            trackIdInput = 0;
        }

        GUILayout.EndHorizontal();

        // Messaggi di stato per l'utente
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Modifying IDs now will only change the VISUALS. Start Play Mode to sync via NetworkVariables.", MessageType.Warning);
        }
        else if (!script.IsOwner)
        {
            EditorGUILayout.HelpBox("You are NOT the owner of this tank. NetworkVariable sync is disabled.", MessageType.Error);
        }
    }
}