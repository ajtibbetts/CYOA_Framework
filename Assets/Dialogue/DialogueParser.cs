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
        
        [Header("Dialogue Elements")]
        [SerializeField] private DialogueContainer dialogue;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button choicePrefab;
        [SerializeField] private Transform buttonContainer;
        [Header("Dialogue Speakers")]
        [SerializeField] private List<DialogueSpeaker> _commonSpeakers = new List<DialogueSpeaker>(); // speakers available at any time (not profiles)

        [Header("Dialogue History")]
        [SerializeField] private List<dialogueHistory> _dialogueHistory = new List<dialogueHistory>();
        [SerializeField] private dialogueHistory _currentNodeHistory;
        private string _previousNodeGUID;
        private string _currentNodeGUID;
        private string _pendingText = "";

        // for 'skipping to dialogue' stuff
        private DialogueContainer _homeDialogue;
        private DialogueContainer _previousDialogue;

        private List<NodeLinkData> _pendingAdditiveChoices = new List<NodeLinkData>();

        [Serializable]
        private class dialogueHistory {
            public string previousNodeGUID;
            public string currentNodeGUID;
            public string selectedOutputNodeGUID;
            public nodeType nodeType;

        }
        
        private PlayerProgressTracker _playerProgress;

        private void Awake() {
            controller = GetComponent<gameController>();  
            _playerProgress = PlayerProgressTracker.Instance;

            controller.EventManager.onEventFinished += UpdatePendingText;
            checkRollManager.onRollCheckComplete += ProcessRollResult;
            RollScreen.onRollScreenCancelled += BacktrackToLastDialogueNodeInHistory;
            UIManager.onDialogueEnded += ClearDialogueHistory;
            UIManager.onProceedToNextNode += ProceedToNextNode;
            NavObject.OnSkipToDialogue += SkipToDialogue;
            
            
        }
        
        private void Start()
        {
            
            

            // ProceedToNarrative(narrativeData.TargetNodeGuid);
        }

        public void SetupNewDialogue(DialogueContainer newDialogue)
        {
            Debug.Log("DIALOGE PARSER ---- ENABLED");
            // UIManager.onOptionSelected -= ClearContentAndProceedToNode; // remove first if any
            UIManager.onOptionSelected += ClearContentAndProceedToNode;
            dialogue = newDialogue;
            // clear any previous 'skip' stuff
            _homeDialogue = null;
            _previousDialogue = null;
        }

        public void DisableDialogueParser()
        {
            Debug.Log("DIALOGE PARSER ---- DISABLED");
            UIManager.onOptionSelected -= ClearContentAndProceedToNode;
        }

        public void InitDialogue(string optionalTargetNodeGUID = null) {

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
            
            if(optionalTargetNodeGUID == null)
            {
                ClearContentAndProceedToNode(narrativeData.TargetNodeGuid);
            }
            else
            {
                ClearContentAndProceedToNode(optionalTargetNodeGUID);
            }
        }

        public void SkipToDialogue(DialogueContainer newDialogue, string optionalTargetNodeGUID = null, string baseFlag = "")
        {
            if(newDialogue == null)
            {
                // SetupNewDialogue(_previousDialogue);
                if(baseFlag == "previous") dialogue = _previousDialogue;
                else if (baseFlag == "home") dialogue = _homeDialogue;
            }
            else
            {
                // if this is first 'skip', set current dialogue to home
                if(_homeDialogue == null) _homeDialogue = dialogue;
                _previousDialogue = dialogue;
                // SetupNewDialogue(newDialogue);
                dialogue = newDialogue;
            }
            InitDialogue(optionalTargetNodeGUID);
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

        private void ClearContentAndProceedToNode(string narrativeDataGUID)
        {
            // clear existing buttons and recreate
            controller.UIManager.ClearContentAndButtons();
            ProceedToNarrative(narrativeDataGUID);
        }

        private void ProceedToNarrative(string narrativeDataGUID)
        {
            Debug.Log("DIALOGUE PARSER ---- Proceeding to next narrative node for GUID: " + narrativeDataGUID);
            var data = dialogue.DialogueNodeData.Find(x => x.Guid == narrativeDataGUID);
            var text = data.DialogueText;
            var nodeType = data.nodeType;
            var characterID = data.characterID;

            // setup the next history node data
            AddNodeToHistory(narrativeDataGUID, nodeType);
            
            // check for any 'returned' text.
            text = GetReturnedText(narrativeDataGUID, text);

            // cache current node to be used in segmented methods
            _currentNodeGUID = narrativeDataGUID;

            switch(nodeType)
            {
                case nodeType.additiveDialogue:
                    ProcessAdditiveParagraphNode(narrativeDataGUID, text, characterID, data.autoProgress);
                    PlayerProgressTracker.Instance.AddDialogueEntry(data);
                break;
                case nodeType.dialogueNode:
                    if(text.Contains("Additive Choice Node")) ProcessAdditiveChoiceNode(narrativeDataGUID);
                    else ProcessDialogueNode(narrativeDataGUID, text, characterID); // regular dialogue node
                    PlayerProgressTracker.Instance.AddDialogueEntry(data);
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

        private void ProcessDialogueNode(string narrativeDataGUID, string text, string characterID)
        {
            Debug.Log("DIALOGUE PARSER ---- Setting up new Dialogue Node with Choices");
            var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGuid == narrativeDataGUID).ToList();
            _pendingText = "";
            // Debug.Log("DIALOGUE PARSER ---- number of choices: " + choices.Count());
        
            
            AddTextToUI(text, characterID);
            // add any pending additive choices and clear list
            if(_pendingAdditiveChoices.Count > 0)
            {
                choices.AddRange(_pendingAdditiveChoices);
                _pendingAdditiveChoices.Clear();
            }

            ProcessDialogueChoicesAndConfirm(choices);

            // finally add node to player progress.
            
            
        }

        private void ProcessDialogueChoicesAndConfirm(IEnumerable<NodeLinkData> choices)
        {
            // flag to catch any passed roll checks within a single groupTagID in this dialogue node
            bool hasPassedGroupTagID = false;
            int buttonCreatedCount = 0;

            foreach (var choice in choices)
            {
                var buttonText = GetReturnedText(choice.TargetNodeGuid, choice.PortName); // check for any 'returning' text set with | delimiter
                
                // check if target is a roll node, and if so add skill prefix
                if(dialogue.RollNodeData.Exists( x => x.nodeGuid == choice.TargetNodeGuid))
                { 
                    // if we've already found a matched success, exit early.
                    if(hasPassedGroupTagID) continue;
                    
                    var rollData = dialogue.RollNodeData.Find(x => x.nodeGuid == choice.TargetNodeGuid);
                    var skillPrefix = rollData.rollSkillName;
                    var rollDifficulty = rollData.rollDifficulty;
                    var skillHex = globalConfig.UI.SkillTextHexColor;

                    var fancyButton = $"[ROLL <size=25%><color={skillHex}>{skillPrefix}</color></size>] <i>(Difficulty: {rollDifficulty})</i>\n<b>{buttonText}</b>";

                    // first check if player has attempted check before
                    if(_playerProgress.RollCheckGroupEntryExists(rollData.rollGroupTagID))
                    {
                        // check if player has passed one of any roll checks within this groupTagID
                        if(_playerProgress.hasPlayerPassedRollCheck(rollData.rollGroupTagID))
                        {
                            hasPassedGroupTagID = true; // set flag to avoid duplicate entries
                            buttonText = "<b>[PASSED]</b> " + rollData.passedDescription;
                        }
                        else if (rollData.isRepeatable) // failed but can repeat
                        {
                            buttonText = fancyButton;
                        }
                        else 
                        {
                            continue; // if failed and not repeatable, remove the option. BE SURE TO ADD OTHER OPTIONS. 
                        }
                    }
                    else
                    {
                        // otherwise setup the fancy roll check button
                        buttonText = fancyButton;
                    }
                }
                
                // let UI manager handle button creation and logic for any standard non-roll choices
                controller.UIManager.CreateDialogueOptionButton(choice.TargetNodeGuid, buttonText);
                buttonCreatedCount++;
                
            }

            // if there are no dialogue choices left/ flag end reached
            if(choices.Count() < 1 || buttonCreatedCount < 1)  
            {
                onDialogueReachedDeadEnd?.Invoke();
                return;
            }

            // finally create confirm button
            controller.UIManager.initConfirmActionButton();
        }

        private void ProcessAdditiveParagraphNode(string narrativeDataGUID, string text, string characterID, bool autoProgress)
        {
            // var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGuid == narrativeDataGUID);
            // simply add the paragraph, "continue dialogue "button, and proceed to next node
            // UIManager.Instance.CreateContentParagraph(text);
             AddTextToUI(text, characterID);

            // add logic to create / call continue button here

            if(!autoProgress)
            {
                // if no auto progress, create button for manual progress to next node
                UIManager.Instance.SetAdditiveDialogueState(); 
            }
            else 
            {
                // otherwise auto go to next node.
                ProceedToNextNode();
            }
            
        }

        private string GetReturnedText(string nodeGUID, string text)
        {
            // check for | delimiter for returned text
            if(text.Contains("|"))
            {
                var pos = text.IndexOf("|"); // if in history, return after | delimiter.
                if(PlayerProgressTracker.Instance.DialogueEntryExists(nodeGUID)) return text.Substring(pos + 1);
                else return text.Substring(0, pos); // if not, return first portion up to delimiter.
            }
            return text; // if none found or not in history, return normal text before | delimiter.
        }

        private void AddTextToUI(string text, string characterID)
        {
            

            if(characterID != null)
            {
                if(characterID.Length > 0)
                {
                    // first check for common speakers
                    if(_commonSpeakers.Exists(x => x.characterID == characterID))
                    {
                        var speaker = _commonSpeakers.Find(x => x.characterID == characterID);
                        var speakerName = $"<style=h3>{speaker.speakerName}</style>\n";
                        UIManager.Instance.CreateContentPortraitParagraph(speakerName + text, speaker.speakerPortrait);
                        return;
                    }
                    
                    // otherwise check active case profile
                    var charProfile = CaseManager.Instance.GetProfileByID(characterID);
                    if(charProfile != null)
                    {
                        var speakerName = $"<style=h3>{charProfile.characterName}</style>\n";
                        UIManager.Instance.CreateContentPortraitParagraph(speakerName + text, charProfile.portrait.portraitSprite);
                        return;
                    }
                }
            }
            // if neither is true, craete regular paragraph
            UIManager.Instance.CreateContentParagraph(text);
            
        }

        private void ProceedToNextNode()
        {
            var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGuid == _currentNodeGUID);
            ProceedToNarrative(choices.ElementAt(0).TargetNodeGuid);
        }

        private void ProcessAdditiveChoiceNode(string narrativeDataGUID)
        {
            var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGuid == narrativeDataGUID);
            string targetDialogueGUID = choices.ToList().Find(x => x.PortName.Contains("Dialogue Node To Add To")).TargetNodeGuid;
            foreach(var choice in choices)
            {
                if(!choice.PortName.Contains("Dialogue Node To Add To")) _pendingAdditiveChoices.Add(choice);
            }
            // once looped, proceed to the next node.
            ProceedToNarrative(targetDialogueGUID);
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
            if(_playerProgress.RollCheckGroupEntryExists(rollGroupTagID))
            {
                var hasPassed = _playerProgress.hasPlayerPassedRollCheck(rollGroupTagID);

                if(hasPassed) ProceedToNarrative(outputs.ElementAt(5).TargetNodeGuid); // already passed, can't repeat
                else if (!isRepeatable) ProceedToNarrative(outputs.ElementAt(6).TargetNodeGuid); // already failed, can't repeat
                else checkRollManager.Instance.InitializeRollCheck(rollSkillName, rollDescription, rollDifficulty); // can repeat after fail
                
            }
            else 
            {
                // initialize the roll check UI and process for first time
                checkRollManager.Instance.InitializeRollCheck(rollSkillName, rollDescription, rollDifficulty);
            }
        }

        private void ProcessRollResult(bool rollResult, rollCheckResultType resultType)
        {
            var data = dialogue.RollNodeData.Find(x => x.nodeGuid == _currentNodeGUID);
            var outputs = dialogue.NodeLinks.Where(x => x.BaseNodeGuid == _currentNodeGUID);

            // add record to player progress
            _playerProgress.AddRollCheckEntry(
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
            if(rollResult) _playerProgress.PassAllRollChecksByGroupTagID(data.rollGroupTagID);
            

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

            // check player progess to see if event is already entered
            var hasTriggeredAlready = PlayerProgressTracker.Instance.hasEventTriggeredAlready(data.nodeGuid);
            
            // add logic to check event specifics
            if(!hasTriggeredAlready || data.isRepeatable) {
                // trigger event
                Debug.Log("DIALOGUE PARSER ---- Triggering Event!");

                // add event to history (progress will ignore if entry exists already)
                PlayerProgressTracker.Instance.AddEventEntry(data);

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
                    if(eventName != "skiptodialogue") DisableDialogueParser(); // do not disable when skipping to dialogue
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
            var checkType = data.checkType;
            var comparisonOperator = data.comparisonOperator;
            // if dead end, call and return
            if(outputs.Count() < 1) {
                Debug.Log("Check node found with no outputs. Please check this dialogue graph.");
                Debug.Log("Graph: " + dialogue);
                Debug.Log("NodeGUID: " + narrativeDataGUID);
                onDialogueReachedDeadEnd?.Invoke();
                return;
            }

            var result = checkManager.GetCheckResult(checkType, checkName, checkValue,comparisonOperator);
            if(result) ProceedToNarrative(outputs.ElementAt(0).TargetNodeGuid);
            else ProceedToNarrative(outputs.ElementAt(1).TargetNodeGuid);
        }

        private void ProcessEventDeadEnd()
        {
            Debug.Log("DIALOGUE PARSER ---- Dead end reached on event node. Updating content and clearing exist buttons before invocation.");
            // controller.UIManager.updateContentText("");
            controller.UIManager.ClearContentAndButtons();
            onDialogueReachedDeadEnd?.Invoke();
        }

        private void ProcessEndpointNode(string narrativeDataGUID)
        {
            Debug.Log("End point reached. Jumping to start point.");
            var tagID = dialogue.EndpointNodeData.Find(x => x.nodeGuid == narrativeDataGUID).nodeLinkTagID;
            var startNodeGUID = dialogue.EndpointNodeData.Find(x => x.nodeLinkTagID == tagID && x.isLinkStart).nodeGuid;
            var destinationNodeGUID = dialogue.NodeLinks.Find(x => x.BaseNodeGuid == startNodeGUID).TargetNodeGuid;
            ProceedToNarrative(destinationNodeGUID);
            // onDialogueReachedDeadEnd?.Invoke();
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
