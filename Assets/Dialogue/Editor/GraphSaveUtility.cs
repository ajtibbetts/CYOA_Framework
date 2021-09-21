using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;


public class GraphSaveUtility
{

    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

    
    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        
        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        if(!SaveNodes(dialogueContainer)) return;
        SaveExposedProperties(dialogueContainer);

        // check for existing filename
        if(System.IO.File.Exists($"{Application.dataPath}/Resources/{fileName}.asset"))
        {
            var overwrite = EditorUtility.DisplayDialog("Filename already exists!",$"Target filename: {fileName} already exists. Are you sure you want to overwrite?","Yes","No");
            if(!overwrite)
            {
                return;
            }
        }
        else
        {
            Debug.Log("File name does not exist. Creating new file at location: " + Application.dataPath);
        }

        // auto creates resources folder if it doesn't exist
        if(!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets","Resources");

        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();

    
    }

    private bool SaveNodes(DialogueContainer dialogueContainer)
    {
        if(!Edges.Any()) return false; // if there are no connections (edges) then return;
        
        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            var inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID
            });

        }

        foreach (var dialogueNode in Nodes.Where(node=>!node.EntryPoint))
        {
            // store node data
            dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
            {
                Guid = dialogueNode.GUID,
                DialogueText = dialogueNode.DialogueText,
                Position = dialogueNode.GetPosition().position
            });

            // check if this node is event node and add data to save
            if(dialogueNode.DialogueText == "Event Node")
            {
                var eventNode = (EventNode) dialogueNode;
                dialogueContainer.EventNodeData.Add(new EventNodeData
                {
                    nodeGuid = dialogueNode.GUID,
                    EventName = eventNode.EventName,
                    EventValue = eventNode.EventValue,
                    isRepeatable = eventNode.isRepeatable,
                    hasFired = eventNode.hasFired
                });

            }

            // Check this nodes ports for any skill checks to save
            foreach (Port outputPort in dialogueNode.outputContainer.Children())
            {
                if(outputPort.portName.Contains("Chk.Pass"))
                {
                    var _passPort = outputPort;
                    var _failPort = ((Port)dialogueNode.outputContainer.ElementAt(dialogueNode.outputContainer.IndexOf(outputPort) + 1));
                    var _content = ((TextField)_passPort.contentContainer.ElementAt(3)).value;
                    var _skillName = ((TextField)_failPort.contentContainer.ElementAt(2)).value;
                    var _checkValue = ((TextField)_failPort.contentContainer.ElementAt(4)).value;
   
                    dialogueContainer.DialogueCheckData.Add(new DialogueCheckData
                    {
                        BaseNodeGuid = dialogueNode.GUID,
                        passPortName = _passPort.portName,
                        failPortName = _failPort.portName,
                        content = _content,
                        skillName = _skillName,
                        checkValue = _checkValue
                    });   
                }
            }
        }

        return true;
    }

    private void SaveExposedProperties(DialogueContainer dialogueContainer)
    {
        dialogueContainer.ExposedProperties.AddRange(_targetGraphView.ExposedProperties);
    }

    public void LoadGraph(string fileName)
    {
        _containerCache = Resources.Load<DialogueContainer>(fileName);
        if(_containerCache == null)
        {
            EditorUtility.DisplayDialog("File not Found!","Target dialogue graph file does not exist!","OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
        CreateExposedProperties();


    }

    private void ClearGraph()
    {
        // Set entry point's guid back from the save. Discard existing guid.
        Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGuid;

        foreach(var node in Nodes)
        {
            if(node.EntryPoint) continue;

            // remove edges that connected to this node
            Edges.Where(x => x.input.node==node).ToList() // list <Edge>
            .ForEach(edge => _targetGraphView.RemoveElement(edge));

            // then remove the node
            _targetGraphView.RemoveElement(node);

        }

    }

    private void CreateNodes()
    {

        foreach(var nodeData in _containerCache.DialogueNodeData)
        {
            
            // first check for any event nodes
            if(nodeData.DialogueText == "Event Node")
            {
                var _eventNodeData = _containerCache.EventNodeData.FirstOrDefault(x => x.nodeGuid == nodeData.Guid);
                if(_eventNodeData != null)
                {
                    var tempNode = _targetGraphView.CreateEventNode(nodeData.DialogueText, Vector2.zero, 
                        _eventNodeData.EventName, _eventNodeData.EventValue, _eventNodeData.isRepeatable, _eventNodeData.hasFired);
                    tempNode.GUID = nodeData.Guid;
                    _targetGraphView.AddElement(tempNode);
                }
            }
            else
            {

            
                // we set position later, so send vec2 zero for now
                var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueText, Vector2.zero);
                tempNode.GUID = nodeData.Guid;
                _targetGraphView.AddElement(tempNode);

                var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
                nodePorts.ForEach(x => 
                {
                    // check for any skill check ports
                    if(x.PortName.Contains("Chk.Pass"))
                    {
                        var _checkData = _containerCache.DialogueCheckData.FirstOrDefault(y => y.passPortName == x.PortName && y.BaseNodeGuid == nodeData.Guid);
                        if(_checkData != null)
                        {
                            // create check ports
                            _targetGraphView.AddCheckPorts(tempNode, 
                                _checkData.content, _checkData.skillName, _checkData.checkValue);
                        }
                    }
                    else if (x.PortName.Contains("Chk.Fail"))
                    {
                    // do nothing / skip;
                    }
                    else {
                        _targetGraphView.AddChoicePort(tempNode,x.PortName);
                    }
                    
                });
            }

        }


    }

    private void ConnectNodes()
    {

        for(var i = 0; i < Nodes.Count; i++)
        {
            var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid==Nodes[i].GUID).ToList();
            for(var j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                targetNode.SetPosition(new Rect( 
                    _containerCache.DialogueNodeData.First(x => x.Guid == targetNodeGuid).Position,
                    _targetGraphView.defaultNodeSize
                ));
            }
        }


    }

    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };

        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);
    }

    private void CreateExposedProperties()
    {
        // clear existing properties on hot reload
        _targetGraphView.ClearBlackBoardAndExposedProperties();

        // add properties from data
        foreach (var exposedProperty in _containerCache.ExposedProperties)
        {
            _targetGraphView.AddPropertyToBlackboard(exposedProperty);
        }
    }


}
