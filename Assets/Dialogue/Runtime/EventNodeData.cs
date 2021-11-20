using System;
using UnityEngine;
using globalDataTypes;

[Serializable]
public class EventNodeData
{
    public string nodeGuid;
    public string EventName;
    public string EventValue;
    public bool isRepeatable; 
    public bool hasFired; 
    public bool ignoreDeadEnd;
    public eventType eventType;
}
