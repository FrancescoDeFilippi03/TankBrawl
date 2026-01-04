using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker.Models;
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
        var options = new SessionOptions { 
            MaxPlayers = 6,
            IsPrivate = true,
        }.WithRelayNetwork();
        
        try
        {
            currentSession = await MultiplayerService.Instance.CreateSessionAsync(options);


            TankConfigData tankConfigData = new TankDataBuilder()
            .WithTankId(0)
            .WithPlayerId(AuthenticationService.Instance.PlayerId)
            .WithClientId(NetworkManager.Singleton.LocalClientId)
            .WithTeam(TeamColor.Red)
            .Build();

            TeamManager.Instance.tankConfigs.Add(tankConfigData);

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


            TeamColor assignedTeam = (TeamManager.Instance.tankConfigs.Count % 2 == 0) ? TeamColor.Red : TeamColor.Blue;
            
            TankConfigData tankConfigData = new TankDataBuilder()
            .WithTankId(0)
            .WithPlayerId(AuthenticationService.Instance.PlayerId)
            .WithClientId(NetworkManager.Singleton.LocalClientId)
            .WithTeam(assignedTeam)
            .Build();
            
            TeamManager.Instance.RegisterMyPlayer(tankConfigData);
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
    }
    public async Task StartGame(){
        
        if(!currentSession.IsHost) return;
        
        Debug.Log("Starting game and loading scene...");

        currentSession.AsHost().IsLocked = true;
        await currentSession.AsHost().SavePropertiesAsync();
        
        NetworkManager.Singleton.SceneManager.LoadScene("TestGame", UnityEngine.SceneManagement.LoadSceneMode.Single);
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
    public TankConfigData GetTankConfigDataForPlayer(string playerId)
    {
        var player = GetPlayerById(playerId);
        if (player == null) return default;

        TankConfigData configData = new TankConfigData();
        configData.PlayerId = new Unity.Collections.FixedString64Bytes(playerId);
        var teamProp = GetPlayerProperty(playerId, "Team");


        if (teamProp != null)     
        {
            configData.Team = (teamProp.Value == "Red") ? TeamColor.Red : TeamColor.Blue;
        }

        return configData;
    }

}