using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CaseScenario", menuName = "CYOA/Case/CaseScenario", order = 0)]
public class CaseScenario : ScriptableObject {
    [SerializeField] private string _caseTitle;
    [SerializeField] [TextArea] private string _caseSummary;
    [SerializeField] private CaseVictim _caseVictim;
    [SerializeField] private CaseCulprit _caseCulprit;
    [SerializeField] private List<CaseLead> _startingLeads;

    [SerializeField] private List<CaseCharacterProfile> _characterProfiles;


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

    public List<CaseCharacterProfile> GetCharacterProfiles()
    {
        return _characterProfiles;
    }
}
