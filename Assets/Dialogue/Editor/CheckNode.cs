using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using globalDataTypes;

public class CheckNode : DialogueNode
{
    public conditionType checkType;

    public string checkName;
    public string checkValue;
    public string comparisonOperator;

    public void setType(conditionType type)
    {
        checkType = type;
    }
}
