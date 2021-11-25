using System;
using UnityEngine;
using globalDataTypes;

[Serializable]
public class DialogueNodeData 
{
    public string Guid;
    public string characterID = null;
    public string DialogueText;
    public Vector2 Position;
    public nodeType nodeType;
}
