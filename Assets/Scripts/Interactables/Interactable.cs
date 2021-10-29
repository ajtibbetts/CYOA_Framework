using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public static event Action<string> onLocalEventTriggered;

    
    [Header("Navigation")]
    public string Name;
    public string GUID = Guid.NewGuid().ToString();

    [Header("Dialogue Display Text")]
    public DialogueContainer interactiveDialogue;

    [Header("As Option Button")]
    public conditionalText[] buttonTextOnNew;
    public conditionalText[] buttonTextOnReturn;

    public static void TriggerLocalEvent(string eventName)
    {
        Debug.Log("Local event triggered with name: " + eventName);
        onLocalEventTriggered?.Invoke(eventName);
    }

    public virtual void ActivateInteractable(gameController controller)
    {
        // add active object to player's data if not already in there
        if(!controller.player._player.visitedInteractableObjects.Contains(GUID))
        {
            controller.player._player.visitedInteractableObjects.Add(GUID);
        }
        
        onLocalEventTriggered += ProcessLocalEvent;
        DialogueParser.onDialogueReachedDeadEnd += DeactivateInteractable;
        gameObject.name = gameObject.name + "-Active";
        // INIT DIALOGUE HERE
    }

    public virtual void DeactivateInteractable()
    {
        onLocalEventTriggered -= ProcessLocalEvent;
        DialogueParser.onDialogueReachedDeadEnd -= DeactivateInteractable;
        gameObject.name = gameObject.name.Substring(0,gameObject.name.LastIndexOf("-Active"));
        // INIT DIALOGUE HERE
    }

    public abstract void ProcessLocalEvent(string eventName);

    public bool HasPlayerInteracted(gameController controller)
    {
        return controller.player._player.visitedInteractableObjects.Contains(GUID);
    }

    public string GetNewOrReturnedText(bool hasInteracted)
    {
        // used for both buttons and display text
        string displayText = "MISSING";
        if(hasInteracted)
        {
            // Debug.Log($"WORLD NAVIGATOR ---- PLAYER HAS VISITED THIS NAV OBJECT");
            if(buttonTextOnReturn.Length > 0)
            {
                displayText = conditionManager.GetConditionalText(buttonTextOnReturn);
            }
            else
            {
                Debug.Log($"INTERACTABLE ---- THIS INTERACTABLE OBJECT DOES NOT HAVE RETURN TEXT SET: {Name}");
            }

        }
        else 
        {
            // Debug.Log($"WORLD NAVIGATOR ---- PLAYER HAS NOT VISITED THIS NAV OBJECT");
            if(buttonTextOnNew.Length > 0)
            {
                displayText = conditionManager.GetConditionalText(buttonTextOnNew);
            }
            else
            {
                Debug.Log($"INTERACTABLE ---- THIS INTERACTABLE DOES NOT HAVE NEW TEXT SET: {Name}");
            }
        }  

        return displayText;
    }

    
}
