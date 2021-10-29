using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class gameEvents 
{
    private static Dictionary<string, Action<string>> eventsDictionary = new Dictionary<string, Action<string>>();

    static gameEvents()
    {
        eventsDictionary.Add("local", Interactable.TriggerLocalEvent);
        
    }

    public static void ProcessGameEvent(string eventName, string eventValue)
    {
        if(eventsDictionary.ContainsKey(eventName))
        {
            eventsDictionary[eventName](eventValue);
        }
    }
}
