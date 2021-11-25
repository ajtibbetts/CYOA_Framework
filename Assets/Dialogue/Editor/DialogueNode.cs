using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using globalDataTypes;


public class DialogueNode : Node
{
    public string GUID;

    public string characterID = null;

    public string DialogueText;

    public bool EntryPoint = false;

    public nodeType nodeType;
}
