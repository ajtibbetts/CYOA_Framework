using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class EventNode : DialogueNode
{
    public string EventName;
    public string EventValue;
    public bool isRepeatable = false; 
    public bool hasFired = false; 
    public eventType eventType;
}
