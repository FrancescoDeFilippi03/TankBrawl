using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }
    private ISession currentSession;
    public ISession CurrentSession => currentSession;
    public bool IsInitialized { get; private set; } = false;



    //delegate for NetworkManager usage
    public delegate void StartHostDelegate();
    public event StartHostDelegate StartHost;


    public delegate void StartClientDelegate();
    public event StartClientDelegate StartClient;


    public delegate void SessionLeaveDelegate();
    public event SessionLeaveDelegate SessionEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this) return;
        Instance = this;
    }

    async void Start()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
        {
            IsInitialized = true;
            return;
        }

        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            IsInitialized = true;
            Debug.Log($"Signed in: {AuthenticationService.Instance.PlayerId}");

            await NetworPlayerManager.Instance.InitializeAsync();
            await TeamManager.Instance.InitializeAsync();



        }
        catch (Exception e) { Debug.LogException(e); }
    }

    

    //HOST LOGIC
    public async Task StartSessionAsHost()
    {
        if (!IsInitialized) return;

        var options = new SessionOptions { MaxPlayers = 6 }.WithRelayNetwork();

        try
        {
            currentSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            Debug.Log($"Host Session Created: {currentSession.Id}");
            Debug.Log($"Code :{currentSession.Code}");
            currentSession.PlayerJoined += OnPlayerJoined;

            StartHost?.Invoke();
        }
        catch (Exception e) 
        { 
            Debug.LogException(e);
        }
    }

    //CLIENT LOGIC 

    public async Task JoinSessionAsClient(string joinCode)
    {
        if (!IsInitialized) return;
        //await CheckNetworkManagerIsListeningAndShutdown();

        try
        {
            currentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
            Debug.Log($"Joined session. Waiting for Host to assign team...");
            currentSession.PlayerJoined += OnPlayerJoined;

            StartClient?.Invoke();
        }
        catch (Exception e) 
        { 
            Debug.LogException(e);
        }
    }

    public async Task LeaveSession()
    {
        if (currentSession != null)
        {
            try
            {
                Debug.Log($"Leaving session... {currentSession.CurrentPlayer.Id} {currentSession.Id}");
                await currentSession.LeaveAsync();
                Debug.Log("Left session successfully.");
            }
            catch (Exception e) { 
                // Session might already be closed (e.g., server shut down the lobby)
                Debug.Log($"Could not leave session (likely already closed): {e.Message}");
            }
            finally
            {
                currentSession = null;
                SessionEnded?.Invoke();
            }
        }
    }

    private void OnPlayerJoined(string playerId) { Debug.Log($"Player Joined: {playerId}"); }


}