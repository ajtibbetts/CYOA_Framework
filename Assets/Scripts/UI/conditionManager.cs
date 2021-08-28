using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class conditionManager : MonoBehaviour
{
    [HideInInspector] public UIManager UIManager;

    private Dictionary<string,string> propertyLabels = new Dictionary<string,string>();
    private string currentPropertyValue;
    private string requiredPropertyValue;

    void Awake() {
        UIManager = GetComponent<UIManager>();
        initDictionary();
    }

    void initDictionary(){
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

    public string getConditionLabel(string propertyName) {
        if(propertyLabels.ContainsKey(propertyName) == true) {
            return $"{propertyLabels[propertyName].ToUpper()} {currentPropertyValue}/{requiredPropertyValue}";
        }
        else {
            return null;
        }
    }

    public Boolean areConditionsMet(actionOption actionOption) {
        Debug.Log("checking action option from conditionManager");
        Debug.Log($"condition length: {actionOption.conditionalRequirement.conditionsToMeet.Length}");
        for(int i = 0; i < actionOption.conditionalRequirement.conditionsToMeet.Length; i ++) {
            // get appropriate condition type
            Debug.Log($"condition type: {actionOption.conditionalRequirement.conditionsToMeet[i].condition}");
            switch (actionOption.conditionalRequirement.conditionsToMeet[i].condition)
            {
                case conditionType.playerProperty:
                     Debug.Log("Checking player property");
                     if(!checkPlayerCondition(actionOption.conditionalRequirement.conditionsToMeet[i])){
                         Debug.Log($"Player property conditional failed at option {i}.");
                         return false;
                     }
                break;
                default:
                
                break;
            }
            
        }
        // if all conditions are satisfied, return true
        return true;
    }

    private Boolean checkPlayerCondition(conditionalStatement condition) {
        // get player property and value;
        PropertyInfo propertyInfo = UIManager.controller.player.stats.GetType().GetProperty(condition.propertyName);
        int currentValue = (int)propertyInfo.GetValue(UIManager.controller.player.stats, null);
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
