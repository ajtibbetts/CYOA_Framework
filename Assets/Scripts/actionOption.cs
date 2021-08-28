using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class actionOption
{
    [System.Serializable]
    public struct actionEvent {
        public CYOA_Event eventToTrigger;
        public eventParams parameters;
    }
    [Tooltip("Default text to display on this action option button.")]
    public string buttonText;
    [Tooltip("Text to be added to the content on next page after option is selected and confirmed.")]
    public string actionTakenText;
    [Tooltip("Default room that taking this action will move player to.")]
    public Room valueRoom;
    [Tooltip("Optional conditional requirement that must be met for this action to be taken.")]
    public actionOptionCondition conditionalRequirement;
    [Tooltip("Action event to fire off if this action is taken.")]
    public actionEvent eventData;
    
}


