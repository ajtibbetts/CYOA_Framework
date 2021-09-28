using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
    public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    public List<DialogueCheckData> DialogueCheckData = new List<DialogueCheckData>();
    public List<EventNodeData> EventNodeData = new List<EventNodeData>();
    public List<CheckNodeData> CheckNodeData = new List<CheckNodeData>();
    public List<EndpointNodeData> EndpointNodeData = new List<EndpointNodeData>();

}
