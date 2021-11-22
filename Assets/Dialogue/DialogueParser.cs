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
         // Events
        public event Action<eventType, string, string> onEventTriggered;
        public static event Action onDialogueReachedDeadEnd;

        public static event Action<string,string> onExposedPropertyFound;
        // delegate
        public Func<string, string, bool> onPlayerSkillRoll;
     
     
     
        [HideInInspector] public gameController controller;
        
        [SerializeField] private DialogueContainer dialogue;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button choicePrefab;
        [SerializeField] private Transform buttonContainer;

        
        [SerializeField] private List<dialogueHistory> _dialogueHistory = new List<dialogueHistory>();
        [SerializeField] private dialogueHistory _currentNodeHistory;
        private string _previousNodeGUID;
        private string _currentNodeGUID;
        private string _pendingText = "";

        [Serializable]
        private class dialogueHistory {
            public string previousNodeGUID;
            public string currentNodeGUID;
            public string selectedOutputNodeGUID;
            public nodeType nodeType;

        }
        

        private void Awake() {
            controller = GetComponent<gameController>();  
            controller.EventManager.onEventFinished += UpdatePendingText;
            checkManager.onRollCheckComplete += ProcessRollResult;
            RollScreen.onRollScreenCancelled += BacktrackToLastDialogueNodeInHistory;
            UIManager.onDialogueEnded += ClearDialogueHistory;
            
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

            // clear old dialoge history
            _dialogueHistory.Clear();
            
            // check for exposed properties to use in nav object
            if(dialogue.ExposedProperties.Count > 0)
            {
                foreach(ExposedProperty exposedProperty in dialogue.ExposedProperties)
                {
                    onExposedPropertyFound?.Invoke(exposedProperty.PropertyName, exposedProperty.PropertyValue);
                }
            }
            var narrativeData = dialogue.NodeLinks.First(); //Entrypoint node

            // setup first node dialoge history.
            _currentNodeHistory = new dialogueHistory {
                previousNodeGUID = "START",
                currentNodeGUID = narrativeData.BaseNodeGuid,
                selectedOutputNodeGUID = null,
                nodeType = nodeType.dialogueNode
            };
            // _previousNodeGUID = narrativeData.TargetNodeGuid;
            
            
            ProceedToNarrative(narrativeData.TargetNodeGuid);
        }

        public void UpdatePendingText(string text)
        {
            Debug.Log("Updating pending text: " + text);
            _pendingText += text;
        }
        
        private void AddNodeToHistory(string selectedOutputGUID, nodeType nodeType)
        {
            _currentNodeHistory.selectedOutputNodeGUID = selectedOutputGUID;
            // _currentNodeHistory.nodeType = nodeType;
            _dialogueHistory.Add(_currentNodeHistory);
            _currentNodeHistory = new dialogueHistory {
                previousNodeGUID = _dialogueHistory[_dialogueHistory.Count - 1].currentNodeGUID,
                currentNodeGUID = selectedOutputGUID,
                selectedOutputNodeGUID = null,
                nodeType = nodeType
            };
        }

        private void ProceedToNarrative(string narrativeDataGUID)
        {
            Debug.Log("DIALOGUE PARSER ---- Proceeding to next narrative node for GUID: " + narrativeDataGUID);
            var text = dialogue.DialogueNodeData.Find(x => x.Guid == narrativeDataGUID).DialogueText;
            var nodeType = dialogue.DialogueNodeData.Find(x => x.Guid == narrativeDataGUID).nodeType;

            // setup the next history node data
            AddNodeToHistory(narrativeDataGUID, nodeType);
            

            // cache current node to be used in segmented methods
            _currentNodeGUID = narrativeDataGUID;

            switch(nodeType)
            {
                case nodeType.dialogueNode:
                    ProcessDialogueNode(narrativeDataGUID, text);
                break;
                case nodeType.eventNode:
                    ProcessEventNode(narrativeDataGUID);
                break;
                case nodeType.rollNode:
                    ProcessRollNode(narrativeDataGUID);
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

            // flag to catch any passed roll checks within a single groupTagID in this dialogue node
            bool hasPassedGroupTagID = false;

            foreach (var choice in choices)
            {
                var buttonText = choice.PortName;
                // check if target is a roll node, and if so add skill prefix
                if(dialogue.RollNodeData.Exists( x => x.nodeGuid == choice.TargetNodeGuid))
                {
                    // if we've already found a matched success, exit early.
                    if(hasPassedGroupTagID) continue;
                    
                    var rollData = dialogue.RollNodeData.Find(x => x.nodeGuid == choice.TargetNodeGuid);
                    var skillPrefix = rollData.rollSkillName;
                    var rollDifficulty = rollData.rollDifficulty;
                    var skillHex = globalConfig.UI.SkillTextHexColor;

                    // check if player has passed one of any roll checks within this groupTagID
                    if(PlayerProgressTracker.Instance.hasPlayerPassedRollCheck(rollData.rollGroupTagID))
                    {
                        hasPassedGroupTagID = true; // set flag to avoid duplicate entries
                        buttonText = "<b>[PASSED]</b> " + rollData.passedDescription;
                    }
                    else
                    {
                        // otherwise setup the fancy roll check button
                        buttonText = $"[ROLL <size=25%><color={skillHex}>{skillPrefix}</color></size>] <i>(Difficulty: {rollDifficulty})</i>\n<b>{buttonText}</b>";
                    }


                    
                }
                
                // let UI manager handle button creation and logic for any standard non-roll choices
                controller.UIManager.CreateDialogueOptionButton(choice.TargetNodeGuid, buttonText);
                
            }
            
            // finally create confirm button
            controller.UIManager.initConfirmActionButton();

            
        }

        private void ProcessRollNode(string narrativeDataGUID)
        {
            Debug.Log("DIALOGUE PARSER ---- Roll node Found. Guid: " + narrativeDataGUID);
            var data = dialogue.RollNodeData.Find(x => x.nodeGuid == narrativeDataGUID);
            var outputs = dialogue.NodeLinks.Where(x => x.BaseNodeGuid == narrativeDataGUID);
            var rollGroupTagID = data.rollGroupTagID;
            var rollSkillName = data.rollSkillName;
            var rollDescription = data.rollDescription;
            var rollDifficulty = data.rollDifficulty;
            var isRepeatable = data.isRepeatable;

            // first check if roll group tag exists in history and roll is not repeatable
            if(PlayerProgressTracker.Instance.RollCheckGroupEntryExists(rollGroupTagID))
            {
                var historyData = PlayerProgressTracker.Instance.GetRollCheckEntry(data.nodeGuid);
                if(historyData != null)
                {
                    if(historyData.passedRoll)
                    {
                        ProceedToNarrative(outputs.ElementAt(5).TargetNodeGuid); // already passed, can't repeat
                    }
                    else if(!isRepeatable)
                    {
                        ProceedToNarrative(outputs.ElementAt(6).TargetNodeGuid); // already failed, can't repeat
                    }
                    else 
                    {
                        // initialize the roll check UI and process, failed before but can repeat
                        checkManager.Instance.InitializeRollCheck(rollSkillName, rollDescription, rollDifficulty);
                    }
                }
                else
                {
                    // otherwise we know roll has not passed yet, so start a roll
                    checkManager.Instance.InitializeRollCheck(rollSkillName, rollDescription, rollDifficulty);
                }
                
            }
            else 
            {
                // initialize the roll check UI and process for first time
                checkManager.Instance.InitializeRollCheck(rollSkillName, rollDescription, rollDifficulty);
            }
        }

        private void ProcessRollResult(bool rollResult, rollCheckResultType resultType)
        {
            var data = dialogue.RollNodeData.Find(x => x.nodeGuid == _currentNodeGUID);
            var outputs = dialogue.NodeLinks.Where(x => x.BaseNodeGuid == _currentNodeGUID);

            // add record to player progress
            PlayerProgressTracker.Instance.AddRollCheckEntry(
                new RollCheckEntry {
                    rollNodeGUID = data.nodeGuid,
                    rollGroupTagID = data.rollGroupTagID,
                    rollDescription = data.rollDescription,
                    passedRoll = rollResult,
                    passedDescription = data.passedDescription,
                    isRepeatable = data.isRepeatable
                }
            );

            // if passed, update all roll records under this group id
            if(rollResult) PlayerProgressTracker.Instance.PassAllRollChecksByGroupTagID(data.rollGroupTagID);
            

            switch(resultType)
            {
                case rollCheckResultType.MASTERPASS:
                    ProceedToNarrative(outputs.ElementAt(0).TargetNodeGuid);
                break;
                case rollCheckResultType.MASTERFUMBLE:
                    ProceedToNarrative(outputs.ElementAt(1).TargetNodeGuid);
                break;
                case rollCheckResultType.PASS:
                    ProceedToNarrative(outputs.ElementAt(2).TargetNodeGuid);
                break;
                case rollCheckResultType.FAIL:
                    ProceedToNarrative(outputs.ElementAt(3).TargetNodeGuid);
                break;
                case rollCheckResultType.CRITICALFAIL:
                    ProceedToNarrative(outputs.ElementAt(4).TargetNodeGuid);
                break;
            }
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

        private void ClearDialogueHistory()
        {
           // called after a dialogue is concluded from UI
            _dialogueHistory.Clear();
            _currentNodeHistory = null;
            _currentNodeGUID = null;
        }


        private void BacktrackToLastDialogueNodeInHistory()
        {
            Debug.Log("DIALOGUE PARSER ---- Backtracing to last selected dialogue node.");
            foreach(dialogueHistory nodeEntry in _dialogueHistory.AsEnumerable().Reverse())
            {
                // var nodeEntry = _dialogueHistory[i];
                Debug.Log("DIALOGUE PARSER ---- Checking node guid for dialogueNode: " + nodeEntry.currentNodeGUID);
                if(nodeEntry.nodeType == nodeType.dialogueNode) 
                {
                    Debug.Log("DIALOGUE PARSER ---- Found dialogue node, backtracking to GUID: " + nodeEntry.currentNodeGUID);
                    ProceedToNarrative(nodeEntry.currentNodeGUID);
                    return;
                }
            }
        }

}
