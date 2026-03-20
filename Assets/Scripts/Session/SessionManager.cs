using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;
    private ISession currentSession;
    public ISession CurrentSession => currentSession;    
    public event Action OnHostStarted;
    public event Action OnClientStarted;
    
    [SerializeField] private GameObject sessionDataManagerPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    async void Start()
    {
        await InitializeServicesAsync();
    }
    
    public async Task InitializeServicesAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            Debug.Log($"Signed in: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e) { 
            Debug.LogException(e); 
        }
    }


    //HOST LOGIC
    public async Task StartSessionAsHost()
    {
        var options = new SessionOptions { 
            MaxPlayers = 6,
            IsPrivate = true,
        }.WithRelayNetwork(); //new RelayNetworkOptions(RelayProtocol.WSS) da usare poi alla fine
        
        try
        {
            currentSession = await MultiplayerService.Instance.CreateSessionAsync(options);

            OnHostStarted?.Invoke();

            SubscribeToNetworkEvents();

            Instantiate(sessionDataManagerPrefab).GetComponent<NetworkObject>().Spawn();

            Debug.Log($"Host Session Created: {currentSession.Id} (Team: Red)");
            Debug.Log($"Code :{currentSession.Code}");
        }
        catch (Exception e) 
        { 
            Debug.LogException(e);
        }
    }

    //CLIENT LOGIC 
    public async Task JoinSessionAsClient(string joinCode)
    {
        try
        {
            currentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);

            //currentSession.RemovedFromSession += OnRemovedFromSession;

            SubscribeToNetworkEvents();  

            OnClientStarted?.Invoke();

        }
        catch (Exception e) 
        { 
            Debug.LogException(e);
        }
    }

    private void OnRemovedFromSession()
    {
        currentSession = null;
        Debug.Log("Removed from session, returning to main menu");
        LoaderUI.Instance.LoadScreenScene("MainMenu");
    }

    //COMMON LOGIC
    public async Task LeaveSession()
    {

        Debug.Log("Leaving session....");
        if (currentSession != null)
        {

            UnsubscribeFromNetworkEvents();

            try 
            {
                if(currentSession.IsHost)
                {
                    await currentSession.AsHost().DeleteAsync();
                    Debug.Log("Session deleted from cloud.");
                }
                else
                {
                    //currentSession.RemovedFromSession -= OnRemovedFromSession;
                    await currentSession.LeaveAsync();
                    Debug.Log("Left session.");
                }
            }
            catch(Exception e) 
            { 
                Debug.LogException(e);
            }

            currentSession = null;
        }
    }
    public async Task StartGame(){
        
        if(!currentSession.IsHost) return;
        
        Debug.Log("Starting game and loading scene...");

        currentSession.AsHost().IsLocked = true;

        await currentSession.AsHost().SavePropertiesAsync();
        
        NetworkManager.Singleton.SceneManager.LoadScene("TestGame",LoadSceneMode.Single);

    }

    //NETWORK EVENTS
    private void SubscribeToNetworkEvents()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkClientDisconnected;
    }

    private void UnsubscribeFromNetworkEvents()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnNetworkClientDisconnected;
    }

    private void OnNetworkClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Disconnected from host");
            currentSession = null;

            if(LoaderUI.Instance != null)
                LoaderUI.Instance.LoadScreenScene("MainMenu");
        }
    }

    //UTILS
    public IPlayer GetPlayerById(string playerId)
    {
        if (currentSession == null) return null;

        return currentSession.AsHost().GetPlayer(playerId);
    }
    public PlayerProperty GetPlayerProperty(string playerId, string propertyKey)
    {
        var player = GetPlayerById(playerId);
        if (player != null && player.Properties.ContainsKey(propertyKey))
        {
            return player.Properties[propertyKey];
        }
        return null;
    }

}