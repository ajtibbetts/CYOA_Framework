using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaseDataObjects;
using System.Linq;



[CreateAssetMenu(fileName = "CaseSuspect", menuName = "CYOA/Case/Suspect", order = 0)]
public class CaseSuspect : ScriptableObject {
    [SerializeField] private CaseCharacterProfile _suspectProfile;
    [SerializeField] private SuspectTheory _means;
    [SerializeField] private SuspectTheory _motive;
    [SerializeField] private SuspectTheory _opportunity;


    public string ValidateTheory(CaseEvidence evidence, string theoryType)
    {
        switch(theoryType.ToLower())
        {
            case "means":
                GetTheoryResponse(evidence, _means);
            break;
            case "motive":
                GetTheoryResponse(evidence, _motive);
            break;
            case "opportunity":
                GetTheoryResponse(evidence, _opportunity);
            break;
            default:
            break;
        }

        return null;
    }

    public string GetTheoryResponse(CaseEvidence evidence, SuspectTheory type)
    {
        
        if(type.ValidTheories.Any(x => x.evidence == evidence))
        {
            var validMatch = Array.Find(type.ValidTheories, e => e.evidence == evidence); 
            return validMatch.response;
        }
        else if(type.InvalidTheories.Any(x => x.evidence == evidence))
        {
            var invalidMatch = Array.Find(type.InvalidTheories, e => e.evidence == evidence); 
            return invalidMatch.response;
        }
        else return "I don't know what to say about this theory, Detective. It doesn't seem to fit at all.";
    }
}
