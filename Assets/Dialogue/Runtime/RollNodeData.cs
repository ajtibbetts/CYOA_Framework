using System;
using System.Collections.Generic;
using UnityEngine;
using globalDataTypes;

[Serializable]
public class RollNodeData
{
    public string nodeGuid;
    public string rollSkillName;
    public string rollDifficulty;
    public bool isRepeatable;  

     // holds list of modifier tags that can affect roll difficulty (not implemented yet in graph)
    public List<string> modifierTags = new List<string>();
}
