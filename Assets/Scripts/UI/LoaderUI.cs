using System;
using System.Collections;
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

        Hide();
    }
    public IEnumerator LoadScreenScene(string sceneToTransit)
    {
        Show();
        
        float currentProgress = 0f;
        
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName : sceneToTransit);
        while (!loading.isDone)
        {
            float targetProgress = loading.progress * 100f;
            Debug.Log(targetProgress);
            currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * 5f);
            
            loaderBar.style.width = Length.Percent(currentProgress);
            
            yield return null;
        }
        
        loaderBar.style.width = Length.Percent(100);

        yield return new WaitForSeconds(0.2f);
        
        Hide();
    }
}