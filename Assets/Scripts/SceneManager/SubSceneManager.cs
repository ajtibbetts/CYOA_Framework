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
        WorldNavObject.OnObjectsLoaded += ConfirmSceneLoaded;
    }
    
    public static void AddScene(string sceneName)
    {
        // check if scene is already loaded
        var sceneToLoad = SceneManager.GetSceneByName(sceneName);
        Debug.Log("OnSceneLoaded sub count: " + OnSceneLoaded.GetInvocationList().Length);
        if(!sceneToLoad.isLoaded)
        {
            Debug.Log($"Loading scene additive: {sceneName}");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            _currentLoadeSceneName = sceneName;
            
        }
        else 
        {
            Debug.Log($"Scene {sceneName} is already loaded.");
            Debug.Log("Firing off onSceneLoaded event!");
            OnSceneLoaded?.Invoke(sceneName);
            
        }
    }

    public static void ConfirmSceneLoaded()
    {
        Debug.Log("Scene load complete. Firing off onSceneLoaded event!");
        OnSceneLoaded?.Invoke(_currentLoadeSceneName);
    }

    public static void RemoveScene(string sceneName)
    {
        Debug.Log($"Removing scene additive: {sceneName}");
        OnSceneMarkedForUnload?.Invoke(sceneName);
        SceneManager.UnloadSceneAsync(sceneName);
        OnSceneUnloaded?.Invoke(sceneName);
    }

}
