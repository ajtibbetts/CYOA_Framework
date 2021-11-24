using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using globalDataTypes;
using CaseDataObjects;

public class WorldNavigator : MonoBehaviour
{
    private static WorldNavigator _instance;
    public static WorldNavigator Instance { get { return _instance; } }

    [HideInInspector] public gameController controller;

    public static WorldNavObject ActiveWorldNavObject {get; private set;}
    private NavObject _activeNavObject;

    // events
    public static event Action<string> OnActiveNavObjectLoaded;
    public static event Action<DialogueContainer> OnNavInteractableLoaded;
    public static event Action OnNewNavObjectSet;
    

    private void Awake() {
        //init singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        controller = GetComponent<gameController>();          
        SubSceneManager.OnSceneLoaded += SetupNewArea;
        Debug.Log("WORLD NAVIGATOR ---- World Navigation manager setup.");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public static void NavigateToNewWorldNavObject(WorldNavObject navObject)
    {
        ActiveWorldNavObject.DeactivateNavObject();
        ActiveWorldNavObject = navObject;
        // _activeNavObject = navObject.GetComponent<NavObject>();
        ActiveWorldNavObject.ActivateNavObject();
        OnActiveNavObjectLoaded?.Invoke(null);
        OnNewNavObjectSet?.Invoke();
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
            ActiveWorldNavObject = rootObject.GetComponent<WorldNavObject>();
            ActiveWorldNavObject.ActivateNavObject();

            OnActiveNavObjectLoaded?.Invoke(sceneName);
        }

    }

 
    public DialogueContainer GetActiveDialogue()
    {
        Debug.Log($"WORLD NAVIGATOR ---- Checking active nav object name: {ActiveWorldNavObject.Name} for new or returning dialogue graph.");
        return ActiveWorldNavObject.GetNewOrReturnedDialogue();
    }

    public string GetActiveNavObjectProperty(string propertyName)
    {
        if(_activeNavObject != null)
        {
            return _activeNavObject.GetLocalPropertyValue(propertyName);
        }
        else 
        {
            Debug.LogError($"WORLD NAVIGATOR ---- Tried to get property name {propertyName} but activeNavObject is null");
            return null;
        }
    }

    public CharacterProfileData GetActiveNPCData()
    {
        try
        {
            var serializedParent = Newtonsoft.Json.JsonConvert.SerializeObject(_activeNavObject); 
            interactableNPC npcObject  = Newtonsoft.Json.JsonConvert.DeserializeObject<interactableNPC>(serializedParent);
            return npcObject.GetProfileData();
        }
        catch (Exception e)
        {
            Debug.LogError("WORLD NAVIGATIOR ---- FAILED TO CAST NAV OBJECT TO NPC. Exception displays: " + e);
            return null;
        }
    }

    public void DisplayActiveNavObject()
    {
        // register event listener
        UIManager.onOptionSelected -= NavigateToNavObject; // remove extra listener if any
        UIManager.onOptionSelected += NavigateToNavObject;
        
        Debug.Log($"WORLD NAVIGATOR ---- Displaying active world nav object: {ActiveWorldNavObject.Name}");
        _activeNavObject = ActiveWorldNavObject;

        // clear existing buttons first
        controller.UIManager.ClearContentAndButtons();

        // displays new or returned text
        DisplayActiveNavObjectText();

        // display all toggle buttons for this active nav object (new/returned/parent)
        DisplayActiveNavObjectOptions();

        // add object to player data
        ActiveWorldNavObject.AddNavObjectToPlayer();
        // AddActiveNavObjectToPlayer();
    }

    public void DisplayActiveNavObjectText()
    {
        // string displayText = GetNewOrReturnedText(ActiveNavObject, ActiveNavObject.displayTextOnNew, ActiveNavObject.displayTextOnReturn);
        string displayText = ActiveWorldNavObject.GetNewOrReturnedText(ActiveWorldNavObject.displayTextOnNew, ActiveWorldNavObject.displayTextOnReturn);
        // controller.UIManager.updateContentText(displayText);
        controller.UIManager.CreateContentParagraph(displayText);
    }

    public void DisplayActiveNavObjectOptions()
    {
        // list all child nav objects as options
        List<WorldNavObject> childNavObjects = ActiveWorldNavObject.ChildNavObjects;
        foreach(WorldNavObject navObject in childNavObjects)
        {
            // string buttonDisplayText = GetNewOrReturnedText(navObject, navObject.buttonTextOnNew, navObject.buttonTextOnReturn);
            string buttonDisplayText = navObject.GetNewOrReturnedText(navObject.buttonTextOnNew, navObject.buttonTextOnReturn);
            controller.UIManager.CreateDialogueOptionButton(navObject.GUID, buttonDisplayText);
        }

        // list all interactable child objects
        List<Interactable> childInteractables = ActiveWorldNavObject.ChildInteractiveObjects;
        foreach(Interactable interactableObject in childInteractables)
        {
            string buttonDisplayText = interactableObject.GetNewOrReturnedText(interactableObject.HasPlayerVisitedNavObject());
            controller.UIManager.CreateDialogueOptionButton(interactableObject.GUID, buttonDisplayText);
        }

        // add choice to go back to parent or map if no parent
        if(ActiveWorldNavObject.ParentNavObject != null)
        {
            WorldNavObject parentNavObject = ActiveWorldNavObject.ParentNavObject.GetComponent<WorldNavObject>();
            string buttonDisplayText = conditionManager.GetConditionalText(parentNavObject.buttonTextAsParent);
            controller.UIManager.CreateDialogueOptionButton("PARENT", buttonDisplayText);
        }
        else 
        {
            // remove map creation for now, maybe make separate button
            //controller.UIManager.CreateDialogueOptionButton("MAP", "Return to map.");
        }
        
        controller.UIManager.initConfirmActionButton();

        
    }

    
    public void NavigateToNavObject(string GUID)
    {
        Debug.Log("WORLD NAVIGATOR ---- Navigating to next nav object.");
        // remove listener
        UIManager.onOptionSelected -= NavigateToNavObject;
        // // add object to player data
        // AddActiveNavObjectToPlayer();
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
        List<WorldNavObject> childNavObjects = ActiveWorldNavObject.ChildNavObjects;
        foreach(WorldNavObject navObject in childNavObjects)
        {
           if(navObject.GUID == childGUID)
           {
            //    ActiveNavObject.DeactivateNavObject();
            //    ActiveNavObject = navObject;
            //    ActiveNavObject.ActivateNavObject();
            //    OnActiveNavObjectLoaded?.Invoke(null);
                NavigateToNewWorldNavObject(navObject);
                return;
           }
        }

        // if not a nav object, will be interactable
        List<Interactable> childInteractables = ActiveWorldNavObject.ChildInteractiveObjects;
        foreach(Interactable interactiveObject in childInteractables)
        {
            if(interactiveObject.GUID == childGUID)
            {
                // ActiveNavObject.DeactivateNavObject(); // deactivate active nav object event
                ActiveWorldNavObject.StopPropertyListener(); // disable so duplicate properties aren't added.
                interactiveObject.ActivateNavObject();
                _activeNavObject = interactiveObject;
                OnNavInteractableLoaded?.Invoke(interactiveObject.interactiveDialogue);
                interactiveObject.AddNavObjectToPlayer();
                return;
            }
        }
    }

    public void NavigateToParent()
    {
        Debug.Log("WORLD NAVIGATOR ---- Attempting to navigate to this object's parent.");
        if(ActiveWorldNavObject !=null)
        {
            if(ActiveWorldNavObject.ParentNavObject != null)
            {
                var destinationNavObject = ActiveWorldNavObject.ParentNavObject.GetComponent<WorldNavObject>();
                if(destinationNavObject != null)
                {
                    // ActiveNavObject.DeactivateNavObject();
                    // ActiveNavObject = destinationNavObject;
                    // ActiveNavObject.ActivateNavObject();
                    // OnActiveNavObjectLoaded?.Invoke(null);
                    NavigateToNewWorldNavObject(destinationNavObject);
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
