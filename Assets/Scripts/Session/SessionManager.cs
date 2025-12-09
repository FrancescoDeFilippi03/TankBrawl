using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;
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
            Debug.Log($"Joined session. Waiting for Host to assign team...");
            
        }
        catch (Exception e) 
        { 
            Debug.LogException(e);
        }
    }


    //COMMON LOGIC
    public async Task LeaveSession()
    {
        if (currentSession != null)
        {
            try 
            {
                if(currentSession.IsHost)
                {
                    await currentSession.AsHost().DeleteAsync();
                    Debug.Log("Session deleted from cloud.");
                }
                else
                {
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
        //Destroy(gameObject);
    }
    public async Task StartGame(){
        
        if(!currentSession.IsHost) return;
        
        Debug.Log("Starting game and loading scene...");
        
        NetworkManager.Singleton.SceneManager.LoadScene("TestGame", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

}