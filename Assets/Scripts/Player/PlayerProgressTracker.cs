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
    [SerializeField] private List<NavObjectEntry> _visitedNavObjects = new List<NavObjectEntry>();


    [Header("Case Data")]
    [SerializeField] private List<PlayerCaseRecord> completedCaseRecords = new List<PlayerCaseRecord>();

    [Header("Roll Check Data")]
    public List<RollCheckEntry> rollCheckEntries = new List<RollCheckEntry>();    


    // accessor properties
    public string CurrentScene { get { return _currentScene; } set { _currentScene = value; } }
    public string CurrentAreaName { get { return _currentAreaName; } set { _currentAreaName = value; } }

    private void Awake() {
        //init singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

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
}
