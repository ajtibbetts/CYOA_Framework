using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactableDoor : Interactable
{
    [Header("Door")]
    [SerializeField]
    private WorldNavObject destinationObject;
    [SerializeField]
    private string destinationLevelName;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ActivateInteractable(gameController controller)
    {
        base.ActivateInteractable(controller);
        WorldNavigator.OnNewNavObjectSet += DeactivateInteractable;
    }

    public override void DeactivateInteractable()
    {
        base.DeactivateInteractable();
        WorldNavigator.OnNewNavObjectSet -= DeactivateInteractable;
    }

    public override void ProcessLocalEvent(string eventName)
    {
        Debug.Log("Activating local event on this door: " + eventName);
        switch(eventName)
        {
            case "door":
                SwitchToNavObject();
            break;
            case "level":
                SwitchToLevel();
            break;
            default:
            break;
        }
    }

    private void SwitchToNavObject()
    {
        WorldNavigator.NavigateToNewWorldNavObject(destinationObject);
    }

    private void SwitchToLevel()
    {

    }
}
