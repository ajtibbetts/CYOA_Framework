using System;   
using UnityEngine;

// public class globalDataTypes : MonoBehaviour
// {
//     // Start is called before the first frame update
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }
// }

// ENUMS
[Serializable]
public enum conditionType {
    playerProperty,
    npcProperty,
    enemyProperty,
    storyProperty,
    itemProperty

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
[Serializable]
public struct actionOptionCondition {
    [Header("Condition")]
    public Boolean conditionMet;
    public Boolean hideUnlessMet;
    public conditionalStatement[] conditionsToMeet;
    [Header("Outcome")]
    public Room passRoom;
    public string passButtonText;
    public string passActionText;
    public string failButtonText;

}

[Serializable]
public struct conditionalStatement {
    public conditionType condition;
    public string propertyName;
    public conditionEval evaluation;
    public string valueString;
    public int valueInt;
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
