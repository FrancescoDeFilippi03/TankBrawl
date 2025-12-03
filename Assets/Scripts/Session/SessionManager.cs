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

    //singleton pattern don't drestroy on load 
    // ensures only one instance of SessionManager exists
    //rember to clean up for not having memory leaks
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Onable()
    {
        //subscribe to events
    }
    
    private void OnDisable()
    {
        //unsubscribe from events
    }

    async void Start()
    {   
    
        try
	    {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Sign in anonymously succeeded! PlayerID: {AuthenticationService.Instance.PlayerId}");
	    }
	    catch (Exception e)
	    {
	        Debug.LogException(e);
	    }
    }

    public async Task StartSessionAsHost()
    {
        var options = new SessionOptions
        {
            MaxPlayers = 6

        }.WithRelayNetwork();

        try
        {

            currentSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            Debug.Log($"Session {currentSession.Id} created! Join code: {currentSession.Code}");

        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return;
        }
      
    }

    public async Task JoinSessionAsClient(string joinCode)
    {
        try
        {
            currentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
            Debug.Log($"Joined session {currentSession.Id} with join code: {joinCode}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return;
        }
    }

    

}
