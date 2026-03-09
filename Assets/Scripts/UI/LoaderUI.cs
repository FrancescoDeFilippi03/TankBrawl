using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoaderUI : UI
{
    public static LoaderUI Instance { get; private set; }
    VisualElement loaderBar;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (root == null) return;
        
        loaderBar = root.Q<VisualElement>("LoaderBar");

        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.OnHostStarted += OnNetworkManagerReady;
            SessionManager.Instance.OnClientStarted += OnNetworkManagerReady;
        }

        Hide();
    }
    
    private void OnNetworkManagerReady()
    {
        SubscribeToNetworkEvents();
    }

    private void OnDisable()
    {
        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.OnHostStarted -= OnNetworkManagerReady;
            SessionManager.Instance.OnClientStarted -= OnNetworkManagerReady;
        }
        
        // Rimuovi sottoscrizione da NetworkManager
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            UnsubscribeFromNetworkEvents();
        }
    }
    
    private void SubscribeToNetworkEvents()
    {
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.SceneManager == null)
        {
            Debug.LogWarning("LoaderUI: NetworkManager or SceneManager not available");
            return;
        }
        
        NetworkManager.Singleton.SceneManager.OnLoad += OnNetworkSceneLoad;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnNetworkSceneLoadCompleted;
    }
    
    private void UnsubscribeFromNetworkEvents()
    {
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.SceneManager == null)
            return;
            
        NetworkManager.Singleton.SceneManager.OnLoad -= OnNetworkSceneLoad;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnNetworkSceneLoadCompleted;
    }
    
    private void OnNetworkSceneLoad(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            LoadScreenSceneMultiplayer(asyncOperation);
        }
    }
    
    private void OnNetworkSceneLoadCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log($"Scene {sceneName} loaded. Clients completed: {clientsCompleted.Count}, Timed out: {clientsTimedOut.Count}");
    }


    //SINGLE PLAYER
    public void LoadScreenScene(string sceneToTransit)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName : sceneToTransit);
        StartCoroutine(LoadScreen(loading));
    }
    
    //MULTIPLAYER
    private void LoadScreenSceneMultiplayer(AsyncOperation asyncOperation)
    {
        StartCoroutine(LoadScreen(asyncOperation));
    }

    private IEnumerator LoadScreen(AsyncOperation asyncOperation)
    {
        Show();
        
        float currentProgress = 0f;
        
        while (!asyncOperation.isDone)
        {
            float targetProgress = asyncOperation.progress * 100f;
            Debug.Log($"Scene loading: {targetProgress}%");
            currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * 5f);
            
            loaderBar.style.width = Length.Percent(currentProgress);
            
            yield return null;
        }
        
        loaderBar.style.width = Length.Percent(100);

        yield return new WaitForSeconds(0.2f);
        
        Hide();
    }    
}