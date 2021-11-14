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

    [SerializeField] private CaseScenario _activeCase;

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

    public void GetNewCase(CaseScenario newScenario)
    {
        _activeCase = newScenario;
        
    }

    public void SetupCaseMap()
    {
        OnNewCaseMap?.Invoke(_activeCase.GetCaseMap());
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
        _victimData.VictimName = _activeCase.GetVictim().GetVictimName();
        _victimData.VictimPortrait = _activeCase.GetVictim().GetVictimPortrait();
        _victimData.VictimSummary = _activeCase.GetVictim().GetVictimSummary();
        _victimData.VictimAge = _activeCase.GetVictim().GetVictimAge();
        _victimData.VictimResidence = _activeCase.GetVictim().GetVictimResidence();
        _victimData.VictimOccupation = _activeCase.GetVictim().GetVictimOccupation();
        _victimData.CauseOfDeath = _activeCase.GetVictim().GetCauseOfDeath();
        _victimData.TimeofDeath = _activeCase.GetVictim().GetTimeOfDeath();
        _victimData.LocationOfDeath = _activeCase.GetVictim().GetLocationOfDeath();
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

    public CharacterProfileData GetStartingCharacterProfileData(string characterName)
    {
        CharacterProfileData _characterData = new CharacterProfileData();
        List<CaseCharacterProfile> _caseCharacters = _activeCase.GetCharacterProfiles();
        // match to (hidden) character name and popualte starting values;
        var matchedCharacter = _caseCharacters.Find(x => x.GetCharacterName(true).ToLower() == characterName.ToLower());
        _characterData.characterName = matchedCharacter.GetCharacterName();
        _characterData.portrait = matchedCharacter.GetPortrait();
        _characterData.summary = matchedCharacter.GetSummary();
        _characterData.age = matchedCharacter.GetAge();
        _characterData.residence = matchedCharacter.GetResidence();
        _characterData.occupation = matchedCharacter.GetOccupation();
        _characterData.relationshipToVictim = matchedCharacter.GetRelationShipToVictim();
        _characterData.characterType = matchedCharacter.GetCharacterType();
    
        return _characterData;
    }

    public CharacterPortrait UncoverCharacterPortrait(string characterName)
    {
        List<CaseCharacterProfile> _caseCharacters = _activeCase.GetCharacterProfiles();
        var matchedCharacter = _caseCharacters.Find(x => x.GetCharacterName(true) == characterName);
        return matchedCharacter.RevealCharacterPortrait();
    }

    public string UncoverCharacterProperty(string characterName, string propertyName)
    {
        List<CaseCharacterProfile> _caseCharacters = _activeCase.GetCharacterProfiles();
        var matchedCharacter = _caseCharacters.Find(x => x.GetCharacterName(true) == characterName);
        return matchedCharacter.RevealCharacterProperty(propertyName);
    }

    public List<CaseEvidence> GetAvailableEvidence()
    {
        return _activeCase.GetAvailableEvidence();
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
