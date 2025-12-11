using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class NetworkLifecycle : MonoBehaviour
{
    public static Dictionary<ulong, string> ClientIdToPlayerId = new Dictionary<ulong, string>();
    private void Awake()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.gameObject != gameObject)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Iscriviti al callback di approvazione
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        byte[] connectionData = request.Payload;
        string playerId = Encoding.UTF8.GetString(connectionData);

        ulong clientId = request.ClientNetworkId;
        
        if (ClientIdToPlayerId.ContainsKey(clientId))
            ClientIdToPlayerId[clientId] = playerId;
        else
            ClientIdToPlayerId.Add(clientId, playerId);

        Debug.Log($"Mappato Client {clientId} -> Player {playerId}");

        response.Approved = true;
        response.CreatePlayerObject = false;
    }
}