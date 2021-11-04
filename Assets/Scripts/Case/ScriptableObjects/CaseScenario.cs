using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CaseScenario", menuName = "CYOA/Case/CaseScenario", order = 0)]
public class CaseScenario : ScriptableObject {
    [SerializeField] private string _caseTitle;
    [SerializeField] [TextArea] private string _caseSummary;
    [SerializeField] private CaseVictim _caseVictim;
    [SerializeField] private CaseSuspect _caseCulprit;
    [SerializeField] private CaseLead[] _startingLeads;


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

    public CaseSuspect GetCulprit()
    {
        return _caseCulprit;
    }

    public CaseLead[] GetStartingLeads()
    {
        return _startingLeads;
    }
}
