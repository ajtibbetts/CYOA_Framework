using System;   
using UnityEngine;

namespace globalDataTypes {

    // CLASSES
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
                    case conditionType.caseProperty:
                        checkText = "Case Flag";
                    break;
                    default:
                    break;
                }
            return checkText;
        }
    }

    [Serializable]
    public class localProperty {
        public string PropertyName;
        public string PropertyValue;
    }

    // ENUMS
    [Serializable]
    public enum GAMESTATE 
    {
        WORLDNAVIGATION,
        DIALOGUE,
        INVESTIGATING
    }

    [Serializable]
    public enum UISTATE
    {
        MAINMENU,
        CASEMENU,
        DRAGGING,
        NORMALGAMEPLAY
    }

    [Serializable]
    public enum MENUTYPE
    {
        MAINMENU,
        CASEMENU
    }

    [Serializable]
    public enum conditionType {
        playerProperty,
        playerSkill,
        itemProperty,
        npcProperty,
        enemyProperty,
        caseProperty,
        storyProperty,
        worldProperty,
        gameProperty,
        none
    }

    public enum eventType {
        none,
        player,
        skill,
        item,
        npc,
        enemy,
        caseRecord,
        story,
        world,
        game
        
    }

    [Serializable]
    public enum nodeType {
        dialogueNode,
        eventNode,
        checkNode,
        endpointNode,
        rollNode
        
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
            public eventType eventType;
            public string eventName;
            public string eventValue;
    }

    [Serializable]
    public struct conditionalEvent {
        public conditionalStatement[] conditions;
        public actionEvent eventToTrigger;
    }


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

}