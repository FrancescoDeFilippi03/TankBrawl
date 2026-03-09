using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.Netcode;

public class GameStatsUI : UI
{
    private List<VisualElement> teamRows = new List<VisualElement>();
    protected override void OnEnable()
    {
        base.OnEnable();

        //divisi sulla base di dispari e pari A pari e B dispari
        teamRows.Add(root.Q<VisualElement>("TeamA_RowContainer_1"));
        teamRows.Add(root.Q<VisualElement>("TeamB_RowContainer_1"));

        teamRows.Add(root.Q<VisualElement>("TeamA_RowContainer_2"));
        teamRows.Add(root.Q<VisualElement>("TeamB_RowContainer_2"));

        teamRows.Add(root.Q<VisualElement>("TeamA_RowContainer_3"));
        teamRows.Add(root.Q<VisualElement>("TeamB_RowContainer_3"));

        
    }

    public void BindTank(Tank tank)
    {
        var row = teamRows[(int)tank.PlayerData.ClientId];
        
        tank.kill.OnValueChanged += (prev, newVal) => 
            row.Q<Label>("Kill").text = newVal.ToString();
            
        tank.death.OnValueChanged += (prev, newVal) => 
            row.Q<Label>("Death").text = newVal.ToString();
    }
}
