using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CYOA_EventManager : MonoBehaviour
{
    
    [HideInInspector] public gameController controller;
    
    private void Awake() {
            controller = GetComponent<gameController>();  
    }

    public void Start() {
        RegisterStartingEvents();

    }

    public void RegisterStartingEvents()
    {
        // register for events
        controller.DialogueParser.onEventTriggered += ProcessDialogueEvent;
    }

    public void ProcessDialogueEvent(eventType eventType, string eventName, string eventValue)
    {
        Debug.Log("EVENT MANAGER FIRING EVENT: " + eventName + " VALUE: " + eventValue + " TYPE: " + eventType.ToString());
    }
}
