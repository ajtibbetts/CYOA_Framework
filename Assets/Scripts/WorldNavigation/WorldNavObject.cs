using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


public class WorldNavObject : MonoBehaviour
{
    
    // structs
    

    // [Serializable]
    // public struct conditionalText {
    //     public conditionalStatement[] conditions;
    //     [TextArea]
    //     public string displayText;
    // }

    [Serializable]
    public struct conditionalUnityEvent {
        public conditionalStatement[] conditions;
        public UnityEvent eventToTrigger;
    }

    // vars
    [Header("Conditions as Child")]
    public conditionalStatement[] conditionsToEnable;
    [Header("Navigation")]
    public string Name;
    public string GUID = Guid.NewGuid().ToString();
    public GameObject ParentNavObject;
    public List<WorldNavObject> AllChildObjects = new List<WorldNavObject>();
    public List<GameObject> ChildInteractiveObjects = new List<GameObject>(); // update list <T> to InteractiveObject once abstract structure is setup

    // dialogue stuff for this nav object
        // dialogue to activate if set
    [Header("Dialogue before Display Text")]
    // [SerializeField] public DialogueContainer dialogue; // remove this once all other dialogue systems are integrated
    public conditionalDialogue[] dialogueOnNew;
    public conditionalDialogue[] dialogueOnReturn;
    public conditionalDialogue[] dialogueOnExit;

    [Header("Display Text when Active")]
    // [TextArea]
    // [Tooltip("Text to display first time player navigates to this object.")]
    // public string descriptionNew;
    // [TextArea]
    // [Tooltip("Text to display subsequent times this player returns to this object.")]
    // public string descriptionReturned;
    // [TextArea]
    // [Tooltip("Text to display when the player navigates away from this object.")]
    // public string descriptionExit;
    public conditionalText[] displayTextOnNew;
    public conditionalText[] displayTextOnReturn;
    public conditionalText[] displayTextOnExit;

    [Header("Button Text")]
    [Header("As Option Button")]
    // text to display if this nav is an optional button for an activated parent
    // [Tooltip("Button text to display first time player sees this object as an option.")]
    // public string buttonTextNew;
    // [Tooltip("Button text to display every other time player sees this object as an option.")]
    // public string buttonTextReturned;
    public conditionalText[] buttonTextOnNew;
    public conditionalText[] buttonTextOnReturn;
    public conditionalText[] buttonTextAsParent;
    

    // events
    [Header("Events")]
    [SerializeField] private UnityEvent OnNavObjectActivated;
    [SerializeField] private UnityEvent OnNavObjectDeactivated;
    public conditionalUnityEvent[] conditionalEvents;

    // stores all nav objects in loaded scene
    public static List<WorldNavObject> AllWorldObjects {get; private set;} 

    // context creation objects
    [ContextMenu ("Generate new GUID")]
    void GenerateNewGUID () {
        Debug.Log ("Creating new GUID for this object.");
        GUID = Guid.NewGuid().ToString();
    }


    //public static event Action OnObjectsLoaded;

    private void Awake() {
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

    public void ActivateNavObject()
    {
        SetupChildObjects();
        OnNavObjectActivated.Invoke();
    }

    public void DeactivateNavObject()
    {
        // clear list to free memory
        AllChildObjects.Clear();
        OnNavObjectDeactivated.Invoke();
    }

    
    public void SetupChildObjects()
    {
        foreach (Transform child in transform)
        {
            // Debug.Log("child name: " + child.name);
            var navObject = child.GetComponent<WorldNavObject>();
            if(navObject != null)
            {
                AllChildObjects.Add(navObject);
                // Debug.Log("World nav child name: " + navObject.Name);
            }
        }
    }

    public string GetConditionalText(conditionalText[] conditionalTexts)
    {   
        // return first matched text that meets all conditions in its set
        foreach(conditionalText conditionText in conditionalTexts)
        {
            // if there are any conditions, check to ensure all are met
            if(conditionText.conditions.Length > 0)
            {
                bool allConditionsMet = true;

                // ensure each condition is met, flag if any single is unmet
                for(int i = 0; i < conditionText.conditions.Length; i++)
                {
                    
                    if(!conditionManager.isConditionMet(conditionText.conditions[i]))
                    {
                        allConditionsMet = false;
                    }
                }
                // return only if all conditions are met
                if(allConditionsMet) return conditionText.displayText;
            }
            else
            {
                return conditionText.displayText;
            }
        }

        // return null if nothing is met
        return null;
    }

    public DialogueContainer GetConditionalDialogue(conditionalDialogue[] conditionalDialogues)
    {
        // return first matched dialogue that meets all conditions in its set
        foreach(conditionalDialogue conditionDialogue in conditionalDialogues)
        {
            // if there are any conditions, check to ensure all are met
            if(conditionDialogue.conditions.Length > 0)
            {
                bool allConditionsMet = true;

                // ensure each condition is met, flag if any single is unmet
                for(int i = 0; i < conditionDialogue.conditions.Length; i++)
                {
                    
                    if(!conditionManager.isConditionMet(conditionDialogue.conditions[i]))
                    {
                        allConditionsMet = false;
                    }
                }
                // return only if all conditions are met
                if(allConditionsMet) return conditionDialogue.dialogue;
            }
            else
            {
                return conditionDialogue.dialogue;
            }
        }

        // return null if nothing is met
        return null;
    }
}
