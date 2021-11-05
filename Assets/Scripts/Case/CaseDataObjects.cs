using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CaseDataObjects 
{
    /***
    /
    /
    / STRUCTS
    /
    /
    ***/
    
    [Serializable]
    public struct CaseProperty
    {
        public string propertyValue;
        
        [Tooltip("If true, this value will be known to player at start of new case.")]
        public bool startAsDiscovered;
    }

    [Serializable]
    public struct CaseImage
    {
        public Sprite portraitSprite;
        
        [Tooltip("If true, this value will be known to player at start of new case.")]
        public bool startAsDiscovered;
    }

    [Serializable]
    public struct CaseTheory
    {
        [Tooltip("Evidence to present for this theory.")]
        public CaseEvidence evidence;
        
        [Tooltip("Resposne the courts will provide as to validity of evidencial theory.")]
        public string response;
    }

    [Serializable]
    public struct SuspectTheory
    {
        public List<CaseTheory> ValidTheories;
        public List<CaseTheory> InvalidTheories;
    }

    [Serializable]
    public struct ActiveLead
    {
        public CaseLead lead;
        public bool isResolved;
    }

    [Serializable]
    public struct VictimData
    {
        [Header("Victim Info")]
        public string VictimName;
        public Sprite VictimPortrait;
        public string VictimSummary;
        public string VictimAge;
        public string VictimResidence;
        public string VictimOccupation;
        [Header("Homicide Info")]
        public string CauseOfDeath;
        public string TimeofDeath;
        public string LocationOfDeath;
        public List<String> AdditionalInjuries;
        public List<String> AdditionalNotes;
    }

    [Serializable]
    public struct CharacterProfileData
    {
        [Header("Profile Info")]
        public string _characterName;
        public Sprite _portrait;
        public string _age;
        public string _occupation;
        public string _residence;
        public string _summary;
        public string _relationshipToVictim;
        public List<string> _additionalNotes;

        [Header("As Suspect Info")]
        public string AsCulpritCompleteResponse;
        public string AsCulpritPartialResponse;
        public string AsInnocentResponse;
        public bool HasAlibi;
        public string AlibiText;
    }
    /***
    /
    /
    / CLASSES
    /
    /
    ***/

    [Serializable]
    public class CaseSuspect
    {
        public CharacterProfileData SuspectProfile;
        public CaseEvidence ProposedMeans;
        public CaseEvidence ProposedMotive;
        public CaseEvidence ProposedOpportunity;

        public CaseSuspect(CharacterProfileData profile)
        {
            SuspectProfile = profile;
        }
    }

    public class TheoryResults
    {
        public bool MatchedSuspect;
        public bool MatchedMeans;
        public bool MatchedMotive;
        public bool MatchedOpportunity;

        public bool IsTheoryValid()
        {
            return MatchedSuspect && MatchedMeans && MatchedMotive && MatchedOpportunity;
        }
    }
}
