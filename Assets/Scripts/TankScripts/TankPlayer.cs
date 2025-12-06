using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    // Definiamo i team
    public enum TeamColor { Red, Blue }

    [Header("Team Settings")]
    // Variabile sincronizzata: appena il Server la cambia, tutti i client lo sanno
    public NetworkVariable<TeamColor> Team = new NetworkVariable<TeamColor>();

    [Header("Visual References")]
    [SerializeField] private SpriteRenderer tankSpriteRenderer; // Trascina qui lo sprite del tank
    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private Color blueColor = Color.blue;

    // Viene chiamato appena l'oggetto appare in rete
    public override void OnNetworkSpawn()
    {
        ApplyTeamColor(Team.Value);
        Team.OnValueChanged += OnTeamChanged;
    }

    public override void OnNetworkDespawn()
    {
        Team.OnValueChanged -= OnTeamChanged;
    }

    

    private void OnTeamChanged(TeamColor oldTeam, TeamColor newTeam)
    {
        ApplyTeamColor(newTeam);
    }

    private void ApplyTeamColor(TeamColor teamColor)
    {
        if (tankSpriteRenderer != null)
        {
            tankSpriteRenderer.color = (teamColor == TeamColor.Red) ? redColor : blueColor;
        }
    }

    public TeamColor GetTeam()
    {
        return Team.Value;
    }

    /// <summary>
    /// Initializes the tank with the specified team. Should be called on the server.
    /// </summary>
    [Rpc(SendTo.Server)]
    public void InitializeServerRpc(TeamColor teamColor)
    {
        Team.Value = teamColor;
    }

    /// <summary>
    /// Alternative: Direct initialization method (call only on server)
    /// </summary>
    public void Initialize(TeamColor teamColor)
    {
        if (!IsServer) return;
        Team.Value = teamColor;
    }
}