using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldNavigator : MonoBehaviour
{
    [HideInInspector] public gameController controller;

    public static WorldNavObject ActiveNavObject {get; private set;}

    // events
    public static event Action<string> OnActiveNavObjectLoaded;
    public static event Action<DialogueContainer> OnNavInteractableLoaded;
    public static event Action OnNewNavObjectSet;
    

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

    public static void NavigateToNewWorldNavObject(WorldNavObject navObject)
    {
        ActiveNavObject.DeactivateNavObject();
        ActiveNavObject = navObject;
        ActiveNavObject.ActivateNavObject();
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
            ActiveNavObject = rootObject.GetComponent<WorldNavObject>();
            ActiveNavObject.ActivateNavObject();
            OnActiveNavObjectLoaded?.Invoke(sceneName);
        }

    }

    public void AddActiveNavObjectToPlayer()
    {
        // add active nav object to player's data if not already in there
        if(!controller.player._player.visitedWorldNavObjects.Contains(ActiveNavObject.GUID))
        {
            controller.player._player.visitedWorldNavObjects.Add(ActiveNavObject.GUID);
        }
    }

    public bool HasPlayerVisitedNavObject(string objectGUID)
    {
        return controller.player._player.visitedWorldNavObjects.Contains(objectGUID);
    }

    public DialogueContainer GetActiveDialogue()
    {
        Debug.Log($"WORLD NAVIGATOR ---- Checking active nav object name: {ActiveNavObject.Name} for new or returning dialogue graph.");
        return GetNewOrReturnedDialogue();
    }

    public void DisplayActiveNavObject()
    {
        // register event listener
        UIManager.onOptionSelected += NavigateToNavObject;
        
        Debug.Log($"WORLD NAVIGATOR ---- Displaying active world nav object: {ActiveNavObject.Name}");

        // displays new or returned text
        DisplayActiveNavObjectText();

        // display all toggle buttons for this active nav object (new/returned/parent)
        DisplayActiveNavObjectOptions();
    }

    public void DisplayActiveNavObjectText()
    {
        string displayText = GetNewOrReturnedText(ActiveNavObject, ActiveNavObject.displayTextOnNew, ActiveNavObject.displayTextOnReturn);
        controller.UIManager.updateContentText(displayText);
    }

    public void DisplayActiveNavObjectOptions()
    {
        // clear existing buttons first
        controller.UIManager.ClearButtons();

        // list all child nav objects as options
        List<WorldNavObject> childNavObjects = ActiveNavObject.ChildNavObjects;
        foreach(WorldNavObject navObject in childNavObjects)
        {
            string buttonDisplayText = GetNewOrReturnedText(navObject, navObject.buttonTextOnNew, navObject.buttonTextOnReturn);
            controller.UIManager.CreateDialogueOptionButton(navObject.GUID, buttonDisplayText);
        }

        // list all interactable child objects
        List<Interactable> childInteractables = ActiveNavObject.ChildInteractiveObjects;
        foreach(Interactable interactableObject in childInteractables)
        {
            string buttonDisplayText = interactableObject.GetNewOrReturnedText(interactableObject.HasPlayerInteracted(controller));
            controller.UIManager.CreateDialogueOptionButton(interactableObject.GUID, buttonDisplayText);
        }

        // add choice to go back to parent or map if no parent
        if(ActiveNavObject.ParentNavObject != null)
        {
            WorldNavObject parentNavObject = ActiveNavObject.ParentNavObject.GetComponent<WorldNavObject>();
            string buttonDisplayText = conditionManager.GetConditionalText(parentNavObject.buttonTextAsParent);
            controller.UIManager.CreateDialogueOptionButton("PARENT", buttonDisplayText);
        }
        else 
        {
            controller.UIManager.CreateDialogueOptionButton("MAP", "Return to map.");
        }
        
        controller.UIManager.initConfirmActionButton();
    }

    public string GetNewOrReturnedText(WorldNavObject navObject, conditionalText[] newConditions, conditionalText[] returningConditions)
    {
        // used for both buttons and display text
        string displayText = "MISSING";
        if(HasPlayerVisitedNavObject(navObject.GUID))
        {
            // Debug.Log($"WORLD NAVIGATOR ---- PLAYER HAS VISITED THIS NAV OBJECT");
            if(returningConditions.Length > 0)
            {
                displayText = conditionManager.GetConditionalText(returningConditions);
            }
            else
            {
                Debug.Log($"WORLD NAVIGATOR ---- THIS NAV OBJECT DOES NOT HAVE RETURN TEXT SET: {navObject.Name}");
            }

        }
        else 
        {
            // Debug.Log($"WORLD NAVIGATOR ---- PLAYER HAS NOT VISITED THIS NAV OBJECT");
            if(newConditions.Length > 0)
            {
                displayText = conditionManager.GetConditionalText(newConditions);
            }
            else
            {
                Debug.Log($"WORLD NAVIGATOR ---- THIS NAV OBJECT DOES NOT HAVE NEW TEXT SET: {navObject.Name}");
            }
        }  

        return displayText;
    }

    public DialogueContainer GetNewOrReturnedDialogue()
    {
        // used only on the activeNavObject
        DialogueContainer dialogueToDisplay = null;
        if(HasPlayerVisitedNavObject(ActiveNavObject.GUID))
        {
            // Debug.Log($"WORLD NAVIGATOR ---- PLAYER HAS VISITED THIS NAV OBJECT");
            if(ActiveNavObject.dialogueOnReturn.Length > 0) dialogueToDisplay = conditionManager.GetConditionalDialogue(ActiveNavObject.dialogueOnReturn);
        }
        else 
        {
            // Debug.Log($"WORLD NAVIGATOR ---- PLAYER HAS NOT VISITED THIS NAV OBJECT");
            if(ActiveNavObject.dialogueOnNew.Length > 0) dialogueToDisplay = conditionManager.GetConditionalDialogue(ActiveNavObject.dialogueOnNew);
        }  

        return dialogueToDisplay;
    }

    

    

    public void NavigateToNavObject(string GUID)
    {
        Debug.Log("WORLD NAVIGATOR ---- Navigating to next nav object.");
        // remove listener
        UIManager.onOptionSelected -= NavigateToNavObject;
        // add object to player data
        AddActiveNavObjectToPlayer();
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
        List<WorldNavObject> childNavObjects = ActiveNavObject.ChildNavObjects;
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
        List<Interactable> childInteractables = ActiveNavObject.ChildInteractiveObjects;
        foreach(Interactable interactiveObject in childInteractables)
        {
            if(interactiveObject.GUID == childGUID)
            {
                interactiveObject.ActivateInteractable(controller);
                OnNavInteractableLoaded?.Invoke(interactiveObject.interactiveDialogue);
                return;
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
