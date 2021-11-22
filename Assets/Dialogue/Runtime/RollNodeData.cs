using System;
using System.Collections.Generic;
using UnityEngine;
using globalDataTypes;

[Serializable]
public class RollNodeData
{
    public string nodeGuid;
    public string rollGroupTagID; 
    public string rollSkillName;
    public string rollDescription;
    public string rollDifficulty;
    public string passedDescription; // non-unique - shared across all nodes within that grouptagID once passed.

    public bool isRepeatable;  

     // holds list of modifier tags that can affect roll difficulty (not implemented yet in graph)
    public List<string> modifierTags = new List<string>();
}
