using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class NetworPlayerManager : MonoBehaviour
{
    public static NetworPlayerManager Instance { get; private set; }


    //TeamManager usage
    public delegate void PlayerEventDelegate(ulong clientId);
    public event PlayerEventDelegate PlayerConnected;
    public event PlayerEventDelegate PlayerDisconnected;

    public delegate void PlayerClearList();
    public event PlayerClearList PlayerListCleared;




    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }
        Instance = this;

    }

    public async Task InitializeAsync()
    {
        SubscribeToSessionEvents();
        SubscribeToNetworkEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromSessionEvents();
        UnsubscribeFromNetworkEvents();
    }

    void SubscribeToSessionEvents()
    {
        SessionManager.Instance.StartHost += OnStartHost;
        SessionManager.Instance.StartClient += OnStartClient;
        SessionManager.Instance.SessionEnded += OnSessionEnded;
    }

    void SubscribeToNetworkEvents()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            NetworkManager.Singleton.OnServerStopped += OnServerStopped;
        }
    }

    void UnsubscribeFromSessionEvents()
    {
        SessionManager.Instance.StartHost -= OnStartHost;
        SessionManager.Instance.StartClient -= OnStartClient;
        SessionManager.Instance.SessionEnded -= OnSessionEnded;
    }
    void UnsubscribeFromNetworkEvents()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        }
    }

    //Session Event Handlers
   private void OnStartHost()
    {
        Debug.Log("Host started - NetworPlayerManager received the event.");
        // Additional logic for when hosting starts
        NetworkManager.Singleton.StartHost();
    }

    private void OnStartClient()
    {
        Debug.Log("Client started - NetworPlayerManager received the event.");
        // Additional logic for when client starts
        NetworkManager.Singleton.StartClient();
    }

    private void OnSessionEnded()
    {
        Debug.Log("Session ended - NetworPlayerManager received the event.");
        // Unsubscribe from network events before shutting down
        UnsubscribeFromNetworkEvents();
        // Additional logic for when session ends
        NetworkManager.Singleton.Shutdown();
    }


    //Network Event Handlers
    private async void OnServerStopped(bool isHost)
    {
        // When Host stops the game, clear the list
        if (isHost)
        {
            PlayerListCleared?.Invoke();
            Debug.Log("Server Stopped: Team dictionary cleared.");
        }

        // All players leave the session when server stops
        await SessionManager.Instance.LeaveSession();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = true; 
        response.Pending = false;

        // Assign team upon approval
        PlayerConnected?.Invoke(request.ClientNetworkId);
        
    }

    private async void OnClientDisconnect(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // Server handling client disconnection
            Debug.Log($"Client {clientId} disconnected and removed from team list.");
            PlayerDisconnected?.Invoke(clientId);
        }
        else
        {
            // Client detected disconnection (server stopped or lost connection)
            Debug.Log("Disconnected from server. Leaving session...");
            await SessionManager.Instance.LeaveSession();
        }
    }
}
