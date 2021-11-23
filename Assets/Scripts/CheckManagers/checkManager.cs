using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using globalDataTypes;
using CaseDataObjects;


public static class checkManager
{
    

    public static bool GetCheckResult(conditionType checkType, string checkName, string checkValue, string comparisonOperator)
    {
        switch(checkType)
        {
            case conditionType.playerProperty:
                return ComparePlayerProperty(checkName, checkValue, comparisonOperator);
            case conditionType.playerSkill:
                return ComparePlayerSkill(checkName, checkValue, comparisonOperator);
            case conditionType.itemProperty:

            break;
            case conditionType.npcProperty:

            break;
            case conditionType.enemyProperty:

            break;
            case conditionType.caseProperty:

            break;
            case conditionType.storyProperty:
                return CompareStoryProperty(checkName, checkValue, comparisonOperator);
            case conditionType.worldProperty:
                return PlayerProgressTracker.Instance.HasWorldFlag(checkName);
            case conditionType.gameProperty:

            break;
            default:
                Debug.LogError("CHECK MANAGER ---- FAILED TO GET CHECK RESULT.");
            break;
        }


        return false;
    }
    
    private static bool ComparePlayerProperty(string checkName, string checkValue, string comparisonOperator)
    {
        var playerPropertyValue = Player.Instance.GetFieldValue(checkName);
        return CompareValues(playerPropertyValue, Int32.Parse(checkValue), comparisonOperator);
    }

    private static bool ComparePlayerSkill(string checkName, string checkValue, string comparisonOperator)
    {
        var playerSkillValue = Player.Instance.GetSkillValue(checkName);
        return CompareValues(playerSkillValue, Int32.Parse(checkValue), comparisonOperator);
    }

    private static bool CompareStoryProperty(string checkName, string checkValue, string comparisonOperator)
    {
        if(checkName == "local")
        {
            // local properties are split as name.value ( . delimiter)
            var checkParams = checkValue.Split('.');
            if(checkParams.Length == 2)
            {
                var localNavObjectProperty = WorldNavigator.Instance.GetActiveNavObjectProperty(checkParams[0]);
                var localCheckValue = checkParams[1];
                return CompareStrings(localNavObjectProperty, localCheckValue, comparisonOperator);
            }
            else 
            {
                Debug.LogError("CHECK MANAGER ---- Failed to split checkValue in proper name.value format");
                return false;
            }
        }
        else 
        {
            return PlayerProgressTracker.Instance.HasStoryFlag(checkName);
        }
    }




    private static bool CompareValues(int val1, int val2, string comparisonOperator)
    {
        switch(comparisonOperator)
        {
            case "==":
                return val1 == val2;
            case "!=":
                return val1 != val2;
            case ">=":
                return val1 >= val2;
            case "<=":
                return val1 <= val2;
            case ">":
                return val1 > val2;
            case "<": 
                return val1 < val2;
            default:
            break;
        }
        Debug.LogError($"CHECK MANAGER - Failed to compare val1 {val1} and val2 {val2} with operator {comparisonOperator}.");
        return false;
    }

    private static bool CompareStrings(string val1, string val2, string comparisonOperator)
    {
        Debug.Log($"Check Manager ---- Comparing strings val1: {val1} to val2: {val2} using operator: {comparisonOperator}");
        switch(comparisonOperator)
        {
            case "==":
                return val1 == val2;
            case "!=":
                return val1 != val2;
            default:
            break;
        }
        Debug.LogError($"CHECK MANAGER - Failed to compare STRINGS val1 {val1} and val2 {val2} with operator {comparisonOperator}.");
        return false;
    }
}
