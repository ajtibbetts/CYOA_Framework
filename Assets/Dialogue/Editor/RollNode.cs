using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using globalDataTypes;

public class RollNode : DialogueNode
{
    public string rollSkillName;
    public string rollDifficulty;
    public bool isRepeatable;  

     // holds list of modifier tags that can affect roll difficulty (not implemented yet in graph)
    public List<string> modifierTags = new List<string>();
    
}
