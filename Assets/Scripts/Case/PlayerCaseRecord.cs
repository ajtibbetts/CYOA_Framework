using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CaseDataObjects;

public class PlayerCaseRecord : MonoBehaviour
{
    private static PlayerCaseRecord _instance;
    public static PlayerCaseRecord Instance { get { return _instance; } }

    // events
    public static event Action OnCaseDataUpdated;
    
    [SerializeField] private VictimData _victim;
    [SerializeField] private List<ActiveLead> _leads = new List<ActiveLead>();
    [SerializeField] private List<CharacterProfileData> _profiles = new List<CharacterProfileData>();
    [SerializeField] private List<CaseSuspect> _suspects = new List<CaseSuspect>();
    [SerializeField] private List<CaseEvidence> _evidence = new List<CaseEvidence>();
    [SerializeField] private List<string> _customNotes = new List<string>();
    
    
    
    
    private void Awake() {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        // to test, setup new case
        StartNewCase();
    }

    void StartNewCase()
    {
        Debug.Log("Starting new case.");
        ClearCollectionsForNewCase();
        _victim = CaseManager.Instance.GetStartingVictimData();
    }

    void ClearCollectionsForNewCase()
    {
        _leads.Clear();
        _profiles.Clear();
        _suspects.Clear();
        _evidence.Clear();
        _customNotes.Clear();
    }


    // these will be called by external event we'll subscribe to from event manager
    private void DiscoverVictimData(string propertyName)
    {
        if(propertyName == "portrait")
        {
            _victim.VictimPortrait = CaseManager.Instance.UncoverVictimPortrait();
            OnCaseDataUpdated?.Invoke();
            return;
        }
        else
        {
            PropertyInfo propertyInfo = _victim.GetType().GetProperty(propertyName);
            string DiscoveredValue = CaseManager.Instance.UncoverVictimProperty(propertyName);
            propertyInfo.SetValue(_victim, Convert.ChangeType(DiscoveredValue, propertyInfo.PropertyType), null);
            OnCaseDataUpdated?.Invoke();
            return;
        }
    }

    
    private void AddLead(CaseLead lead)
    {
        ActiveLead newLead= new ActiveLead();
        newLead.lead = lead;
        newLead.isResolved = false;
        _leads.Add(newLead);
        OnCaseDataUpdated?.Invoke();
    }

    private void ResolveLead(CaseLead lead)
    {
        var matchedLead = _leads.Find(x => x.lead.GetLead() == lead.GetLead());
        if(matchedLead.lead.GetLead() != null)
        {
            matchedLead.isResolved = true;
            OnCaseDataUpdated?.Invoke();
        }
    }

    private void AddProfile(string characterName)
    {
        CharacterProfileData _newCharacterData = CaseManager.Instance.GetStartingCharacterProfileData(characterName);
        _profiles.Add(_newCharacterData);
        OnCaseDataUpdated?.Invoke();
    }

    private void DiscoverProfileData(string characterName, string propertyName)
    {
        var characterToUpdate = _profiles.Find(x => x._characterName == characterName);
        
        if(propertyName == "portrait")
        {
            characterToUpdate._portrait = CaseManager.Instance.UncoverCharacterPortrait(characterName);
            OnCaseDataUpdated?.Invoke();
            return;
        }
        else
        {
            PropertyInfo propertyInfo = characterToUpdate.GetType().GetProperty(propertyName);
            string DiscoveredValue = CaseManager.Instance.UncoverCharacterProperty(characterName,propertyName);
            propertyInfo.SetValue(characterToUpdate, Convert.ChangeType(DiscoveredValue, propertyInfo.PropertyType), null);
            OnCaseDataUpdated?.Invoke();
            return;
        }
    }

    private void AddProfileToSuspects(CharacterProfileData characterProfile)
    {
        CaseSuspect newSuspect = new CaseSuspect(characterProfile);
        _suspects.Add(newSuspect);
    }

    private void RemoveProfileFromSuspects(CaseSuspect suspect)
    {
        _suspects.Remove(suspect);
    }

    private void UpdateSuspectProfile(CaseSuspect suspect, string profileType, CaseEvidence evidence)
    {
        switch(profileType.ToLower())
        {
            case "means":
                suspect.ProposedMeans = evidence;
            break;
            case "motive":
                suspect.ProposedMotive = evidence;
            break;
            case "opportunity":
                suspect.ProposedOpportunity = evidence;
            break;
            default:
            break;
        }
    }

    
    private void AddEvidence(CaseEvidence evidence)
    {
        _evidence.Add(evidence);
    }
    
    private void SubmitProposedCase()
    {
        // add stuff here tomorrow
    }

    private void AddCustomNote(string note)
    {
        _customNotes.Add(note);
    }

}
