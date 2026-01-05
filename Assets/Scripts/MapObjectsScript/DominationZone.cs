using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DominationZone : NetworkBehaviour
{
    // Tieni traccia di chi è all'interno
    private Dictionary<TeamColor, int> tanksInZone = new Dictionary<TeamColor, int>
    {
        { TeamColor.Red, 0 },
        { TeamColor.Blue, 0 }
    };


    [SerializeField] SpriteRenderer zoneInsideSpriteRenderer;
    [SerializeField] SpriteRenderer zoneCenterSpriteRenderer;

    [SerializeField] Sprite redTeamCenterSprite;
    [SerializeField] Sprite blueTeamCenterSprite;
    [SerializeField] Sprite neutralCenterSprite;


    public override void OnNetworkSpawn()
    {        
        GameManager.Instance.controllingTeam.OnValueChanged += OnControllingTeamChanged;
    }

    public override void OnNetworkDespawn()
    {
        GameManager.Instance.controllingTeam.OnValueChanged -= OnControllingTeamChanged;
    }
    
    void Update()
    {
        if (IsServer) 
        {
            GameManager.Instance.controllingTeam.Value = GetCurrentControl();
        }

    }

    private TeamColor GetCurrentControl()
    {
        int redCount = tanksInZone[TeamColor.Red];
        int blueCount = tanksInZone[TeamColor.Blue];
        return redCount > blueCount ? TeamColor.Red :
               blueCount > redCount ? TeamColor.Blue :
               TeamColor.None;
    }

    private void UpdateVisuals(TeamColor team)
    {
        Color newColor = Color.white;
        Sprite centerSprite = neutralCenterSprite;
        
        if (team == TeamColor.Red)
        {
            newColor = Color.red;
            centerSprite = redTeamCenterSprite;
        }
        else if (team == TeamColor.Blue)
        {
            newColor = Color.blue;
            centerSprite = blueTeamCenterSprite;
        }

        zoneInsideSpriteRenderer.color = new Color(newColor.r, newColor.g, newColor.b, 40 / 255f);
        
        if (centerSprite != null)
        {
            zoneCenterSpriteRenderer.sprite = centerSprite;
        }
    }

    private void OnControllingTeamChanged(TeamColor oldTeam, TeamColor newTeam)
    {
        UpdateVisuals(newTeam);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Tank>(out var tankController))
        {
            TeamColor team = tankController.TankConfigData.Team; 
            
            if (team != TeamColor.None)
            {
                tanksInZone[team]++;
            }
        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Tank>(out var tankController))
        {
            TeamColor team = tankController.TankConfigData.Team; 
            
            if (team != TeamColor.None && tanksInZone[team] > 0)
            {
                tanksInZone[team]--;
            }
        }
    }
}