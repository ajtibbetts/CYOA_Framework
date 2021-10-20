using System;   
using UnityEngine;

public static class dataFormatter 
{
    public static string getConditionText(conditionType checkType)
    {
        string checkText = "";
            switch(checkType)
            {
                case conditionType.playerProperty:
                    checkText = "Player Stat";
                break;
                case conditionType.playerSkill:
                    checkText = "Skill";
                break;
                case conditionType.npcProperty:
                    checkText = "NPC Stat";
                break;
                case conditionType.enemyProperty:
                    checkText = "Enemy Stat";
                break;
                case conditionType.storyProperty:
                    checkText = "Story Flag";
                break;
                case conditionType.itemProperty:
                    checkText = "Item Count";
                break;
                case conditionType.worldProperty:
                    checkText = "World Flag";
                break;
                case conditionType.questProperty:
                    checkText = "Quest Flag";
                break;
                default:
                break;
            }
        return checkText;
    }
}

// ENUMS
[Serializable]
public enum conditionType {
    playerProperty,
    playerSkill,
    itemProperty,
    npcProperty,
    enemyProperty,
    questProperty,
    storyProperty,
    worldProperty,
    gameProperty,
    none
}

public enum eventType {
    player,
    skill,
    item,
    npc,
    enemy,
    quest,
    story,
    world,
    game,
    none
}

[Serializable]
public enum nodeType {
    dialogueNode,
    eventNode,
    checkNode,
    endpointNode
}

[Serializable]
public enum conditionEval {
    isEqualTo,
    isNotEqualTo,
    isLessThan,
    isLessThanOrEqualTo,
    isGreaterThan,
    isGreaterThanOrEqualTo
}

// STRUCTS
[Serializable]
public struct actionEvent {
        public CYOA_Event eventToTrigger;
        public eventParams parameters;
}

// [Serializable]
// public struct actionOptionCondition {
//     [Header("Condition")]
//     public Boolean conditionMet;
//     public Boolean hideUnlessMet;
//     public conditionalStatement[] conditionsToMeet;
//     [Header("Outcome")]
//     public Room passRoom;
//     public string passButtonText;
//     public string passActionText;
//     public string failButtonText;

// }

// [Serializable]
// public struct conditionalProperty {
//     [Header("Condition")]
//     public Boolean allowRoll;
//     public Boolean hideUnlessMet;
//     public conditionType conditionalType;
//     public String propertyName;
//     public int requiredValue;
//     [Header("Outcome")]
//     public Room failedPage;
//     public actionEvent[] failedEvents;

// }

// [Serializable]
// public struct conditionalThought {
//     [Header("Condition")]
//     public string requiredSkill;
//     public int requiredSkillLevel;
//     [Header("Outcome")]
//     public string passedText;
//     public string failedText;
//     [Header("Optional Player Actions")]
//     public conditionalPageOption[] actionOptionsToAdd;
//     public actionEvent[] eventsToTrigger;
// }

// [Serializable]
// public struct conditionalPageText {
//     [Header("Page Info")]
//     [Tooltip("Conditional page text that will replace default text.")]
//     public string pageText;
//     [Tooltip("Conditional page image that will replace default image.")]
//     public Sprite pageImage;
//     [Header("Condition")]
//     public conditionalStatement conditionalRequirement;
//     [Header("Optional Player Actions")]
//     public conditionalPageOption[] actionOptionsToAdd;
    
// }

// [Serializable]
// public struct conditionalPageOption {
//     [Tooltip("Options if any that will be added to options list BEFORE global action options.")]
//     public string buttonText;
//     public Room destinationPage;
//     public actionEvent[] eventsToTrigger;
// }

// combined with different structs to make certain conditionals
[Serializable]
public struct conditionalStatement {
    public conditionType condition;
    public string propertyName;
    public conditionEval evaluation;
    public string valueString;
    public int valueInt;
}

[Serializable]
public struct conditionalText {
    public conditionalStatement[] conditions;
    [TextArea]
    public string displayText;
}

[Serializable]
public struct conditionalDialogue {
    public conditionalStatement[] conditions;
    public DialogueContainer dialogue;
}


[Serializable]
public struct statPlayerStat {
    public string statName;
    public int statCurrent;
    public int statMax;
    public int statID;
}

[Serializable]
public struct statAttribute {
    public string attributeName;
    public int attributeLevel;
    public int attributeID;
}

[Serializable]
public struct StatAbility {
    public string abilityName;
    public int abilityLevel;
    public int[] baseAttributeIDs;
}
