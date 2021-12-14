using System;
using UnityEngine;
using globalDataTypes;

public abstract class Interactable : NavObject
{
    public static event Action<string> onLocalEventTriggered;

    
    [Header("Navigation")]
    // public string Name;
    // public string GUID = Guid.NewGuid().ToString();

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

    public override void ActivateNavObject()
    {
        base.ActivateNavObject();
        ActivateInteractable();
    }

    public virtual void ActivateInteractable()
    {
        onLocalEventTriggered += ProcessLocalEvent;
        DialogueParser.onDialogueReachedDeadEnd += DeactivateInteractable;
    }

    public virtual void DeactivateInteractable()
    {
        onLocalEventTriggered -= ProcessLocalEvent;
        DialogueParser.onDialogueReachedDeadEnd -= DeactivateInteractable;
        DeactivateNavObject();
        // INIT DIALOGUE HERE
    }

    

    // public override void AddNavObjectToPlayer()
    // {
    //     base.AddNavObjectToPlayer();
    //     // add active object to player's data if not already in there
    //     if(!Player.Instance.visitedInteractableObjects.Contains(GUID))
    //     {
    //         Player.Instance.visitedInteractableObjects.Add(GUID);
    //     }
    // }

    public abstract void ProcessLocalEvent(string eventName);

    // public override bool HasPlayerVisitedNavObject()
    // {
    //     return Player.Instance.visitedInteractableObjects.Contains(GUID);
    // }

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
