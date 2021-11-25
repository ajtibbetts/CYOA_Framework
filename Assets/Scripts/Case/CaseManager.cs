using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaseDataObjects;

public class CaseManager : MonoBehaviour
{
    
    private static CaseManager _instance;
    public static CaseManager Instance { get { return _instance; } }


    public static event Action<MapObject> OnNewCaseMap;
    public static event Action OnNewCaseSet;

    [SerializeField] private CaseScenario _activeCase;
    [SerializeField] private List<CaseScenario> _availableCases = new List<CaseScenario>();
    [SerializeField] private CharacterPortrait _unknownPortrait;

    private void Awake() {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        // add listener for warrant proposal results
        CaseCulprit.OnCulpritTheoryAssessed += OutputWarrantResults;
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    public void StartNewCase(string caseID)
    {
        var caseToStart = _availableCases.Find(x => x.GetCaseID().ToLower() == caseID.ToLower());
        if(caseToStart != null)
        {
            _activeCase = caseToStart;
            SetupCaseMap();
            OnNewCaseSet?.Invoke();
        }
        else Debug.LogError("CASE MANAGER ---- FAILED TO START NEW CASEID: " + caseID);
    }

    public string GetActiveCaseID()
    {
        return _activeCase.GetCaseID();
    }

    public void SetupCaseMap()
    {
        OnNewCaseMap?.Invoke(_activeCase.GetCaseMap());
    }

    public List<MapLocationObject> GetMapLocations()
    {
        return _activeCase.GetCaseMap().GetLocations();
    }

    public string GetActiveCaseTitle()
    {
        return _activeCase.GetCaseTitle();
    }

    public string GetActiveCaseSummary()
    {
        return _activeCase.GetCaseSummary();
    }

    public VictimData GetStartingVictimData()
    {
        VictimData _victimData = new VictimData();
        _victimData.name = _activeCase.GetVictim().GetVictimName();
        if(_activeCase.GetVictim().GetVictimPortrait() == null) _victimData.portrait = _unknownPortrait;
        else _victimData.portrait = _activeCase.GetVictim().GetVictimPortrait();
        _victimData.summary = _activeCase.GetVictim().GetVictimSummary();
        _victimData.age = _activeCase.GetVictim().GetVictimAge();
        _victimData.residence = _activeCase.GetVictim().GetVictimResidence();
        _victimData.occupation = _activeCase.GetVictim().GetVictimOccupation();
        _victimData.causeOfDeath = _activeCase.GetVictim().GetCauseOfDeath();
        _victimData.timeOfDeath = _activeCase.GetVictim().GetTimeOfDeath();
        _victimData.locationOfDeath = _activeCase.GetVictim().GetLocationOfDeath();
        _victimData.AdditionalInjuries = _activeCase.GetVictim().GetAdditionalInjuries();
        _victimData.AdditionalNotes = _activeCase.GetVictim().GetAdditionalNotes();
        return _victimData;
    }

    public CharacterPortrait UncoverVictimPortrait()
    {
        return _activeCase.GetVictim().RevealVictimPortrait();
    }

    public string UncoverVictimProperty(string propertyName)
    {
        return _activeCase.GetVictim().RevealVictimProperty(propertyName);
    }

    public List<CharacterProfileData> GetStartingCharacterProfiles()
    {
        List<CharacterProfileData> startingProfiles = new List<CharacterProfileData>();
        var startingProfilesFromScenario = _activeCase.GetStartingProfiles();
        foreach(CaseCharacterProfile profile in startingProfilesFromScenario)
        {
            startingProfiles.Add(GetStartingCharacterProfileData(profile.GetCharacterID()));
        }

        return startingProfiles;
    }

    public CharacterProfileData GetStartingCharacterProfileData(string characterID)
    {
        CharacterProfileData _characterData = new CharacterProfileData();
        List<CaseCharacterProfile> _caseCharacters = _activeCase.GetAvailableProfiles();
        // match to (hidden) character name and popualte starting values;
        var matchedCharacter = _caseCharacters.Find(x => x.GetCharacterID().ToLower() == characterID.ToLower());
        if(matchedCharacter != null)
        {
            _characterData.characterID = matchedCharacter.GetCharacterID();
            _characterData.characterName = matchedCharacter.GetCharacterName();
            if(matchedCharacter.GetPortrait() == null) _characterData.portrait = _unknownPortrait;
            else _characterData.portrait = matchedCharacter.GetPortrait();
            _characterData.summary = matchedCharacter.GetSummary();
            _characterData.age = matchedCharacter.GetAge();
            _characterData.residence = matchedCharacter.GetResidence();
            _characterData.occupation = matchedCharacter.GetOccupation();
            _characterData.relationshipToVictim = matchedCharacter.GetRelationShipToVictim();
            _characterData.characterType = matchedCharacter.GetCharacterType();
    
            return _characterData;
        }
        else 
        {
            Debug.LogError("CASE MANAGER ---- FAILED TO GET STARTING PROFILE BY ID: " +characterID);
            return null;
        }
        
    }

    public CharacterPortrait UncoverCharacterPortrait(string characterID)
    {
        List<CaseCharacterProfile> _caseCharacters = _activeCase.GetAvailableProfiles();
        var matchedCharacter = _caseCharacters.Find(x => x.GetCharacterID().ToLower() == characterID.ToLower());
        return matchedCharacter.RevealCharacterPortrait();
    }

    public string UncoverCharacterProperty(string characterID, string propertyName)
    {
        List<CaseCharacterProfile> _caseCharacters = _activeCase.GetAvailableProfiles();
        var matchedCharacter = _caseCharacters.Find(x => x.GetCharacterID().ToLower() == characterID.ToLower());
        return matchedCharacter.RevealCharacterProperty(propertyName);
    }

    public CharacterProfileData GetProfileByID(string characterID)
    {
        CharacterProfileData _characterData = new CharacterProfileData();
        List<CaseCharacterProfile> _caseCharacters = _activeCase.GetAvailableProfiles();
        // match to (hidden) character name and popualte starting values;
        var matchedCharacter = _caseCharacters.Find(x => x.GetCharacterID().ToLower() == characterID.ToLower());
        if(matchedCharacter != null)
        {
            _characterData = matchedCharacter.GetFullData();
            return _characterData;
        }
        else
        {
            Debug.LogError("CASE MANAGER --- FAILED to get data for charID: " + characterID);
            return null;
        }
    }

    public List<CaseEvidence> GetStartingEvidence()
    {
        return _activeCase.GetStartingEvidence();
    }

    public List<CaseEvidence> GetAvailableEvidence()
    {
        return _activeCase.GetAvailableEvidence();
    }

    public List<ActiveLead> GetStartingLeads()
    {
        List<ActiveLead> startingLeads = new List<ActiveLead>();
        var initialLeads = _activeCase.GetStartingLeads();
        foreach(CaseLead initialLead in initialLeads)
        {
            ActiveLead newLead= new ActiveLead();
            newLead.lead = initialLead;
            newLead.isResolved = false;
            startingLeads.Add(newLead);
        }


        return startingLeads;
    }

    public List<CaseLead> GetAvailableLeads()
    {
        return _activeCase.GetAvailableLeads();
    }

    public bool ValidateArrestWarrant(CaseSuspect suspect)
    {
        // check if suspect and evidence is match
        // add additional events/penalties as needed based on success/failure
        if(_activeCase.GetCulprit().IsCulpritTheoryValid(suspect))
        {
            return true;
        }
        else
        {
            return false;
        } 
    }

     public void OutputWarrantResults(List<string> resultsText)
    {
        // triggered after validate arrest warrrant is called from CaseCulprit
        // send this to UI in some manner
    }

    public void BeginInterrogation()
    {

    }

    public void CompleteCase()
    {

    }

   
    
}
