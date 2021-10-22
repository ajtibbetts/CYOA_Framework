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
    public List<WorldNavObject> ChildNavObjects = new List<WorldNavObject>();
    public List<Interactable> ChildInteractiveObjects = new List<Interactable>(); // update list <T> to InteractiveObject once abstract structure is setup

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
        ChildNavObjects.Clear();
        ChildInteractiveObjects.Clear();
        OnNavObjectDeactivated.Invoke();
    }

    
    public void SetupChildObjects()
    {
        foreach (Transform child in transform)
        {
            // cache child world nav objects / interactables seprately
            if(child.GetComponent<WorldNavObject>() != null) ChildNavObjects.Add(child.GetComponent<WorldNavObject>());
            if(child.GetComponent<Interactable>() != null) ChildInteractiveObjects.Add(child.GetComponent<Interactable>());
        }
    }

    

    
}
