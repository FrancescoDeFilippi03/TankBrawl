using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }
    private ISession currentSession;
    public ISession CurrentSession => currentSession;


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
        var options = new SessionOptions { MaxPlayers = 6 }.WithRelayNetwork();

        try
        {
            currentSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            Debug.Log($"Host Session Created: {currentSession.Id}");
            Debug.Log($"Code :{currentSession.Code}");
            
            currentSession.PlayerJoined += OnPlayerJoined;
            NetworkManager.Singleton.StartHost();
        }
        catch (Exception e) 
        { 
            Debug.LogException(e);
        }
    }


    public async Task StartGame(){
        
        if(!currentSession.IsHost) return;
        NetworkManager.Singleton.SceneManager.LoadScene("TestGame", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    //CLIENT LOGIC 

    public async Task JoinSessionAsClient(string joinCode)
    {

        try
        {
            currentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
            Debug.Log($"Joined session. Waiting for Host to assign team...");
            
            currentSession.PlayerJoined += OnPlayerJoined;
            NetworkManager.Singleton.StartClient();
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

                NetworkManager.Singleton.Shutdown();
            }
            catch (Exception e) { 
                Debug.Log($"Could not leave session (likely already closed): {e.Message}");
            }
            finally
            {
                currentSession = null;
            }
        }
    }

    private void OnPlayerJoined(string playerId) { Debug.Log($"Player Joined: {playerId}"); }


}