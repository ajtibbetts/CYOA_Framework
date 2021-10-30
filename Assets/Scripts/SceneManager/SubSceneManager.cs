using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// using Unity.Entities;
// using Unity.Scenes;

public static class SubSceneManager 
{
   
    public static event Action<string> OnSceneLoaded;
    public static event Action<string> OnSceneMarkedForUnload;
    public static event Action<string, string> OnSceneUnloaded;

    private static string _currentLoadedSceneName;
    private static string _newSceneToLoadName;

    static SubSceneManager()
    {
        SceneManager.sceneLoaded += OnSceneLoadComplete; // use scene manager delegate from API
        SceneManager.sceneUnloaded += OnSceneUnloadComplete;
    }

    public static string GetCurrentSceneName()
    {
        return _currentLoadedSceneName;
    }
    
    public static void AddScene(string sceneName)
    {
        // check if scene is already loaded
        var sceneToLoad = SceneManager.GetSceneByName(sceneName);
        Debug.Log("SCENE MANAGER ---- OnSceneLoaded sub count: " + OnSceneLoaded.GetInvocationList().Length);
        if(!sceneToLoad.isLoaded)
        {
            Debug.Log($"SCENE MANAGER ---- Loading scene additive: {sceneName}");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            _currentLoadedSceneName = sceneName;
            
        }
        else 
        {
            Debug.Log($"SCENE MANAGER ---- Scene {sceneName} is already loaded.");
            Debug.Log("SCENE MANAGER ---- Firing off onSceneLoaded event!");
            _currentLoadedSceneName = sceneName;
            OnSceneLoaded?.Invoke(sceneName);
        }
    }

    public static void RemoveScene(string sceneName, string newSceneName)
    {
        Debug.Log($"SCENE MANAGER ---- Removing scene additive: {sceneName}");
        _newSceneToLoadName = newSceneName;
        OnSceneMarkedForUnload?.Invoke(sceneName);
        SceneManager.UnloadSceneAsync(sceneName);
        
    }

    public static void OnSceneLoadComplete(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("SCENE MANAGER ---- OnSceneLoaded: " + scene.name + " | Load Mode: " + mode);
        if(_currentLoadedSceneName == scene.name && mode == LoadSceneMode.Additive)
        {
            Debug.Log("SCENE MANAGER ---- Additive scene confirmed for load: " + _currentLoadedSceneName);
            OnSceneLoaded?.Invoke(_currentLoadedSceneName);
        }
    }

    public static void OnSceneUnloadComplete(Scene scene)
    {
        Debug.Log("SCENE MANAGER ---- OnSceneULoaded: " + scene.name);
        OnSceneUnloaded?.Invoke(_currentLoadedSceneName, _newSceneToLoadName);
    }

}
