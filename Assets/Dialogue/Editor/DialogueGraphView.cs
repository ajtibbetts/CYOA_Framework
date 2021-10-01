using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;

public class DialogueGraphView : GraphView
{
    
    public readonly Vector2 defaultNodeSize = new Vector2(150,200);

    private NodeSearchWindow _searchWindow;

    public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    public Blackboard Blackboard;
    
    public DialogueGraphView(EditorWindow editorWindow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        
        SetupZoom(ContentZoomer.DefaultMinScale,ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());


        var grid = new GridBackground();
        Insert(0,grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
        AddSearchWindow(editorWindow);
    }

    private void AddSearchWindow(EditorWindow editorWindow)
    {
        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _searchWindow.Init(editorWindow, this);
        nodeCreationRequest = context => 
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition),_searchWindow);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach( port =>
        {
            if(startPort!=port && startPort.node!=port.node)
            {
                compatiblePorts.Add(port);
            }

        });

        return compatiblePorts;
    }



    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float)); // abritary float, could be set for w/e
    }

    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENTRYPOINT",
            EntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;


        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100,200,100,150));
        return node;
    }

    public void CreateNode(string nodeName, Vector2 position, string nodePrefix="", string enumType = "")
    {
        switch(nodeName)
        {
            case "Dialogue Node":
                AddElement(CreateDialogueNode(nodeName, position));
            break;
            case "Event Node":
                eventType eventType;
                if(Enum.TryParse(enumType, out eventType))
                {
                    if(Enum.IsDefined(typeof(eventType), eventType) | eventType.ToString().Contains(","))
                    {
                        AddElement(CreateEventNode(nodeName, position, eventType));
                    }
                }

            break;
            case "Check Node":
                conditionType checkType;
                if(Enum.TryParse(enumType, out checkType))
                {
                    if (Enum.IsDefined(typeof(conditionType), checkType) | checkType.ToString().Contains(","))
                    {
                        AddElement(CreateCheckNode(nodeName, position, nodePrefix, checkType));
                    }
                }
            break;
            case "ENDPOINT":
                AddElement(CreateEndpointNode(position));
            break;
            default:
            break;
        }
        
    }

    public EndpointNode CreateEndpointNode(Vector2 position, string _exitText = ""){

        var node = new EndpointNode
        {
            title = "END",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENDPOINT",
            exitText = _exitText,
            nodeType = nodeType.endpointNode
        };

        var generatedPort = GeneratePort(node, Direction.Input);
        generatedPort.portName = "Input";
        node.inputContainer.Add(generatedPort);
        node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        var textField = new TextField(string.Empty){
            multiline = true
        };
        textField.RegisterValueChangedCallback(evt => 
        { 
            node.exitText = evt.newValue;
           // dialogueNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(node.exitText);
        node.mainContainer.Add(new Label("Added flavor text."));
        node.mainContainer.Add(textField);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(position, defaultNodeSize));
        return node;
    }


    public DialogueNode CreateDialogueNode(string dialogueText, Vector2 position)
    {
        var dialogueNode = new DialogueNode
        {
            title = "Dialogue Node",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = dialogueText,
            nodeType = nodeType.dialogueNode
        };

        // add input and style sheet
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);
        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));


        // Add Choice Button
        var button = new Button(()=>{AddChoicePort(dialogueNode);});
        button.text = "+ Choice";
        dialogueNode.titleContainer.Add(button);

        // Add Check Button
        // var button2 = new Button(()=>{AddCheckPorts(dialogueNode);});
        // button2.text = "+ Check";
        // dialogueNode.titleContainer.Add(button2);

        var textField = new TextField(string.Empty){
            multiline = true
        };
        textField.RegisterValueChangedCallback(evt => 
        { 
            dialogueNode.DialogueText = evt.newValue;
           // dialogueNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogueNode.DialogueText);
        dialogueNode.mainContainer.Add(textField);


        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(position, defaultNodeSize));

        return dialogueNode;
    }

    public EventNode CreateEventNode(string nodeName, Vector2 position, eventType _eventType, string eventName = "", string eventValue = "", bool repeatable = false, bool fired = false)
    {
        //Debug.Log("creating event node!");
        var eventNode = new EventNode
        {
            title = _eventType.ToString() + " Event Node",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = nodeName,
            EventName = eventName,
            EventValue = eventValue,
            isRepeatable = repeatable,
            hasFired = fired,
            eventType = _eventType,
            nodeType = nodeType.eventNode
        };

         // add input and style sheet
        var inputPort = GeneratePort(eventNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        eventNode.inputContainer.Add(inputPort);
        eventNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        // output port once event fires
        var outputPort = GeneratePort(eventNode, Direction.Output);
        outputPort.portName = "Event Fired";
        eventNode.outputContainer.Add(outputPort);

        var outputPort2 = GeneratePort(eventNode, Direction.Output);
        outputPort2.portName = "Already Triggered";
        eventNode.outputContainer.Add(outputPort2);

        // setup event data container
        var eventDataContainer = new VisualElement{
            name="eventDataContainer"
        };
        var eventNameContainer = new VisualElement();
        var eventValueContainer = new VisualElement();
        var eventToggleContainer = new VisualElement();

        // add event name field
        var textFieldEventName = new TextField
        {
            name = "Event Name",
            value = eventName
        };
        textFieldEventName.RegisterValueChangedCallback(evt => eventNode.EventName = evt.newValue);
        textFieldEventName.SetValueWithoutNotify(eventNode.EventName);
        // add event value field
        var textFieldEventValue = new TextField
        {
            name = "Event Value",
            value = eventValue
        };
        textFieldEventValue.RegisterValueChangedCallback(evt => eventNode.EventValue = evt.newValue);
        textFieldEventValue.SetValueWithoutNotify(eventNode.EventValue);
        // add is repeatable
        var toggleIsRepeatable = new Toggle 
        {
            name = "isRepeatable?",
            value = repeatable
        };
        toggleIsRepeatable.RegisterValueChangedCallback(evt => eventNode.isRepeatable = evt.newValue);
        toggleIsRepeatable.SetValueWithoutNotify(eventNode.isRepeatable);

        eventNameContainer.Add(new Label("Event Name:"));
        eventNameContainer.Add(textFieldEventName);
        eventValueContainer.Add(new Label("Parameter Value:"));
        eventValueContainer.Add(textFieldEventValue);
        eventToggleContainer.Add(new Label("Is Repeatable?"));
        eventToggleContainer.Add(toggleIsRepeatable);

        eventDataContainer.Add(eventNameContainer);
        eventDataContainer.Add(eventValueContainer);
        eventDataContainer.Add(eventToggleContainer);

        
        eventNode.mainContainer.Add(eventDataContainer);

        eventNode.RefreshExpandedState();
        eventNode.RefreshPorts();
        eventNode.SetPosition(new Rect(position, defaultNodeSize));

        return eventNode;
    }

    public CheckNode CreateCheckNode(string nodeName, Vector2 position, string nodePrefix="", conditionType _checkType = conditionType.none, string _checkName = "", string _checkValue = "", bool repeatable = false, bool rollable = true, bool passed = false)
    {
        //Debug.Log("creating event node!");
        var checkNode = new CheckNode
        {
            checkType = _checkType,
            title = nodePrefix + " Check Node",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = nodePrefix + " " + nodeName,
            checkName = _checkName,
            checkValue = _checkValue,
            isRepeatable = repeatable,
            isRollable = rollable,
            alreadyPassed = passed,
            nodeType = nodeType.checkNode
        };
        
        // Debug.Log("this nodes check type " + checkNode.checkType.ToString());

         // add input and style sheet
        var inputPort = GeneratePort(checkNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        checkNode.inputContainer.Add(inputPort);
        checkNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        checkNode.titleContainer.name = checkNode.checkType +"Title";

        // output port once event fires
        var outputPort = GeneratePort(checkNode, Direction.Output);
        outputPort.portName = "Passed";
        checkNode.outputContainer.Add(outputPort);

        var outputPort2 = GeneratePort(checkNode, Direction.Output);
        outputPort2.portName = "Failed";
        checkNode.outputContainer.Add(outputPort2);

        // setup event data container
        var eventDataContainer = new VisualElement{
            name = checkNode.checkType + "DataContainer"
        };
        var eventNameContainer = new VisualElement();
        var eventValueContainer = new VisualElement();
        var eventToggleContainer = new VisualElement();

        // add event name field
        var textFieldEventName = new TextField
        {
            name = "checkName",
            value = _checkName
        };
        textFieldEventName.RegisterValueChangedCallback(evt => checkNode.checkName = evt.newValue);
        textFieldEventName.SetValueWithoutNotify(checkNode.checkName);
        eventNameContainer.Add(new Label("Check Name:"));
        eventNameContainer.Add(textFieldEventName);

        // add event value field
        var textFieldEventValue = new TextField
        {
            name = "checkValue",
            value = _checkValue
        };
        textFieldEventValue.RegisterValueChangedCallback(evt => checkNode.checkValue = evt.newValue);
        textFieldEventValue.SetValueWithoutNotify(checkNode.checkValue);
        eventValueContainer.Add(new Label("Parameter Value:"));
        eventValueContainer.Add(textFieldEventValue);

        // add is repeatable
        var toggleIsRepeatable = new Toggle 
        {
            name = "isRepeatable?",
            value = repeatable
        };
        toggleIsRepeatable.RegisterValueChangedCallback(evt => checkNode.isRepeatable = evt.newValue);
        toggleIsRepeatable.SetValueWithoutNotify(checkNode.isRepeatable);
        eventToggleContainer.Add(new Label("Is Repeatable?"));
        eventToggleContainer.Add(toggleIsRepeatable);

        // check if node check type is a rollable type
        var toggleIsRollable = new Toggle 
            {
                name = "isRepeatable?",
                value = repeatable
        };
        if(checkNode.checkType == conditionType.playerProperty || checkNode.checkType == conditionType.playerSkill || checkNode.checkType == conditionType.npcProperty || checkNode.checkType == conditionType.enemyProperty)
        {
            
            // add is rollable option
            checkNode.isRollable = true;
            toggleIsRollable.RegisterValueChangedCallback(evt => checkNode.isRollable = evt.newValue);
            toggleIsRollable.SetValueWithoutNotify(checkNode.isRollable);
            eventToggleContainer.Add(new Label("Is Rollable?"));
            eventToggleContainer.Add(toggleIsRollable);
        }
        
        
        

        eventDataContainer.Add(eventNameContainer);
        eventDataContainer.Add(eventValueContainer);
        eventDataContainer.Add(eventToggleContainer);

        // var checkTypeSelect = new DropdownMenu();
        // eventDataContainer.Add(checkTypeSelect);

        
        checkNode.mainContainer.Add(eventDataContainer);

        checkNode.RefreshExpandedState();
        checkNode.RefreshPorts();
        checkNode.SetPosition(new Rect(position, defaultNodeSize));

        return checkNode;
    }

    public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "") 
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);


        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        generatedPort.portName = $"Choice {outputPortCount}";

        var choicePortName = string.IsNullOrEmpty(overriddenPortName)
            ? $"Choice {outputPortCount + 1}"
            : overriddenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(()=>RemovePort(dialogueNode,generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);




        generatedPort.portName = choicePortName;
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }

    public void AddCheckPorts(DialogueNode dialogueNode, string content = "", string skillName = "", string checkValue = "")
    {
        var generatedPassPort = GeneratePort(dialogueNode, Direction.Output);
        var generatedFailPort = GeneratePort(dialogueNode, Direction.Output);
        
        // first row pass output and choice text
        

        generatedPassPort.portName = $"{skillName} Chk.Pass";
        var textFieldContent = new TextField
        {
            name = string.Empty,
            value = content
        };
        textFieldContent.RegisterValueChangedCallback(evt => 
        ((TextField) generatedPassPort.contentContainer.ElementAt(3)).value = evt.newValue);
        generatedPassPort.contentContainer.Add(new Label("  "));
        generatedPassPort.contentContainer.Add(textFieldContent);
        var deleteButton = new Button(()=>RemoveCheckPort(dialogueNode,generatedPassPort,generatedFailPort))
        {
            text = "X"
        };
        generatedPassPort.contentContainer.Add(deleteButton);
        
        // second row, fail output port and skill/check fields
        
        generatedFailPort.portName = $"{skillName} Chk.Fail";
        var textFieldSkill = new TextField
        {
            name = string.Empty,
            value = skillName
        };
        textFieldSkill.RegisterValueChangedCallback(evt => 
        {
            // Debug.Log("test: " + generatedFailPort.contentContainer.ElementAt(2));
            
            ((TextField) generatedFailPort.contentContainer.ElementAt(2)).value = evt.newValue;
            generatedPassPort.portName = $"{evt.newValue} Chk.Pass";
            generatedFailPort.portName = $"{evt.newValue} Chk.Fail";
            //UnityEngine.Debug.Log("Changing value: " + evt.newValue);
        });
        generatedFailPort.contentContainer.Add(textFieldSkill);
        generatedFailPort.contentContainer.Add(new Label("Skill: "));
        var textFieldCheck = new TextField
        {
            name = string.Empty,
            value = checkValue
        };
        textFieldCheck.RegisterValueChangedCallback(evt => ((TextField) generatedFailPort.contentContainer.ElementAt(4)).value = evt.newValue);
        generatedFailPort.contentContainer.Add(textFieldCheck);
        generatedFailPort.contentContainer.Add(new Label("Check Value: "));


        dialogueNode.outputContainer.Add(generatedPassPort);
        dialogueNode.outputContainer.Add(generatedFailPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();

    }

    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {

        var targetEdge = edges.ToList().Where(x => 
            x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        if(targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }
        

        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

    private void RemoveCheckPort(DialogueNode dialogueNode, Port passPort, Port failPort)
    {
        // remove pass
        var targetEdge = edges.ToList().Where(x => 
            x.output.portName == passPort.portName && x.output.node == passPort.node);

        if(targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        // remove fail
        targetEdge = edges.ToList().Where(x => 
            x.output.portName == failPort.portName && x.output.node == failPort.node);
        if(targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }
        

        dialogueNode.outputContainer.Remove(passPort);
        dialogueNode.outputContainer.Remove(failPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

    public void AddPropertyToBlackboard(ExposedProperty exposedProperty)
    {
        
        var localPropertyName = exposedProperty.PropertyName;
        var localPropertyValue = exposedProperty.PropertyValue;
        while(ExposedProperties.Any(x => x.PropertyName == localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
        
        
        var property = new ExposedProperty();
        property.PropertyName = localPropertyName;
        property.PropertyValue = localPropertyValue;
        ExposedProperties.Add(property);

        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.PropertyName, typeText = "string property"};
        container.Add(blackboardField);

        var propertyValueTextField = new TextField("Value: ")
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = ExposedProperties.FindIndex(x => x.PropertyName == property.PropertyName);
            ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
        });
        var blackBoardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackBoardValueRow);

        Blackboard.Add(container);
    }

    public void ClearBlackBoardAndExposedProperties()
    {
        ExposedProperties.Clear();
        Blackboard.Clear();
    }
}
