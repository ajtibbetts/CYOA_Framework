using System;
using UnityEngine;

[Serializable]
public class EventNodeData
{
    public string nodeGuid;
    public string EventName;
    public string EventValue;
    public bool isRepeatable; 
    public bool hasFired; 
}
