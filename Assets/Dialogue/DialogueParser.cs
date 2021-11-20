using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using globalDataTypes;

public class DialogueParser : MonoBehaviour
    {
        [HideInInspector] public gameController controller;
        
        [SerializeField] private DialogueContainer dialogue;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button choicePrefab;
        [SerializeField] private Transform buttonContainer;

        private string _pendingText = "";

        // Events
        public event Action<eventType, string, string> onEventTriggered;
        public static event Action onDialogueReachedDeadEnd;

        public static event Action<string,string> onExposedPropertyFound;
        // delegate
        public Func<string, string, bool> onPlayerSkillRoll;

        
        private void Awake() {
            controller = GetComponent<gameController>();  
            controller.EventManager.onEventFinished += UpdatePendingText;
            
        }
        
        private void Start()
        {
            
            

            // ProceedToNarrative(narrativeData.TargetNodeGuid);
        }

        public void SetupNewDialogue(DialogueContainer newDialogue)
        {
            Debug.Log("DIALOGE PARSER ---- ENABLED");
            // UIManager.onOptionSelected -= ProceedToNarrative; // remove first if any
            UIManager.onOptionSelected += ProceedToNarrative;
            dialogue = newDialogue;
        }

        public void DisableDialogueParser()
        {
            Debug.Log("DIALOGE PARSER ---- DISABLED");
            UIManager.onOptionSelected -= ProceedToNarrative;
        }

        public void InitDialogue() {

            // check for exposed properties to use in nav object
            if(dialogue.ExposedProperties.Count > 0)
            {
                foreach(ExposedProperty exposedProperty in dialogue.ExposedProperties)
                {
                    onExposedPropertyFound?.Invoke(exposedProperty.PropertyName, exposedProperty.PropertyValue);
                }
            }
            var narrativeData = dialogue.NodeLinks.First(); //Entrypoint node
            ProceedToNarrative(narrativeData.TargetNodeGuid);
        }

        public void UpdatePendingText(string text)
        {
            Debug.Log("Updating pending text: " + text);
            _pendingText += text;
        }
        


        private void ProceedToNarrative(string narrativeDataGUID)
        {
            Debug.Log("DIALOGUE PARSER ---- Proceeding to next narrative node for GUID: " + narrativeDataGUID);
            var text = dialogue.DialogueNodeData.Find(x => x.Guid == narrativeDataGUID).DialogueText;
            var nodeType = dialogue.DialogueNodeData.Find(x => x.Guid == narrativeDataGUID).nodeType;

            switch(nodeType)
            {
                case nodeType.dialogueNode:
                    ProcessDialogueNode(narrativeDataGUID, text);
                break;
                case nodeType.eventNode:
                    ProcessEventNode(narrativeDataGUID);
                break;
                case nodeType.checkNode:
                    ProcessCheckNode(narrativeDataGUID);
                break;
                case nodeType.endpointNode:
                    ProcessEndpointNode(narrativeDataGUID);
                break;
            }
            
        }

        private void ProcessDialogueNode(string narrativeDataGUID, string text)
        {
            Debug.Log("DIALOGUE PARSER ---- Setting up new Dialogue Node with Choices");
            var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGuid == narrativeDataGUID);
            controller.UIManager.updateContentText(_pendingText + text);
            _pendingText = "";
            Debug.Log("DIALOGUE PARSER ---- number of choices: " + choices.Count());
            
            
            // instead destroy from UI
            controller.UIManager.ClearButtons();
            
            // if there are no dialogue choices left/ flag end reached
            if(choices.Count() < 1) 
            {
                onDialogueReachedDeadEnd?.Invoke();
                return;
            }

            foreach (var choice in choices)
            {
                // let UI manager handle button creation and logic
                controller.UIManager.CreateDialogueOptionButton(choice.TargetNodeGuid, choice.PortName);
            }
            
            // finally create confirm button
            controller.UIManager.initConfirmActionButton();
        }

        private void ProcessEventNode(string narrativeDataGUID)
        {
            Debug.Log("DIALOGUE PARSER ---- Event node Found. GUID: "+ narrativeDataGUID);
            //bool eventAlreadyTriggered = false;
            var data = dialogue.EventNodeData.Find(x => x.nodeGuid == narrativeDataGUID);
            var outputs = dialogue.NodeLinks.Where(x => x.BaseNodeGuid == narrativeDataGUID);
            var eventName = data.EventName;
            var eventValue = data.EventValue;
            var ignoreDeadEnd = data.ignoreDeadEnd;
            
            // add logic to check event specifics
            if(!data.hasFired || data.isRepeatable) {
                // trigger event
                Debug.Log("DIALOGUE PARSER ---- Triggering Event!");
                onEventTriggered?.Invoke(data.eventType, data.EventName,data.EventValue);
                
                // route to next node if output is attached to another node
                if(outputs.Count() > 0)
                {
                    if(outputs.ElementAt(0) != null) 
                    {
                        // route from event triggered
                        Debug.Log("DIALOGUE PARSER ---- Navigating to next node from 'Event Triggered' output!");
                        if(outputs.ElementAt(0).TargetNodeGuid != null) ProceedToNarrative(outputs.ElementAt(0).TargetNodeGuid);
                        return;
                    }
                }
                else if (!ignoreDeadEnd){
                    Debug.Log("DIALOGUE PARSER ---- Not ignoring dead end. Invoking DeadEnd.");
                    ProcessEventDeadEnd();
                    return;
                }
                else {
                    Debug.Log("DIALOGUE PARSER ---- Ignoring dead end. Returning without invocation.");
                    DisableDialogueParser();
                    return;
                }
            }
            else {
                Debug.Log("DIALOGUE PARSER ---- Event already triggered!");
                if(outputs.Count() > 0)
                {
                    if(outputs.ElementAt(1) != null) 
                    {
                        // route from event already triggered
                        Debug.Log("DIALOGUE PARSER ---- Navigating to next node from 'Event Already Triggered' output!");
                        if(outputs.ElementAt(1).TargetNodeGuid != null) ProceedToNarrative(outputs.ElementAt(1).TargetNodeGuid);

                        return;
                    }
                }
                else if(!ignoreDeadEnd) {
                    Debug.Log("DIALOGUE PARSER ---- Not ignoring dead end. Invoking DeadEnd.");
                    ProcessEventDeadEnd();
                    return;
                }
                else {
                    Debug.Log("DIALOGUE PARSER ---- Ignoring dead end. Returning without invocation but will disable parser.");
                    DisableDialogueParser();
                    return;
                }
            }
        }

        private void ProcessCheckNode(string narrativeDataGUID)
        {
            var data = dialogue.CheckNodeData.Find(x => x.nodeGuid == narrativeDataGUID);
            var outputs = dialogue.NodeLinks.Where(x => x.BaseNodeGuid == narrativeDataGUID);
            var checkName = data.checkName;
            var checkValue = data.checkValue;
            // if dead end, call and return
            if(outputs.Count() < 1) {
                Debug.Log("Check node found with no outputs. Please check this dialogue graph.");
                Debug.Log("Graph: " + dialogue);
                Debug.Log("NodeGUID: " + narrativeDataGUID);
                onDialogueReachedDeadEnd?.Invoke();
                return;
            }

            // if check already passed, route to success path
            if(data.alreadyPassed) 
            {
                if(outputs.ElementAt(0) != null) 
                    {
                        // route from event triggered
                        if(outputs.ElementAt(0).TargetNodeGuid != null) ProceedToNarrative(outputs.ElementAt(0).TargetNodeGuid);
                        return;
                    }
            }

            // check for rollable check (mostly player skill)
            if(data.isRollable)
            {
                if(data.checkType == conditionType.playerSkill)
                {
                    Debug.Log("Processing player skill check roll.");
                    Debug.Log("FUNC: " + onPlayerSkillRoll);
                    if(onPlayerSkillRoll != null)
                    {
                        var passedRoll = onPlayerSkillRoll(checkName, checkValue);
                        Debug.Log("Passed roll: " + passedRoll);
                        if(passedRoll)
                        {
                            Debug.Log("Skill check passed. Routing...");
                            // set passed
                            data.alreadyPassed = true;
                            if(outputs.ElementAt(0).TargetNodeGuid != null) ProceedToNarrative(outputs.ElementAt(0).TargetNodeGuid);
                        }
                        else 
                        {
                            Debug.Log("Skill check FAILED. Routing...");
                            if(outputs.ElementAt(1).TargetNodeGuid != null) ProceedToNarrative(outputs.ElementAt(1).TargetNodeGuid);
                        }
                    }
                }

            }

            
        }

        private void ProcessEventDeadEnd()
        {
            Debug.Log("DIALOGUE PARSER ---- Dead end reached on event node. Updating content and clearing exist buttons before invocation.");
            controller.UIManager.updateContentText("");
            controller.UIManager.ClearButtons();
            onDialogueReachedDeadEnd?.Invoke();
        }

        private void ProcessEndpointNode(string narrativeDataGUID)
        {
            Debug.Log("End point reached.");
            onDialogueReachedDeadEnd?.Invoke();
            return;
        }

        private string ProcessProperties(string text)
        {
            foreach (var exposedProperty in dialogue.ExposedProperties)
            {
                text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue);
            }
            return text;
        }
    }
