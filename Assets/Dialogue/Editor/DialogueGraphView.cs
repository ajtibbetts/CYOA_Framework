using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using globalDataTypes;

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
            case "Additive Choice Node":
                AddElement(CreateAdditiveChoiceNode(nodeName, position));
            break;
            case "Roll Node":
                AddElement(CreateSkillRollNode(nodeName, position));
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

    public DialogueNode CreateAdditiveChoiceNode(string dialogueText, Vector2 position)
    {
        Debug.Log("Creating additive choice node");
        var dialogueNode = new DialogueNode
        {
            title = "Additive Choice Node",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = dialogueText,
            nodeType = nodeType.dialogueNode
        };

        // add input and style sheet
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);
        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        dialogueNode.AddToClassList("additive");
        dialogueNode.name = "Additive";

        // output port that connects to the dialogue node to add these choices to.
        var outputPort = GeneratePort(dialogueNode, Direction.Output);
        outputPort.portName = "Dialogue Node To Add To";
        dialogueNode.outputContainer.Add(outputPort);


        // Add Choice Button
        var button = new Button(()=>{AddChoicePort(dialogueNode);});
        button.text = "+ Choice";
        dialogueNode.titleContainer.Add(button);

        // var textField = new TextField(string.Empty){
        //     multiline = true
        // };
        // textField.RegisterValueChangedCallback(evt => 
        // { 
        //     dialogueNode.DialogueText = evt.newValue;
        //    // dialogueNode.title = evt.newValue;
        // });
        // textField.SetValueWithoutNotify(dialogueNode.DialogueText);
        // dialogueNode.mainContainer.Add(textField);


        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(position, defaultNodeSize));

        return dialogueNode;
    }

    public RollNode CreateSkillRollNode(string dialogueText, Vector2 position,
        bool repeatable = false, string skillName = "skillName", string checkDiff = "difficulty",
        string desc = "description", string groupID="groupID", string passDesc="passed text")
    {
        Debug.Log("Creating new skill roll node");
        
        var rollNode = new RollNode
        {
            title = "Skill Roll Check Node",
            GUID = Guid.NewGuid().ToString(),
            rollGroupTagID = groupID,
            DialogueText = dialogueText,
            nodeType = nodeType.rollNode,
            rollSkillName = skillName,
            rollDescription = desc,
            rollDifficulty = checkDiff,
            passedDescription = passDesc,
            isRepeatable = repeatable
        };

        // add input and style sheet
        var inputPort = GeneratePort(rollNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        rollNode.inputContainer.Add(inputPort);
        rollNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

         // output ports for each outcome
        var outputPortMasterPass = GeneratePort(rollNode, Direction.Output);
        outputPortMasterPass.portName = "Master Pass";
        rollNode.outputContainer.Add(outputPortMasterPass);

        var outputPortMasterFumble = GeneratePort(rollNode, Direction.Output);
        outputPortMasterFumble.portName = "Master Fumble";
        rollNode.outputContainer.Add(outputPortMasterFumble);

        var outputPortRegularPass = GeneratePort(rollNode, Direction.Output);
        outputPortRegularPass.portName = "Regular Pass";
        rollNode.outputContainer.Add(outputPortRegularPass);

        var outputPortRegularFail = GeneratePort(rollNode, Direction.Output);
        outputPortRegularFail.portName = "Regular Fail";
        rollNode.outputContainer.Add(outputPortRegularFail);

        var outputPortCriticalFail = GeneratePort(rollNode, Direction.Output);
        outputPortCriticalFail.portName = "Critical Fail";
        rollNode.outputContainer.Add(outputPortCriticalFail);

        var outputPortAlreadyPassed = GeneratePort(rollNode, Direction.Output);
        outputPortAlreadyPassed.portName = "Already Passed";
        rollNode.outputContainer.Add(outputPortAlreadyPassed);

        var outputPortAlreadyFailed = GeneratePort(rollNode, Direction.Output);
        outputPortAlreadyFailed.portName = "Already Failed";
        rollNode.outputContainer.Add(outputPortAlreadyFailed);

        // setup event data container
        var rollDataContainer = new VisualElement{
            name = "RollDataContainer"
        };
        var groupIDContainer = new VisualElement();
        var skillNameContainer = new VisualElement();
        var rollDescriptionContainer = new VisualElement();
        var difficultyContainer = new VisualElement();
        var passedTextContainer = new VisualElement();
        var rollToggleContainer = new VisualElement();
        
        // add group id field
        var textFieldGroupID = new TextField
        {
            name = "groupID",
            value = groupID
        };
        textFieldGroupID.RegisterValueChangedCallback(evt => rollNode.rollGroupTagID = evt.newValue);
        textFieldGroupID.SetValueWithoutNotify(rollNode.rollGroupTagID);
        groupIDContainer.Add(new Label("Group ID:"));
        groupIDContainer.Add(textFieldGroupID);

        // add skill name field
        var textFieldSkillName = new TextField
        {
            name = "skillName",
            value = skillName
        };
        textFieldSkillName.RegisterValueChangedCallback(evt => rollNode.rollSkillName = evt.newValue);
        textFieldSkillName.SetValueWithoutNotify(rollNode.rollSkillName);
        skillNameContainer.Add(new Label("Skill Name:"));
        skillNameContainer.Add(textFieldSkillName);

        // add roll description field
        var textFieldRollDescription = new TextField
        {
            name ="rollDescription",
            value = desc
        };
        textFieldRollDescription.RegisterValueChangedCallback(evt => rollNode.rollDescription = evt.newValue);
        textFieldRollDescription.SetValueWithoutNotify(rollNode.rollDescription);
        rollDescriptionContainer.Add(new Label("Roll Description:"));
        rollDescriptionContainer.Add(textFieldRollDescription);

        // add roll difficulty field
        var textFieldDifficultyValue = new TextField
        {
            name = "checkDifficulty",
            value = checkDiff
        };
        textFieldDifficultyValue.RegisterValueChangedCallback(evt => rollNode.rollDifficulty = evt.newValue);
        textFieldDifficultyValue.SetValueWithoutNotify(rollNode.rollDifficulty);
        difficultyContainer.Add(new Label("Difficulty Value:"));
        difficultyContainer.Add(textFieldDifficultyValue);

        // add passed text container
        // add roll difficulty field
        var textFieldPassedText = new TextField
        {
            name = "passedText",
            value = passDesc
        };
        textFieldPassedText.RegisterValueChangedCallback(evt => rollNode.passedDescription = evt.newValue);
        textFieldPassedText.SetValueWithoutNotify(rollNode.passedDescription);
        passedTextContainer.Add(new Label("Passed Text:"));
        passedTextContainer.Add(textFieldPassedText);


        // add is repeatable
        var toggleIsRepeatable = new Toggle 
        {
            name = "isRepeatable?",
            value = repeatable
        };
        toggleIsRepeatable.RegisterValueChangedCallback(evt => rollNode.isRepeatable = evt.newValue);
        toggleIsRepeatable.SetValueWithoutNotify(rollNode.isRepeatable);
        rollToggleContainer.Add(new Label("Is Repeatable?"));
        rollToggleContainer.Add(toggleIsRepeatable);



        rollDataContainer.Add(groupIDContainer);
        rollDataContainer.Add(skillNameContainer);
        rollDataContainer.Add(rollDescriptionContainer);
        rollDataContainer.Add(difficultyContainer);
        rollDataContainer.Add(passedTextContainer);
        rollDataContainer.Add(rollToggleContainer);

        rollNode.mainContainer.Add(rollDataContainer);

        rollNode.RefreshExpandedState();
        rollNode.RefreshPorts();
        rollNode.SetPosition(new Rect(position, defaultNodeSize));

        return rollNode;
    }

    public EventNode CreateEventNode(string nodeName, Vector2 position, eventType _eventType, string eventName = "", string eventValue = "", bool repeatable = false, bool fired = false, bool ignoreDeadend = true)
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
            ignoreDeadEnd = ignoreDeadend,
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
        // add ignore dead end
        var toggleIgnoreDeadEnd = new Toggle 
        {
            name = "ignoreDeadEnd?",
            value = repeatable
        };
        toggleIgnoreDeadEnd.RegisterValueChangedCallback(evt => eventNode.ignoreDeadEnd = evt.newValue);
        toggleIgnoreDeadEnd.SetValueWithoutNotify(eventNode.ignoreDeadEnd);

        eventNameContainer.Add(new Label("Event Name:"));
        eventNameContainer.Add(textFieldEventName);
        eventValueContainer.Add(new Label("Parameter Value:"));
        eventValueContainer.Add(textFieldEventValue);
        eventToggleContainer.Add(new Label("Is Repeatable?"));
        eventToggleContainer.Add(toggleIsRepeatable);
        eventToggleContainer.Add(new Label("Ignore Dead End?"));
        eventToggleContainer.Add(toggleIgnoreDeadEnd);

        eventDataContainer.Add(eventNameContainer);
        eventDataContainer.Add(eventValueContainer);
        eventDataContainer.Add(eventToggleContainer);

        
        eventNode.mainContainer.Add(eventDataContainer);

        eventNode.RefreshExpandedState();
        eventNode.RefreshPorts();
        eventNode.SetPosition(new Rect(position, defaultNodeSize));

        return eventNode;
    }

    public CheckNode CreateCheckNode(string nodeName, Vector2 position, string nodePrefix="",
        conditionType _checkType = conditionType.none, string _checkName = "", string _checkValue = "", string comparison = "==")
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
            comparisonOperator = comparison,
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
        var checkSelectContainer = new VisualElement();

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

        List<string> choices = new List<string>{"==","!=",">=","<=",">","<"};
        var testSelect = new DropdownField("Comparison",choices,0);
        testSelect.RegisterValueChangedCallback(evt => checkNode.comparisonOperator = evt.newValue);
        testSelect.SetValueWithoutNotify(checkNode.comparisonOperator);
        checkSelectContainer.Add(testSelect);
       
        
        
        

        eventDataContainer.Add(eventNameContainer);
        eventDataContainer.Add(eventValueContainer);
        eventDataContainer.Add(checkSelectContainer);

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
