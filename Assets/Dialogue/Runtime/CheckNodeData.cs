using System;
using UnityEngine;

[Serializable]
public class CheckNodeData
{
    public string nodeGuid;
    public conditionType checkType;
    public string checkName;
    public string checkValue;
    public bool isRepeatable;  
    public bool isRollable;  
    public bool alreadyPassed;
}
