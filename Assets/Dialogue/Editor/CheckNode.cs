using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class CheckNode : DialogueNode
{
    public conditionType checkType;

    public string checkName;
    public string checkValue;
    public bool isRepeatable = true;  
    public bool isRollable = false;  
    public bool alreadyPassed = false;

    public void setType(conditionType type)
    {
        checkType = type;
    }
}
