using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CYOA_EventManager : MonoBehaviour
{
    private string _eventMessage;

    [HideInInspector] public gameController controller;

    // Events
    public event Action<string> onEventFinished;
    public event Action<string, string> onPlayerEvent;
    public event Action<string, string> onSkillEvent;
    public event Action<string, string> onItemEvent;
    public event Action<string, string> onNPCEvent;
    public event Action<string, string> onEnemyEvent;
    public event Action<string, string> onQuestEvent;
    public event Action<string, string> onStoryEvent;
    public event Action<string, string> onWorldEvent;
    // public event Action<string, string> onGameEvent;
    
    private void Awake() {
            controller = GetComponent<gameController>(); 
            controller.DialogueParser.onEventTriggered += ProcessDialogueEvent; 
    }

    public void Start() {
        

    }


    public void ProcessDialogueEvent(eventType eventType, string eventName, string eventValue)
    {
        Debug.Log("EVENT MANAGER FIRING EVENT: " + eventName + " VALUE: " + eventValue + " TYPE: " + eventType.ToString());
        // test message addition

        // process event based on type
        switch(eventType) 
        {
            case eventType.player:
                onPlayerEvent?.Invoke(eventName, eventValue);
                // update this to increase/decrease and send event type to ignore certain types
                _eventMessage = $"{eventName} increased by {eventValue}.\n";
                onEventFinished?.Invoke(_eventMessage);
            break;
            case eventType.skill:
                onSkillEvent?.Invoke(eventName, eventValue);
            break;
            case eventType.item:
                onItemEvent?.Invoke(eventName, eventValue);
            break;
            case eventType.npc:
                onNPCEvent?.Invoke(eventName, eventValue);
            break;
            case eventType.enemy:
                onEnemyEvent?.Invoke(eventName, eventValue);
            break;
            case eventType.quest:
                onQuestEvent?.Invoke(eventName, eventValue);
                caseEvents.ProcessEvent(eventName, eventValue);
            break;
            case eventType.story:
                onStoryEvent?.Invoke(eventName, eventValue);
            break;
            case eventType.world:
                onWorldEvent?.Invoke(eventName, eventValue);
            break;
            case eventType.game:
                // onGameEvent?.Invoke(eventName, eventValue);
                gameEvents.ProcessGameEvent(eventName, eventValue);
            break;
            default:
                return;
        }

        
    }
}
