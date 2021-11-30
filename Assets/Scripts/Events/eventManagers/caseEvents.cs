using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaseDataObjects;

public static class caseEvents
{
    private static Dictionary<string, Action<string>> eventsDictionary = new Dictionary<string, Action<string>>();

    // events
    public static event Action<string> onVictimUpdated;
    public static event Action<string> onProfileAdded;
    public static event Action<string, string> onProfileUpdated;
    public static event Action<string> onEvidenceAdded;
    public static event Action<string, string> onEvidenceChanged;
    public static event Action<string> onEvidenceRemoved;
    public static event Action<string> onLeadAdded;
    public static event Action<string> onLeadResolved;

    public static event Action<string> onLocationDiscovered;
    public static event Action<string, LocationStatus> onLocationStatusUpdated;

    static caseEvents()
    {
        eventsDictionary.Add("startCase", StartCase);
        eventsDictionary.Add("updateVictim", UpdateVictim);
        eventsDictionary.Add("addProfile", AddProfile);
        eventsDictionary.Add("updateProfile", UpdateProfile);
        eventsDictionary.Add("addEvidence", AddEvidence);
        eventsDictionary.Add("removeEvidence", RemoveEvidence);
        eventsDictionary.Add("upgradeEvidence", UpgradeEvidence);
        eventsDictionary.Add("addLead", AddLead);
        eventsDictionary.Add("resolveLead", ResolveLead);
        eventsDictionary.Add("addLocation", AddNewLocation);
        eventsDictionary.Add("updateLocation", UpdateLocationStatus);
        eventsDictionary.Add("warrant", WarrantEvent);
    }

    public static void ProcessEvent(string eventName, string eventValue)
    {
        if(eventsDictionary.ContainsKey(eventName))
        {
            eventsDictionary[eventName](eventValue);
        }
        else
        {
            Debug.LogError("CASE EVENTS ---- Attempted to process event for key but missing key: " + eventName);
        }
    }

    // EVENTS

    private static void StartCase(string caseID)
    {
        CaseManager.Instance.StartNewCase(caseID);
    }

    private static void UpdateVictim(string characterProperty)
    {
        switch(characterProperty)
        {
            case "portrait":
                onVictimUpdated?.Invoke("portrait");
            break;
            case "name":
                onVictimUpdated?.Invoke("name");
            break;
            case "age":
                onVictimUpdated?.Invoke("age");
            break;
            case "occupation":
                onVictimUpdated?.Invoke("occupation");
            break;
            case "residence":
                onVictimUpdated?.Invoke("residence");
            break;
            case "summary":
                onVictimUpdated?.Invoke("summary");
            break;
            case "cod":
                onVictimUpdated?.Invoke("causeOfDeath");
            break;
            case "tod":
                onVictimUpdated?.Invoke("timeOfDeath");
            break;
            case "lod":
                onVictimUpdated?.Invoke("locationOfDeath");
            break;
            default:
                Debug.LogError("CASE EVENTS ---- FAILED TO UPDATE CHARACTER PROFILE, INCORRECT PARAM VALUE: " + characterProperty);
            break;
        }
    }

    private static void AddProfile(string profileName)
    {
        onProfileAdded?.Invoke(profileName);
    }

    private static void UpdateProfile(string eventValue)
    {
        var eventParams = eventValue.Split('.');
        if(eventParams.Length == 2)
        {
            var characterID = eventParams[0];
            var characterProperty = eventParams[1];
            switch(characterProperty)
            {
                case "portrait":
                    onProfileUpdated?.Invoke(characterID, "portrait");
                break;
                case "name":
                    onProfileUpdated?.Invoke(characterID, "characterName");
                break;
                case "age":
                    onProfileUpdated?.Invoke(characterID, characterProperty);
                break;
                case "occupation":
                    onProfileUpdated?.Invoke(characterID, characterProperty);
                break;
                case "residence":
                    onProfileUpdated?.Invoke(characterID, characterProperty);
                break;
                case "summary":
                    onProfileUpdated?.Invoke(characterID, characterProperty);
                break;
                case "relationship":
                    onProfileUpdated?.Invoke(characterID, "relationshipToVictim");
                break;
                default:
                    Debug.LogError("CASE EVENTS ---- FAILED TO UPDATE CHARACTER PROFILE, INCORRECT PARAM VALUE: " + characterProperty);
                break;
            }
        }
        else Debug.LogError("CASE EVENTS ---- FAILED TO UPDATE CHARACTER PROFILE, INCORRECT PARAMS LENGTH: " + eventParams.Length);
    }

    private static void AddEvidence(string evidenceID)
    {
        onEvidenceAdded?.Invoke(evidenceID);
    }

    private static void UpgradeEvidence(string eventValue)
    {
        var eventParams = eventValue.Split('.');
        if(eventParams.Length == 2)
        {
            var oldEvidenceID = eventParams[0];
            var newEvidenceID = eventParams[1];
            onEvidenceChanged?.Invoke(oldEvidenceID,newEvidenceID);
            
        }
        else Debug.LogError("CASE EVENTS ---- FAILED TO UPGRADE EVIDENCE, INCORRECT PARAMS LENGTH: " + eventParams.Length);
    }

    private static void RemoveEvidence(string evidenceID)
    {
        onEvidenceRemoved?.Invoke(evidenceID);
    }

    private static void AddLead(string leadID)
    {
        onLeadAdded?.Invoke(leadID);
    }

    private static void ResolveLead(string leadID)
    {
        onLeadResolved?.Invoke(leadID);
    }

    private static void AddNewLocation(string areaName)
    {
        onLocationDiscovered?.Invoke(areaName);
    }

    private static void UpdateLocationStatus(string eventValue)
    {
        var eventParams = eventValue.Split('.');
        if(eventParams.Length == 2)
        {
            var areaName = eventParams[0];
            switch(eventParams[1])
            {
                case "unlock":
                    onLocationStatusUpdated?.Invoke(areaName, LocationStatus.AVAILABLE);
                break;
                case "lock":
                    onLocationStatusUpdated?.Invoke(areaName, LocationStatus.LOCKED);
                break;
            }
        }
        else Debug.LogError("CASE EVENTS ---- FAILED TO UPDATE LOCATION STATUS, INCORRECT PARAMS LENGTH: " + eventParams.Length);
    }


    private static void WarrantEvent(string eventValue)
    {
        switch(eventValue)
        {
            case "setResults":
                CaseManager.Instance.SetWarrantResults(PlayerCaseRecord.Instance.GetPrimarySuspect());
            break;
            case "grantWarrant":
                PlayerCaseRecord.Instance.GrantWarrant();
            break;
        }

    }
}
