using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class gameEvents 
{
    private static Dictionary<string, Action<string>> eventsDictionary = new Dictionary<string, Action<string>>();


    // events
    public static event Action<string, string> OnDialogueSkipCalled;

    static gameEvents()
    {
        eventsDictionary.Add("local", Interactable.TriggerLocalEvent);
        eventsDictionary.Add("skiptodialogue", SkipToNewDialogueContainer);
        
    }

    public static void ProcessGameEvent(string eventName, string eventValue)
    {
        if(eventsDictionary.ContainsKey(eventName))
        {
            eventsDictionary[eventName](eventValue);
        }
    }

    private static void SkipToNewDialogueContainer(string eventValue)
    {
        // send event in format of dialogueContainerName.optionalNodeGUID
        string dialogueName = eventValue;
        string targetNodeGUID = null;
        if(eventValue.Contains("."))
        {
            var eventParams = eventValue.Split('.');
            dialogueName = eventParams[0];
            targetNodeGUID = eventParams[1];
        }

        OnDialogueSkipCalled?.Invoke(dialogueName, targetNodeGUID);
    }
}
