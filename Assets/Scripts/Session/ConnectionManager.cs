using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class ConnectionManager : NetworkBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    public event Action<ulong> OnClientConnectedEvent;
    public event Action<ulong> OnClientDisconnectedEvent;
    public event Action<string, LoadSceneMode, List<ulong>, List<ulong>> OnSceneLoadedEvent;
    public event Action OnLocalClientDisconnected;

    private bool isShuttingDown = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        isShuttingDown = false;
        SetupConnectionCallbacks();
    }

    private void SetupConnectionCallbacks()
    {
        

        // Eventi solo per il server
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        }
        else{

            // Eventi per tutti i client
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected");
        OnClientConnectedEvent?.Invoke(clientId);
    }

    private async void OnClientDisconnected(ulong clientId)
    {
        if (isShuttingDown)
            return;
            
        Debug.Log($"Client {clientId} disconnected - shutting down server");
        OnClientDisconnectedEvent?.Invoke(clientId);
        
        isShuttingDown = true;
        
        await SessionManager.Instance.LeaveSession();
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log($"Scene {sceneName} loaded. Clients completed: {clientsCompleted.Count}, Timed out: {clientsTimedOut.Count}");
        OnSceneLoadedEvent?.Invoke(sceneName, loadSceneMode, clientsCompleted, clientsTimedOut);
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Local client disconnected, returning to lobby");
            OnLocalClientDisconnected?.Invoke();
        }
    }



    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null)
        {
        
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
            }
            else
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
            }
        }
    }
}