using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Exit
{
    [System.Serializable]
    public struct actionEvent {
        public CYOA_Event eventToTrigger;
        public eventParams parameters;
    }
    public string buttonText;
    public string exitDescription;
    public Room valueRoom;
    public actionEvent eventData;
    
}


