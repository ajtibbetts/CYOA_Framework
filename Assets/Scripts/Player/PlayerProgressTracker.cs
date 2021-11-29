using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using globalDataTypes;
using CaseDataObjects;

public class PlayerProgressTracker : MonoBehaviour
{
    private static PlayerProgressTracker _instance;
    public static PlayerProgressTracker Instance { get { return _instance; } }

    [Header("World Level Data")]
    [SerializeField] private string _currentScene;
    [SerializeField] private string _currentAreaName;
    [SerializeField] private string _currentNavObjectGUID;
    [SerializeField] private string _previousScene;
    [SerializeField] private string _previousAreaName;
    [SerializeField] private string _previousNavObjectGUID;
    [SerializeField] private List<NavObjectEntry> _visitedNavObjects = new List<NavObjectEntry>();

    [Header("Dialoge Data")]
    [SerializeField] private List<DialogueNodeData> _dialogueNodeEntries = new List<DialogueNodeData>();

    [Header("Roll Check Data")]
    public List<RollCheckEntry> _rollCheckEntries = new List<RollCheckEntry>();   

    [Header("Case Data")]
    [SerializeField] private List<caseRecordEntry> _completedCaseRecords = new List<caseRecordEntry>();

    [Header("Progression Flags")]
    [SerializeField] private List<string> _storyFlagIDs = new List<string>();
    [SerializeField] private List<string> _worldFlagIDs = new List<string>();
    [Header("Event Entries")]
    [SerializeField] private List<EventNodeData> _eventHistory = new List<EventNodeData>();


    // accessor properties
    public string CurrentScene { get { return _currentScene; } set { _currentScene = value; } }
    public string CurrentAreaName { get { return _currentAreaName; } set { _currentAreaName = value; } }
    public string CurrentNavObjectGUID {get {return _currentNavObjectGUID;} set {_currentNavObjectGUID = value;}}

    public string PreviousScene {get {return _previousScene;} set {_previousScene = value;}}
    public string PreviousAreaName {get{return _previousAreaName;}set{_previousAreaName = value;}}
    public string PreviousNavObjectGUID {get{return _previousNavObjectGUID;}set{_previousNavObjectGUID = value;}}

    

    private void Awake() {
        //init singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    // NAV OBJECTS
    public void AddNavObject(NavObjectEntry navObjectEntry)
    {
        var entryExists = _visitedNavObjects.Exists(x => x.NavObjectGUID == navObjectEntry.NavObjectGUID);
        if(!entryExists) _visitedNavObjects.Add(navObjectEntry);
        else Debug.Log($"PLAYER PROGRESS ---- Nav Object GUID:{navObjectEntry.NavObjectGUID} / {navObjectEntry.NavObjectName} already exists in history.");
    }

    public bool NavObjectExistsInHistory(string navObjectGUID)
    {
        return _visitedNavObjects.Exists(x => x.NavObjectGUID == navObjectGUID);
    }

    public void UpdateNavObjectProperties(string navObjectGUID, List<localProperty> updatedProperties)
    {
        var navObjectToUpdate = _visitedNavObjects.Find(x => x.NavObjectGUID == navObjectGUID);
        if(navObjectToUpdate != null)
        {
            navObjectToUpdate.NavObjectProperties = updatedProperties;
        }
        else Debug.LogError($"PLAYER PROGRESS ---- Nav object {navObjectGUID} not found in history.");
    }

    public List<localProperty> GetNavObjectProperties(string navObjectGUID)
    {
        var entry = _visitedNavObjects.Find(x => x.NavObjectGUID == navObjectGUID);
        if(entry != null) return entry.NavObjectProperties;
        else {
            Debug.LogError("PROGRESS TRACKER ---- Failed to find nav object for GUID: " + navObjectGUID);
            return null;
        }
    }

    // dialogue node
    public void AddDialogueEntry(DialogueNodeData nodeData)
    {
        var exisitingEntry = _dialogueNodeEntries.Find(x => x.Guid == nodeData.Guid);
        if(exisitingEntry == null) _dialogueNodeEntries.Add(nodeData);
    }

    public bool DialogueEntryExists(string nodeGUID)
    {
        return _dialogueNodeEntries.Exists(x => x.Guid == nodeGUID);
    }


    // ROLL CHECKS
    public void AddRollCheckEntry(RollCheckEntry newRollCheckEntry)
    {
        var existingEntry = _rollCheckEntries.Find(x => x.rollNodeGUID == newRollCheckEntry.rollNodeGUID);
        if(existingEntry == null)
        {
             Debug.Log("Progress Tracker --- Adding new roll check entry.");
             _rollCheckEntries.Add(newRollCheckEntry);
        }
        else if(existingEntry.isRepeatable)
        {
            existingEntry.passedRoll = newRollCheckEntry.passedRoll;
            Debug.Log("Progress Tracker --- Updating repeatable existing roll check entry under GUID: " + existingEntry.rollNodeGUID);
        }
        else Debug.Log("Progress Tracker ---- Roll check entry already exists for GUID: " + newRollCheckEntry.rollNodeGUID);
    }

    public bool RollCheckGroupEntryExists(string rollGroupTagID)
    {
        return _rollCheckEntries.Exists(x => x.rollGroupTagID == rollGroupTagID);
    }

    public RollCheckEntry GetRollCheckEntry(string rollNodeGUID)
    {
        var entry = _rollCheckEntries.Find(x => x.rollNodeGUID == rollNodeGUID);
        if(entry != null) return entry;
        else {
            Debug.LogError("PROGRESS TRACKER ---- No roll check entry found for GUID: " + rollNodeGUID);
            return null;
        }
    }

    public bool hasPlayerPassedRollCheck(string rollGroupTagID)
    {
        var rollEntries = _rollCheckEntries.FindAll(x=> x.rollGroupTagID == rollGroupTagID);
        if(rollEntries.Count > 0)
        {
            foreach(RollCheckEntry rollEntry in rollEntries)
            {
                if(rollEntry.passedRoll) return true;
            }
        }
        return false;
    }

    public void PassAllRollChecksByGroupTagID(string rollGroupTagID)
    {
        // to avoid rolling with other skills on the same check group
        var rollEntries = _rollCheckEntries.FindAll(x => x.rollGroupTagID == rollGroupTagID);
        if(rollEntries.Count > 0)
        {   
            foreach(RollCheckEntry rollEntry in rollEntries)
            {
                rollEntry.passedRoll = true;
            }
        }
    }


    // CASE RECORD
    public void AddCaseRecordToHistory(PlayerCaseRecord caseRecord)
    {
        if(!_completedCaseRecords.Exists(x => x.caseID == caseRecord.activeCaseID))
        {
            Debug.Log("Progress Tracker --- Adding New Case Record: " + caseRecord.activeCaseID);

            _completedCaseRecords.Add(
                new caseRecordEntry
                {
                    caseID = caseRecord.activeCaseID,
                    victimData = caseRecord.GetVictim(),
                    primarySuspect = caseRecord.GetPrimarySuspect()
                }
            );
        }
        else Debug.Log("PROGRESS TRACKER ---- Case ID already in history: " + caseRecord.activeCaseID);
    }

    public caseRecordEntry GetCaseRecordFromHistory(string caseID)
    {
        var caseRecord = _completedCaseRecords.Find( x => x.caseID == caseID);
        if(caseRecord != null) return caseRecord;
        else 
        {
            Debug.LogError("PROGRESS TRACKER ---- Failed to review record for caseID: " + caseID);
            return null;
        }

    }


    // STORY & WORLD FLAGS

    public void AddStoryFlag(string flagID)
    {
        if(!_storyFlagIDs.Contains(flagID)) _storyFlagIDs.Add(flagID);
        else Debug.Log("PLAYER PROGRESS ---- Story flag already added: " + flagID);
    }

    public void RemoveStoryFlag(string flagID)
    {
        if(_storyFlagIDs.Contains(flagID)) _storyFlagIDs.Remove(flagID);
        else Debug.Log("PLAYER PROGRESS ---- Story flag does not exist. Cannot remove: " + flagID);
    }

    public bool HasStoryFlag(string flagID)
    {
        return _storyFlagIDs.Contains(flagID);
    }

    public void AddWorldFlag(string flagID)
    {
        if(!_storyFlagIDs.Contains(flagID)) _storyFlagIDs.Add(flagID);
        else Debug.Log("PLAYER PROGRESS ---- Story flag already added: " + flagID);
    }

    public void RemoveWorldFlag(string flagID)
    {
        if(_storyFlagIDs.Contains(flagID)) _storyFlagIDs.Remove(flagID);
        else Debug.Log("PLAYER PROGRESS ---- Story flag does not exist. Cannot remove: " + flagID);
    }

    public bool HasWorldFlag(string flagID)
    {
        return _storyFlagIDs.Contains(flagID);
    }

    // Events

    public void AddEventEntry(EventNodeData newEntry)
    {
        // currently we only care about non-repeatable events for entry
        if(newEntry.isRepeatable) return;

        var existingEntry = _eventHistory.Find(x => x.nodeGuid == newEntry.nodeGuid);
        if(existingEntry == null)
        {
            Debug.Log("Progress Tracker --- Adding new event entry.");
            _eventHistory.Add(newEntry);
        }
        else Debug.Log("Progress Tracker ---- Event entry already exists for GUID: " + newEntry.nodeGuid);
    }
    public bool hasEventTriggeredAlready(string eventGUID)
    {
        var existingEntry = _eventHistory.Find(x => x.nodeGuid == eventGUID);
        if(existingEntry != null) return true;
        return false;
    }

    public bool isEventRepeatable(string eventGUID)
    {
        var existingEntry = _eventHistory.Find(x => x.nodeGuid == eventGUID);
        if(existingEntry != null) return existingEntry.isRepeatable;
        return false;
    }
}
