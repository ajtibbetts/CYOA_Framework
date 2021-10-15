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
    public static event Action<string> OnSceneUnloaded;

    private static string _currentLoadeSceneName;

    static SubSceneManager()
    {
        SceneManager.sceneLoaded += OnSceneLoadComplete; // use scene manager delegate from API
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
            _currentLoadeSceneName = sceneName;
            
        }
        else 
        {
            Debug.Log($"SCENE MANAGER ---- Scene {sceneName} is already loaded.");
            Debug.Log("SCENE MANAGER ---- Firing off onSceneLoaded event!");
            OnSceneLoaded?.Invoke(sceneName);
            
        }
    }

    public static void RemoveScene(string sceneName)
    {
        Debug.Log($"SCENE MANAGER ---- Removing scene additive: {sceneName}");
        OnSceneMarkedForUnload?.Invoke(sceneName);
        SceneManager.UnloadSceneAsync(sceneName);
        OnSceneUnloaded?.Invoke(sceneName);
    }

    public static void OnSceneLoadComplete(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("SCENE MANAGER ---- OnSceneLoaded: " + scene.name + " | Load Mode: " + mode);
        if(_currentLoadeSceneName == scene.name && mode == LoadSceneMode.Additive)
        {
            Debug.Log("SCENE MANAGER ---- Additive scene confirmed for load: " + _currentLoadeSceneName);
            OnSceneLoaded?.Invoke(_currentLoadeSceneName);
        }
    }

}
