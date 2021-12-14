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
    [SerializeField] private string doorTravelText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ActivateInteractable()
    {
        base.ActivateInteractable();
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

    public override void ActivateNoDialogueAction()
    {
        if(destinationObject != null)
            SwitchToNavObject();
        else if (destinationLevelName != null || destinationLevelName.Length < 1)
            SwitchToLevel();

    }

    private void SwitchToNavObject()
    {
        if(destinationObject != null)
        {
            addDoorExitText();
            WorldNavigator.NavigateToNewWorldNavObject(destinationObject);
        }
        else
        {
            Debug.Log("DOOR ERROR ---- NO DESTINATION OBJECT SET. CANNOT SWITCH TO NAV OBJECT.");
        }
    }

    private void SwitchToLevel()
    {
        if(destinationLevelName != null || destinationLevelName.Length < 1)
        {
            addDoorExitText();
            DeactivateInteractable();
            gameController.Instance.SwitchToLevel(destinationLevelName);
        }
        else 
        {
            Debug.Log("DOOR ERROR ---- NO DESTINATION OBJECT SET. CANNOT LOAD NEW LEVEL.");
        }
    }

    private void addDoorExitText()
    {
        if(doorTravelText == null) return;
        else if(doorTravelText.Length > 1) UIManager.Instance.addGenericMessageToText(doorTravelText);
    }
}
