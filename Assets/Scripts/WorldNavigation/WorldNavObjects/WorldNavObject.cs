using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using globalDataTypes;


public class WorldNavObject : NavObject
{
    
    // structs
    
    [Serializable]
    public struct conditionalUnityEvent {
        public conditionalStatement[] conditions;
        public UnityEvent eventToTrigger;
    }

    // vars
    
    [Header("World Navigation")]
    public List<WorldNavObject> ChildNavObjects = new List<WorldNavObject>();
    public List<Interactable> ChildInteractiveObjects = new List<Interactable>(); 

    [Header("Dialogue before Display Text")]
    public conditionalDialogue[] dialogueOnNew;
    public conditionalDialogue[] dialogueOnReturn;
    public conditionalDialogue[] dialogueOnExit;

    [Header("Display Text when Active")]
    public conditionalText[] displayTextOnNew;
    public conditionalText[] displayTextOnReturn;
    public conditionalText[] displayTextOnExit;

    [Header("As Option Button")]
    public conditionalText[] buttonTextOnNew;
    public conditionalText[] buttonTextOnReturn;
    public conditionalText[] buttonTextAsParent;
    

    // events
    

    // stores all nav objects in loaded scene
    public static List<WorldNavObject> AllWorldObjects {get; private set;} 

    


    //public static event Action OnObjectsLoaded;

    public override void Awake() {
        base.Awake();
        SubSceneManager.OnSceneMarkedForUnload += PrepareForUnload;
        //Debug.Log("World nav object loaded: " + gameObject.name);
    }
    
    private void OnEnable()
    {
        if(AllWorldObjects == null) AllWorldObjects = new List<WorldNavObject>();

        AllWorldObjects.Add(this);
        if(transform.parent != null)
        {
            // Debug.Log($"This world nav object's name: {Name}" );
            ParentNavObject = transform.parent.gameObject;
        }
    }

    private void OnDisable() {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PrepareForUnload(string sceneName)
    {
        AllWorldObjects.Remove(this);
        SubSceneManager.OnSceneMarkedForUnload -= PrepareForUnload;
    }

    public override void ActivateNavObject()
    {
        base.ActivateNavObject();
        SetupChildObjects();
    }

    public override void ActivateNoDialogueAction()
    {
        

    }

    // public override void AddNavObjectToPlayer()
    // {
    //     base.AddNavObjectToPlayer();
    //     // add active nav object to player's data if not already in there
    //     if(!Player.Instance.visitedWorldNavObjects.Contains(GUID))
    //     {
    //         Player.Instance.visitedWorldNavObjects.Add(GUID);
    //     }
    // }

    // public override bool HasPlayerVisitedNavObject()
    // {
    //     return Player.Instance.visitedWorldNavObjects.Contains(GUID);
    // }

    public override void DeactivateNavObject()
    {
        base.DeactivateNavObject();
        // clear list to free memory
        ChildNavObjects.Clear();
        ChildInteractiveObjects.Clear();        
    }

    
    public void SetupChildObjects()
    {
        ChildNavObjects.Clear();
        ChildInteractiveObjects.Clear();

        foreach (Transform child in transform)
        {
            // cache child world nav objects / interactables seprately
            if(child.GetComponent<WorldNavObject>() != null) ChildNavObjects.Add(child.GetComponent<WorldNavObject>());
            if(child.GetComponent<Interactable>() != null) ChildInteractiveObjects.Add(child.GetComponent<Interactable>());
        }
    }

    
    public DialogueContainer GetNewOrReturnedDialogue()
    {
        // used only on the activeNavObject
        DialogueContainer dialogueToDisplay = null;
        if(HasPlayerVisitedNavObject())
        {
            // Debug.Log($"WORLD NAVIGATOR ---- PLAYER HAS VISITED THIS NAV OBJECT");
            if(dialogueOnReturn.Length > 0) dialogueToDisplay = conditionManager.GetConditionalDialogue(dialogueOnReturn);
            
        }
        else 
        {
            // Debug.Log($"WORLD NAVIGATOR ---- PLAYER HAS NOT VISITED THIS NAV OBJECT");
            if(dialogueOnNew.Length > 0) dialogueToDisplay = conditionManager.GetConditionalDialogue(dialogueOnNew);
            else
            {
                // if no attached new dialogue, check for a returned dialogue (treated as default)
                if(dialogueOnReturn.Length > 0) dialogueToDisplay = conditionManager.GetConditionalDialogue(dialogueOnReturn);
            }
        }  
        // otherwise return null
        return dialogueToDisplay;
    }

    public string GetNewOrReturnedText(conditionalText[] newConditions, conditionalText[] returningConditions)
    {
        // used for both buttons and display text
        string displayText = "MISSING";
        if(HasPlayerVisitedNavObject())
        {
            // Debug.Log($"WORLD NAVIGATOR ---- PLAYER HAS VISITED THIS NAV OBJECT");
            if(returningConditions.Length > 0)
            {
                displayText = conditionManager.GetConditionalText(returningConditions);
            }
            else
            {
                Debug.Log($"WORLD NAVIGATOR ---- THIS NAV OBJECT DOES NOT HAVE RETURN TEXT SET: {Name}");
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
                Debug.Log($"WORLD NAVIGATOR ---- THIS NAV OBJECT DOES NOT HAVE NEW TEXT SET: {Name}");
            }
        }  

        return displayText;
    }

    
}
