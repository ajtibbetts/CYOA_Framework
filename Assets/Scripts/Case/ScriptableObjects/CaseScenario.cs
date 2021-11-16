using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CaseScenario", menuName = "CYOA/Case/CaseScenario", order = 0)]
public class CaseScenario : ScriptableObject {
    [SerializeField] private string _caseID;
    [SerializeField] private string _caseTitle;
    [SerializeField] [TextArea] private string _caseSummary;
    [SerializeField] private CaseVictim _caseVictim;
    [SerializeField] private CaseCulprit _caseCulprit;
    [SerializeField] private List<CaseLead> _startingLeads;
    [SerializeField] private List<CaseLead> _availableLeads;
    [SerializeField] private List<CaseCharacterProfile> _startingCharacterProfiles;
    [SerializeField] private List<CaseCharacterProfile> _availableCharacterProfiles;
    [SerializeField] private List<CaseEvidence> _startingCaseEvidence;
    [SerializeField] private List<CaseEvidence> _availableCaseEvidence;
    [SerializeField] private MapObject _caseMap;


    public string GetCaseID()
    {
        return _caseID;
    }

    public string GetCaseTitle()
    {
        return _caseTitle;
    }

    public string GetCaseSummary()
    {
        return _caseSummary;
    }

    public CaseVictim GetVictim()
    {
        return _caseVictim;
    }

    public CaseCulprit GetCulprit()
    {
        return _caseCulprit;
    }

    public List<CaseLead> GetStartingLeads()
    {
        return _startingLeads;
    }

    public List<CaseLead> GetAvailableLeads()
    {
        return _availableLeads;
    }

    public List<CaseCharacterProfile> GetStartingProfiles()
    {
        return _startingCharacterProfiles;
    }

    public List<CaseCharacterProfile> GetAvailableProfiles()
    {
        return _availableCharacterProfiles;
    }

    public List<CaseEvidence> GetStartingEvidence()
    {
        return _startingCaseEvidence;
    }

    public List<CaseEvidence> GetAvailableEvidence()
    {
        return _availableCaseEvidence;
    }

    public MapObject GetCaseMap()
    {
        return _caseMap;
    }
}
