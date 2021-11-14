using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class caseEvents
{
    private static Dictionary<string, Action<string>> eventsDictionary = new Dictionary<string, Action<string>>();

    public static event Action<string> onProfileAdded;
    public static event Action<string> onEvidenceAdded;

    static caseEvents()
    {
        eventsDictionary.Add("addProfile", AddProfile);
        eventsDictionary.Add("addEvidence", AddEvidence);

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

    private static void AddProfile(string profileName)
    {
        onProfileAdded?.Invoke(profileName);
    }

    private static void AddEvidence(string evidenceID)
    {
        onEvidenceAdded?.Invoke(evidenceID);
    }

}
