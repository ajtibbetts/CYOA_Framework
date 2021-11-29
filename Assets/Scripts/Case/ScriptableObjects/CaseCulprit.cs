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

    [SerializeField] private string _genericFailResponse = "I don't know how this evidence would factor in at all.";

    // public static event Action<List<string>> OnCulpritTheoryAssessed;
    private List<string> _culpritTheoryResponses = new List<string>();

    private TheoryResults _warrantResults = new TheoryResults();

    public void SetSuspectTheoryResults(CaseSuspect suspect)
    {
        _warrantResults = new TheoryResults();
        SetEvidenceResult(suspect.ProposedMeans, _means, EvidenceType.MEANS);
        SetEvidenceResult(suspect.ProposedMotive, _motive, EvidenceType.MOTIVE);
        SetEvidenceResult(suspect.ProposedOpportunity, _opportunity, EvidenceType.OPPORTUNITY);
        SetSuspectResult(suspect);
    }

    public bool GetTheoryResults()
    {
        return _warrantResults.IsTheoryValid();
    }

    public string GetTheoryResponse(EvidenceType type)
    {
        switch(type)
        {
            case EvidenceType.MEANS:
                return _warrantResults.ResponseMeans;
            case EvidenceType.MOTIVE:
                return _warrantResults.ResponseMotive;
            case EvidenceType.OPPORTUNITY:
                return _warrantResults.ResponseOpportunity;
            default:
                return _warrantResults.ResponseSuspect;
        }
    }

    public bool isEvidenceValid(CaseEvidence proposedEvidence, SuspectTheory theoryType)
    {
        List<CaseTheory> ValidTheories = theoryType.ValidTheories;
        List<CaseTheory> InvalidTheories = theoryType.InvalidTheories;

        // check for valid evidenciary match first
        var validEvidence = ValidTheories.Find(x => x.evidence.GetEvidenceName() == proposedEvidence.GetEvidenceName()); 
        if(validEvidence.evidence != null)
        {
            // _culpritTheoryResponses.Add(validEvidence.response);
            return true;
        }

        // check for invalid evidence that may offer feedback
        var invalidEvidence = InvalidTheories.Find(x => x.evidence.GetEvidenceName() == proposedEvidence.GetEvidenceName()); 
        if(validEvidence.evidence != null)
        {
            // _culpritTheoryResponses.Add(invalidEvidence.response);
            return false;
        }

        // if evidence is not found in either, get generic failed response.
        // _culpritTheoryResponses.Add("I don't know how this evidence would factor in at all.");
        return false;
    }

    public void SetEvidenceResult(CaseEvidence proposedEvidence, SuspectTheory theoryType, EvidenceType evidenceType)
    {
        List<CaseTheory> ValidTheories = theoryType.ValidTheories;
        List<CaseTheory> InvalidTheories = theoryType.InvalidTheories;

        // check for valid evidenciary match first
        var validEvidence = ValidTheories.Find(x => x.evidence.GetEvidenceName() == proposedEvidence.GetEvidenceName()); 
        if(validEvidence.evidence != null)
        {
            // _culpritTheoryResponses.Add(validEvidence.response);
            _warrantResults.SetTheoryResult(evidenceType, true, validEvidence.response);
            return;
        }

        // check for invalid evidence that may offer feedback
        var invalidEvidence = InvalidTheories.Find(x => x.evidence.GetEvidenceName() == proposedEvidence.GetEvidenceName()); 
        if(validEvidence.evidence != null)
        {
            _warrantResults.SetTheoryResult(evidenceType, false, invalidEvidence.response);
            return;
        }

        // if evidence is not found in either, get generic failed response.
        _warrantResults.SetTheoryResult(evidenceType, false, _genericFailResponse);
        return;
    }

    public void SetSuspectResult(CaseSuspect suspect)
    {
        if(suspect.SuspectProfile.characterID == _suspectProfile.GetCharacterID())
        {
            _warrantResults.MatchedSuspect = true;
            if(_warrantResults.IsTheoryValid())
            {
                _warrantResults.SetTheoryResult(EvidenceType.UNASSIGNED, true, suspect.SuspectProfile.AsCulpritCompleteResponse);
            }
            else 
            {
                _warrantResults.SetTheoryResult(EvidenceType.UNASSIGNED, true, suspect.SuspectProfile.AsCulpritPartialResponse);
            }
        }
        else 
        {
            // if wrong culprit
            Debug.Log("CASE CULPRIT --- ID NOT MATCH. message: " + suspect.SuspectProfile.AsInnocentResponse);
            _warrantResults.SetTheoryResult(EvidenceType.UNASSIGNED, false, suspect.SuspectProfile.AsInnocentResponse);
            return;
        }
    }

}
