using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode; // Necessario per NetworkManager e NetworkList
using Unity.Collections; // Necessario se usi FixedString

// Assicurati che questo file sia posizionato all'interno di una cartella "Editor".

[CustomEditor(typeof(TeamManager))]
public class TeamManagerEditor : Editor
{
    // Definizioni di supporto (per la compilazione dell'Editor)
    // Se la tua struct TankConfigData e l'enum TeamColor non sono in un namespace, 
    // potresti non aver bisogno di queste definizioni, ma le includo per sicurezza.
    private const int MAX_WIDTH_ID = 60;
    private const int MAX_WIDTH_TEAM = 60;
    
    // Non è possibile prelevare direttamente la struct, quindi assumiamo la sua struttura.
    // Il compilatore dell'Editor prenderà la definizione corretta dallo spazio dei nomi globale.
    
    public override void OnInspectorGUI()
    {
        // 1. Disegna l'Inspector di default
        DrawDefaultInspector();

        TeamManager teamManager = (TeamManager)target;

        GUILayout.Space(15);
        GUI.backgroundColor = Color.cyan;
        GUILayout.Label("--- DATI SQUADRA NETWORKLIST ---", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.white;
        GUILayout.Space(5);

        // 2. Controlla lo stato del gioco e la lista
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (teamManager.tankConfigs == null || !teamManager.IsSpawned)
            {
                EditorGUILayout.HelpBox("TeamManager non è ancora spawnato sulla rete (solo l'Host lo spawna) o la lista è null.", MessageType.Info);
                return;
            }
            
            if (teamManager.tankConfigs.Count == 0)
            {
                 EditorGUILayout.HelpBox("NetworkList pronta, ma vuota (nessun giocatore registrato).", MessageType.Info);
                 return;
            }

            // Forza l'aggiornamento dell'Inspector
            EditorUtility.SetDirty(teamManager); 
            Repaint();

            EditorGUILayout.LabelField($"Totale Giocatori: {teamManager.tankConfigs.Count}", EditorStyles.boldLabel);
            
            // Inizia l'area di visualizzazione dati
            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            // Intestazione delle colonne
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("Client ID", EditorStyles.boldLabel, GUILayout.Width(MAX_WIDTH_ID));
            EditorGUILayout.LabelField("Team", EditorStyles.boldLabel, GUILayout.Width(MAX_WIDTH_TEAM));
            EditorGUILayout.LabelField("Base ID", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.LabelField("Arma ID", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            
            // Itera sui dati
            for (int i = 0; i < teamManager.tankConfigs.Count; i++)
            {
                // Leggiamo la struct
                TankConfigData data = teamManager.tankConfigs[i];
                
                // Colore per distinguere i team
                Color bgColor = (data.Team == TeamColor.Red) ? new Color(1f, 0.7f, 0.7f) : new Color(0.7f, 0.7f, 1f);
                GUI.backgroundColor = bgColor;

                EditorGUILayout.BeginVertical(GUI.skin.box); // Inizia box colorato

                // Riga principale: ID, Team, Base/Arma ID
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(data.ClientId.ToString(), GUILayout.Width(MAX_WIDTH_ID));
                EditorGUILayout.LabelField(data.Team.ToString(), GUILayout.Width(MAX_WIDTH_TEAM));
                EditorGUILayout.LabelField(data.BaseId.ToString(), GUILayout.Width(60));
                EditorGUILayout.LabelField(data.WeaponId.ToString());
                EditorGUILayout.EndHorizontal();

                // Riga dettagli (Turret e Bullet)
                GUI.backgroundColor = Color.Lerp(bgColor, new Color(0.8f, 0.8f, 0.8f), 0.5f);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(5); // Piccola spaziatura
                EditorGUILayout.LabelField($"Bullet ID: {data.BulletId}", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical(); // Fine box colorato
                
                GUI.backgroundColor = Color.white;
            }
            
            EditorGUILayout.EndVertical(); // Fine area dati

            GUILayout.Space(10);
            if (GUILayout.Button("Forza Aggiornamento Inspector"))
            {
                // Forzare Repaint può aiutare se i dati cambiano velocemente
                Repaint();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("I dati della NetworkList sono visibili solo in modalità Play (e solo sull'Host/Server).", MessageType.Warning);
        }
    }
}