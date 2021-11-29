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
    / ENUMS
    /
    /
    ***/

    public enum CharacterType 
    {
        ALLY,
        NEUTRAL,
        SUSPECT
    }

    public enum EvidenceType
    {
        MEANS,
        MOTIVE,
        OPPORTUNITY,
        UNASSIGNED
    }

    public enum LocationStatus
    {
        AVAILABLE,
        LOCKED,
        UNDISCOVERED
    }
    
    
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
        public CharacterPortrait portrait;
        
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
    public class SuspectTheory
    {
        public List<CaseTheory> ValidTheories;
        public List<CaseTheory> InvalidTheories;
    }

    

    
    /***
    /
    /
    / CLASSES
    /
    /
    ***/

    [Serializable]
    public class caseRecordEntry
    {
        public string caseID;
        public VictimData victimData;
        public CaseSuspect primarySuspect; 
    }

    [Serializable]
    public class ActiveLocation
    {
        public String MapLocationAreaName;
        public LocationStatus LocationStatus;

        public ActiveLocation(string areaname, LocationStatus status)
        {
            MapLocationAreaName = areaname;
            LocationStatus = status;
        }
    }

    [Serializable]
    public class ActiveLead
    {
        public CaseLead lead;
        public bool isResolved;
    }

    [Serializable]
    public class CaseSuspect
    {
        public CharacterProfileData SuspectProfile;
        public CaseEvidence ProposedMeans;
        public CaseEvidence ProposedMotive;
        public CaseEvidence ProposedOpportunity;

        // public CaseSuspect(CharacterProfileData profile)
        // {
        //     SuspectProfile = profile;
        // }
    }

    public class TheoryResults
    {
        public bool MatchedSuspect = false;
        public string ResponseSuspect;
        public bool MatchedMeans = false;
        public string ResponseMeans;
        public bool MatchedMotive = false;
        public string ResponseMotive;
        public bool MatchedOpportunity = false;
        public string ResponseOpportunity;

        public bool IsTheoryValid()
        {
            return MatchedSuspect && MatchedMeans && MatchedMotive && MatchedOpportunity;
        }

        public void SetTheoryResult(EvidenceType type, bool result, string response)
        {
            switch(type)
            {
                case EvidenceType.MEANS:
                    MatchedMeans = result;
                    ResponseMeans = response;
                    return;
                case EvidenceType.MOTIVE:
                    MatchedMotive = result;
                    ResponseMotive = response;
                    return;
                case EvidenceType.OPPORTUNITY:
                    MatchedOpportunity = result;
                    ResponseOpportunity = response;
                    return;
                default:
                    MatchedSuspect = result;
                    ResponseSuspect = response;
                    return;
            }
        }
    }

    [Serializable]
    public class CharacterPortrait
    {
        public Sprite portraitSprite;
        public float thumbNailOffsetX; // used in square thumbnail offsets
        public float thumbNailOffsetY; // used in square thumbnail offsets
    }

    [Serializable]
    public class CharacterProfileData
    {
        [Header("Profile Info")]
        public string characterID;
        public string characterName;
        public CharacterPortrait portrait;
        public string age;
        public string occupation;
        public string residence;
        public string summary;
        public string relationshipToVictim;
        public List<string> additionalNotes;

        public CharacterType characterType;

        [Header("As Suspect Info")]
        public string AsCulpritCompleteResponse;
        public string AsCulpritPartialResponse;
        public string AsInnocentResponse;
        public bool HasAlibi;
        public string AlibiText;
    }
    
    [Serializable]
    public class VictimData
    {
        [Header("Victim Info")]
        public string name;
        public CharacterPortrait portrait;
        public string summary;
        public string age;
        public string residence;
        public string occupation;
        [Header("Homicide Info")]
        public string causeOfDeath;
        public string timeOfDeath;
        public string locationOfDeath;
        public List<String> AdditionalInjuries;
        public List<String> AdditionalNotes;
    }

}
