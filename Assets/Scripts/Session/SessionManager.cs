using System;
using System.Collections.Generic;
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
        var initialProps = UpdateLoadoutToSession();
        initialProps.Add("Team", new PlayerProperty("Red", VisibilityPropertyOptions.Member));

        var options = new SessionOptions { 
            MaxPlayers = 6,
            IsPrivate = true,
            PlayerProperties = initialProps
        }.WithRelayNetwork();
        
        try
        {
            currentSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            Debug.Log($"Host Session Created: {currentSession.Id} (Team: Red)");
            Debug.Log($"Code :{currentSession.Code}");
        }
        catch (Exception e) 
        { 
            Debug.LogException(e);
        }
    }


    
    //CLIENT LOGIC 
    /* 
    public async Task JoinSessionAsClient(string joinCode)
    {   
        // 1. Entra con le sole proprietà del Loadout (Niente Team ancora)
        var options = new JoinSessionOptions
        {
            PlayerProperties = UpdateLoadoutToSession()
        };

        try
        {
            currentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode, options);

            int myIndex = currentSession.Players.Count - 1;
            
            string myTeam = (myIndex % 2 == 0) ? "Red" : "Blue";

            Debug.Log($"I am player #{myIndex} (Total: {currentSession.Players.Count}). Team: {myTeam}");

            var myPlayer = currentSession.CurrentPlayer;
            myPlayer.SetProperty("Team", new PlayerProperty(myTeam, VisibilityPropertyOptions.Member));

            await currentSession.SaveCurrentPlayerDataAsync();
        }
        catch (Exception e) 
        { 
            Debug.LogException(e);
        }
    }

 */
    public async Task JoinSessionAsClient(string joinCode)
    {   
        var options = new JoinSessionOptions { PlayerProperties = UpdateLoadoutToSession() };

        try
        {
            currentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode, options);
            
           
            string hostId =  currentSession.Host;

            List<string> clientIds = new List<string>();
            foreach(var p in currentSession.Players)
            {
                if (p.Id != hostId)
                {
                    clientIds.Add(p.Id);
                }
            }

            clientIds.Sort();

            string myId = AuthenticationService.Instance.PlayerId;
            int myClientIndex = clientIds.IndexOf(myId);

            if (myClientIndex != -1) 
            {
                string myTeam = (myClientIndex % 2 == 0) ? "Blue" : "Red";

                Debug.Log($"Host is Red. I am Client #{myClientIndex}. Team: {myTeam}");

                var myPlayer = currentSession.CurrentPlayer;
                myPlayer.SetProperty("Team", new PlayerProperty(myTeam, VisibilityPropertyOptions.Member));
                await currentSession.SaveCurrentPlayerDataAsync();
            }
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


    public Dictionary<string, PlayerProperty> UpdateLoadoutToSession()
    {
        PlayerLoadoutData loadout = LoadoutSystem.LoadLoadout();

        var properties = new Dictionary<string, PlayerProperty>
        {
            { "BaseId",   new PlayerProperty(loadout.BaseId.ToString(),   VisibilityPropertyOptions.Member) },
            { "TurretId", new PlayerProperty(loadout.TurretId.ToString(), VisibilityPropertyOptions.Member) },
            { "WeaponId", new PlayerProperty(loadout.WeaponId.ToString(), VisibilityPropertyOptions.Member) },
            { "BulletId", new PlayerProperty(loadout.BulletId.ToString(), VisibilityPropertyOptions.Member) }
        };
        return properties;
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

        var baseIdProp = GetPlayerProperty(playerId, "BaseId");
        var turretIdProp = GetPlayerProperty(playerId, "TurretId");
        var weaponIdProp = GetPlayerProperty(playerId, "WeaponId");
        var bulletIdProp = GetPlayerProperty(playerId, "BulletId");
        var teamProp = GetPlayerProperty(playerId, "Team");

        if (baseIdProp != null)   configData.BaseId   = int.Parse(baseIdProp.Value);
        if (turretIdProp != null) configData.TurretId = int.Parse(turretIdProp.Value);
        if (weaponIdProp != null) configData.WeaponId = int.Parse(weaponIdProp.Value);
        if (bulletIdProp != null) configData.BulletId = int.Parse(bulletIdProp.Value);
        if (teamProp != null)     
        {
            configData.Team = (teamProp.Value == "Red") ? TeamColor.Red : TeamColor.Blue;
        }

        return configData;
    }

}