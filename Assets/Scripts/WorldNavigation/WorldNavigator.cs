using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldNavigator : MonoBehaviour
{
    [HideInInspector] public gameController controller;

    public WorldNavObject ActiveNavObject {get; private set;}

    // events
    public static event Action<string> OnActiveNavObjectLoaded;
    

    private void Awake() {
        controller = GetComponent<gameController>();  
        // var test = SubSceneManager.OnSceneLoaded.GetInvocationList();
        
            // Debug.Log("Subscene manager OnSceneLoaded is null.");
        
            SubSceneManager.OnSceneLoaded += SetupNewArea;
        
        
        Debug.Log("WORLD NAVIGATOR ---- World Navigation manager setup.");
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
        Debug.Log("WORLD NAVIGATOR ---- Setting up for new scene: " + sceneName);
        if(worldObjects.Count > 0)
        {
            worldObjects.Reverse();
            // for(int i = 0; i < worldObjects.Count; i++)
            // {
            //     Debug.Log($"World object[{i}] name: " + worldObjects[i].Name);
            // }

            var rootObject = worldObjects[0].GetComponent<Transform>().root;
            Debug.Log("WORLD NAVIGATOR ---- Root object in this scene (top of hierarchy): " + rootObject.name);
            ActiveNavObject = rootObject.GetComponent<WorldNavObject>();
            ActiveNavObject.ActivateNavObject();

            OnActiveNavObjectLoaded?.Invoke(sceneName);
        }

    }

    public DialogueContainer GetActiveDialogue()
    {

        Debug.Log($"WORLD NAVIGATOR ---- Checking active nav object name: {ActiveNavObject.Name} for dialogue graph.");
        if(ActiveNavObject.dialogue != null)
        {
            return ActiveNavObject.dialogue;
        }

        return null;
    }

    public void DisplayActiveNavObject()
    {
        // register event listener
        UIManager.onOptionSelected += NavigateToNavObject;
        
        Debug.Log($"WORLD NAVIGATOR ---- Displaying active world nav object: {ActiveNavObject.Name}");

        // add logic here to determine whether to display new or returning text
        bool isReturned = false;
        if(!isReturned)
        {
            controller.UIManager.updateContentText(ActiveNavObject.descriptionNew);
        }
        else 
        {
            controller.UIManager.updateContentText(ActiveNavObject.descriptionReturned);
        }       
        controller.UIManager.ClearButtons();

        // list all child objects as options
        List<WorldNavObject> childNavObjects = ActiveNavObject.AllChildObjects;
        foreach(WorldNavObject navObject in childNavObjects)
        {
            // add logic here to determine whether new/returned
            bool isNavReturned = false;
            if(!isNavReturned)
            {
                // Debug.Log("child nav object: " + navObject.buttonTextNew);
                controller.UIManager.CreateDialogueOptionButton(navObject.GUID, navObject.buttonTextNew);
            }
            else
            {
                // Debug.Log("child nav object: " + navObject.buttonTextNew);
                controller.UIManager.CreateDialogueOptionButton(navObject.GUID, navObject.buttonTextReturned);
            }
        }
        // add choice to go back to parent or map
        if(ActiveNavObject.ParentNavObject != null)
        {
            controller.UIManager.CreateDialogueOptionButton("PARENT", ActiveNavObject.ParentNavObject.GetComponent<WorldNavObject>().buttonTextReturned);
        }
        else 
        {
            controller.UIManager.CreateDialogueOptionButton("MAP", "Return to map.");
        }
        
        controller.UIManager.initConfirmActionButton();
    }

    public void NavigateToNavObject(string GUID)
    {
        Debug.Log("WORLD NAVIGATOR ---- Navigating to next nav object.");
        // remove listener
        UIManager.onOptionSelected -= NavigateToNavObject;
        switch(GUID)
        {
            case "MAP":
                Debug.Log("This would return you to the MAP UI.");
            break;
            case "PARENT":
                NavigateToParent();
            break;
            default:
                NavigateToChild(GUID);
            break;
        }
    }

    public void NavigateToChild(string childGUID)
    {
        Debug.Log("WORLD NAVIGATOR ---- Attempting to navigate to this object's child GUID: " + childGUID);
        List<WorldNavObject> childNavObjects = ActiveNavObject.AllChildObjects;
        foreach(WorldNavObject navObject in childNavObjects)
        {
           if(navObject.GUID == childGUID)
           {
               ActiveNavObject.DeactivateNavObject();
               ActiveNavObject = navObject;
               ActiveNavObject.ActivateNavObject();
               OnActiveNavObjectLoaded?.Invoke(null);
               break;
           }
        }
    }

    public void NavigateToParent()
    {
        Debug.Log("WORLD NAVIGATOR ---- Attempting to navigate to this object's parent.");
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
                    OnActiveNavObjectLoaded?.Invoke(null);
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
