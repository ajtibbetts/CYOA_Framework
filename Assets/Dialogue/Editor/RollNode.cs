using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using globalDataTypes;

public class RollNode : DialogueNode
{
    public string rollGroupTagID; // non-unique/ shared among checks that go towards the same goal
    public string rollSkillName;
    public string rollDescription;
    public string rollDifficulty;
    public string passedDescription; // non-unique - shared across all nodes within that grouptagID once passed.
    public bool isRepeatable;  

     // holds list of modifier tags that can affect roll difficulty (not implemented yet in graph)
    public List<string> modifierTags = new List<string>();
    
}
