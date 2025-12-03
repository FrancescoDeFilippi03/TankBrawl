using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }



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

    async Task StartSessionAsHost()
    {
        var options = new SessionOptions
        {
            MaxPlayers = 2
        }.WithRelayNetwork(); // or WithDistributedAuthorityNetwork() to use Distributed Authority instead of Relay
        var session = await MultiplayerService.Instance.CreateSessionAsync(options);
        Debug.Log($"Session {session.Id} created! Join code: {session.Code}");
    }
}
