using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using globalDataTypes;

public static class conditionManager 
{
    private static Dictionary<string,string> propertyLabels = new Dictionary<string,string>();
    private static string currentPropertyValue;
    private static string requiredPropertyValue;

    static conditionManager()
    {
        initDictionary();
    }

    static void initDictionary(){
        // player vars
        propertyLabels.Add("playerName", "Name");
        propertyLabels.Add("playerClass", "Class");
        propertyLabels.Add("experienceLevel", "Level");
        //myDict.Add("experienceTotal", "Class");

        propertyLabels.Add("currentHealth", "Health");
        propertyLabels.Add("maxHealth", "Max Health");
        propertyLabels.Add("currentEnergy", "Energy");
        propertyLabels.Add("maxEnergy", "Max Energy");
        propertyLabels.Add("currentGold", "Gold");

        propertyLabels.Add("braveryLevel", "Bravery");
        propertyLabels.Add("alacrityLevel", "Alacrity");
        propertyLabels.Add("tenacityLevel", "Tenacity");
        propertyLabels.Add("brillianceLevel", "Brilliance");
        propertyLabels.Add("intuitionLevel", "Intuition");
        propertyLabels.Add("confidenceLevel", "Confidence");

    }

    public static string getConditionLabel(string propertyName) {
        if(propertyLabels.ContainsKey(propertyName) == true) {
            return $"{propertyLabels[propertyName].ToUpper()} {currentPropertyValue}/{requiredPropertyValue}";
        }
        else {
            return null;
        }
    }

    public static bool isConditionMet(conditionalStatement conditionData)
    {
        switch(conditionData.condition)
        {
            case conditionType.none:
                return true;
            // add other types later
            default:
                return true;
        }
    }

    public static string GetConditionalText(conditionalText[] conditionalTexts)
    {   
        // return first matched text that meets all conditions in its set
        foreach(conditionalText conditionText in conditionalTexts)
        {
            // if there are any conditions, check to ensure all are met
            if(conditionText.conditions.Length > 0)
            {
                bool allConditionsMet = true;

                // ensure each condition is met, flag if any single is unmet
                for(int i = 0; i < conditionText.conditions.Length; i++)
                {
                    
                    if(!isConditionMet(conditionText.conditions[i]))
                    {
                        allConditionsMet = false;
                    }
                }
                // return only if all conditions are met
                if(allConditionsMet) return conditionText.displayText;
            }
            else
            {
                return conditionText.displayText;
            }
        }

        // return null if nothing is met
        return null;
    }

    public static DialogueContainer GetConditionalDialogue(conditionalDialogue[] conditionalDialogues)
    {
        // return first matched dialogue that meets all conditions in its set
        foreach(conditionalDialogue conditionDialogue in conditionalDialogues)
        {
            // if there are any conditions, check to ensure all are met
            if(conditionDialogue.conditions.Length > 0)
            {
                bool allConditionsMet = true;

                // ensure each condition is met, flag if any single is unmet
                for(int i = 0; i < conditionDialogue.conditions.Length; i++)
                {
                    
                    if(!isConditionMet(conditionDialogue.conditions[i]))
                    {
                        allConditionsMet = false;
                    }
                }
                // return only if all conditions are met
                if(allConditionsMet) return conditionDialogue.dialogue;
            }
            else
            {
                return conditionDialogue.dialogue;
            }
        }

        // return null if nothing is met
        return null;
    }


    // old method logic, but keep for now to put together new condition checkers
    private static Boolean checkPlayerCondition(conditionalStatement condition, UIManager UIManager) {
        // get player property and value;
        PropertyInfo propertyInfo = Player.Instance.GetType().GetProperty(condition.propertyName);
        int currentValue = (int)propertyInfo.GetValue(Player.Instance, null);
        int requiredValue = condition.valueInt;
        currentPropertyValue = currentValue.ToString();
        Debug.Log("Current property value:" + currentPropertyValue);
        requiredPropertyValue = requiredValue.ToString();
        Debug.Log("required property value:" + requiredPropertyValue);
        Debug.Log("Property value of from condition check " + condition.propertyName+ ": " + currentValue);
        switch(condition.evaluation) {
            case conditionEval.isEqualTo:
                return currentValue == requiredValue;
            case conditionEval.isNotEqualTo:
                return currentValue != requiredValue;
            case conditionEval.isLessThan:
                return currentValue < requiredValue;
            case conditionEval.isLessThanOrEqualTo:
                return currentValue <= requiredValue;
            case conditionEval.isGreaterThan:
                return currentValue > requiredValue;
            case conditionEval.isGreaterThanOrEqualTo:
                return currentValue >= requiredValue;
            
        }

        return false;
    }
}
