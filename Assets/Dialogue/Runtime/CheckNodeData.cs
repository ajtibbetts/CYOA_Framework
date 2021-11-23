using System;
using UnityEngine;
using globalDataTypes;

[Serializable]
public class CheckNodeData
{
    public string nodeGuid;
    public conditionType checkType;
    public string checkName;
    public string checkValue;
    public string comparisonOperator;
}
