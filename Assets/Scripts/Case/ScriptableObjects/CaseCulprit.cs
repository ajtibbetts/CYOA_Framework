using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaseDataObjects;
using System.Linq;



[CreateAssetMenu(fileName = "CaseCulprit", menuName = "CYOA/Case/Culprit", order = 0)]
public class CaseCulprit : ScriptableObject {
    [SerializeField] private CaseCharacterProfile _suspectProfile;
    [SerializeField] private SuspectTheory _means;
    [SerializeField] private SuspectTheory _motive;
    [SerializeField] private SuspectTheory _opportunity;

    public static event Action<List<string>> OnCulpritTheoryAssessed;
    private List<string> _culpritTheoryResponses = new List<string>();



    public bool IsCulpritTheoryValid(CaseSuspect suspect)
    {
        TheoryResults results = new TheoryResults();
        // run through all criteria and add results
        if(isEvidenceValid(suspect.ProposedMeans, _means))
        {
            results.MatchedMeans = true;
        }

        if(isEvidenceValid(suspect.ProposedMotive, _motive))
        {
            results.MatchedMotive = true;
        }

        if(isEvidenceValid(suspect.ProposedOpportunity, _opportunity))
        {
            results.MatchedOpportunity = true;
        }
        
        // check for culprit match
        if(suspect.SuspectProfile.characterName == _suspectProfile.GetCharacterName(true))
        {
            results.MatchedSuspect = true;
            // if any of the evidence was correct, add complete response. else add partial hint
            if(results.IsTheoryValid())
            {
                _culpritTheoryResponses.Add(suspect.SuspectProfile.AsCulpritCompleteResponse);
            }
            else
            {
                _culpritTheoryResponses.Add(suspect.SuspectProfile.AsCulpritPartialResponse);
            }
        }
        else 
        {
            // if wrong culprit
            _culpritTheoryResponses.Add(suspect.SuspectProfile.AsInnocentResponse);
        }



        // send responses to UI and reset list
        OnCulpritTheoryAssessed?.Invoke(_culpritTheoryResponses);
        _culpritTheoryResponses.Clear();
        return results.IsTheoryValid();
    }

    public bool isEvidenceValid(CaseEvidence proposedEvidence, SuspectTheory theoryType)
    {
        List<CaseTheory> ValidTheories = theoryType.ValidTheories;
        List<CaseTheory> InvalidTheories = theoryType.InvalidTheories;

        // check for valid evidenciary match first
        var validEvidence = ValidTheories.Find(x => x.evidence.GetEvidenceName() == proposedEvidence.GetEvidenceName()); 
        if(validEvidence.evidence != null)
        {
            _culpritTheoryResponses.Add(validEvidence.response);
            return true;
        }

        // check for invalid evidence that may offer feedback
        var invalidEvidence = InvalidTheories.Find(x => x.evidence.GetEvidenceName() == proposedEvidence.GetEvidenceName()); 
        if(validEvidence.evidence != null)
        {
            _culpritTheoryResponses.Add(invalidEvidence.response);
            return false;
        }

        // if evidence is not found in either, get generic failed response.
        _culpritTheoryResponses.Add("I don't know how this evidence would factor in at all.");
        return false;
    }

}
