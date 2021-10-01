using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldNavigator : MonoBehaviour
{
    [HideInInspector] public gameController controller;

    public WorldNavObject ActiveNavObject {get; private set;}

    // events
    public static event Action<string> OnWorldLoaded;
    

    private void Awake() {
        controller = GetComponent<gameController>();  
        // var test = SubSceneManager.OnSceneLoaded.GetInvocationList();
        
            // Debug.Log("Subscene manager OnSceneLoaded is null.");
        
            SubSceneManager.OnSceneLoaded += SetupNewArea;
        
        
        Debug.Log("World Navigation manager setup.");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<WorldNavObject> GetNavObjects()
    {
        var objects = WorldNavObject.AllWorldObjects;
        //objects.Reverse();
        return objects;
    }

    public void SetupNewArea(string sceneName)
    {
        // Debug.Log("Current world objects: " + GetNavObjects().Count);
        var worldObjects = GetNavObjects();
        Debug.Log("Setting up for new scene: " + sceneName);
        if(worldObjects.Count > 0)
        {
            worldObjects.Reverse();
            // for(int i = 0; i < worldObjects.Count; i++)
            // {
            //     Debug.Log($"World object[{i}] name: " + worldObjects[i].Name);
            // }

            var rootObject = worldObjects[0].GetComponent<Transform>().root;
            Debug.Log("Root object: " + rootObject.name);
            ActiveNavObject = rootObject.GetComponent<WorldNavObject>();
            ActiveNavObject.ActivateNavObject();

            OnWorldLoaded?.Invoke(sceneName);
        }

    }

    public DialogueContainer GetActiveDialogue()
    {
        
        // while(ActiveNavObject == null)
        // {
        //     Debug.Log("ActiveNavObject is null. Getting default object for loaded scene.");
        //     SetupNewArea("");
        // }
        
        Debug.Log($"Active nav object name: {ActiveNavObject.Name}");
        if(ActiveNavObject.dialogue != null)
        {
            
            return ActiveNavObject.dialogue;
        }

        return null;
    }

    public void NavigateToChild(int index)
    {

    }

    public void NavigateToParent()
    {
        Debug.Log("Attempting to navigate to this object's parent.");
        if(ActiveNavObject !=null)
        {
            if(ActiveNavObject.ParentNavObject != null)
            {
                var destinationNavObject = ActiveNavObject.ParentNavObject.GetComponent<WorldNavObject>();
                if(destinationNavObject != null)
                {
                    ActiveNavObject.DeactivateNavObject();
                    ActiveNavObject = destinationNavObject;
                    ActiveNavObject.ActivateNavObject();
                }
                else {
                    Debug.Log("No Nav Object component found on this destination object. Please check scene.");  
                }
            }
            else {
                Debug.Log("This nav object is at the top of the object hierarchy.");
            }
        }
    }
}
