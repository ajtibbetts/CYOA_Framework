using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldNavObject : MonoBehaviour
{
    

    // identifier data for this nav object
    [Header("Navigation")]
    public string Name;
    public string GUID = Guid.NewGuid().ToString();
    public GameObject ParentNavObject;
    public List<WorldNavObject> AllChildObjects = new List<WorldNavObject>();

    // dialogue stuff for this nav object
        // dialogue to activate if set
    [Header("Dialogue")]
    [SerializeField] public DialogueContainer dialogue;
    // dialogue to display after any Dialogue graph concludes
    [TextArea]
    [Tooltip("Text to display first time player navigates to this object.")]
    public string descriptionNew;
    [TextArea]
    [Tooltip("Text to display subsequent times this player returns to this object.")]
    public string descriptionReturned;
    [TextArea]
    [Tooltip("Text to display when the player navigates away from this object.")]
    public string descriptionExit;
    [Header("As Option Button")]
    // text to display if this nav is an optional button for an activated parent
    [Tooltip("Button text to display first time player sees this object as an option.")]
    public string buttonTextNew;
    [Tooltip("Button text to display every other time player sees this object as an option.")]
    public string buttonTextReturned;

    // events
    [Header("Events")]
    [SerializeField] private UnityEvent OnNavObjectActivated;
    [SerializeField] private UnityEvent OnNavObjectDeactivated;

    // stores all nav objects in loaded scene
    public static List<WorldNavObject> AllWorldObjects {get; private set;} 

    // events

    public static event Action OnObjectsLoaded;

    private void Awake() {
        SubSceneManager.OnSceneMarkedForUnload += PrepareForUnload;
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
        Debug.Log("Root object loaded.");
        var rootObject = this.GetComponent<Transform>().root;
        if(rootObject == this.GetComponent<Transform>())
        {
            Debug.Log("Root object loaded. Firing off event.");
            OnObjectsLoaded?.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PrepareForUnload(string sceneName)
    {
        AllWorldObjects.Remove(this);
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
}
