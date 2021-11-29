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

    // Case Record
    private static bool CompareCaseProperty(string checkName, string checkValue, string comparisonOperator)
    {
        checkName = checkName.ToLower();
        checkValue = checkValue.ToLower();
        // first check for 'case', as case will not use propname.checkValue format
        if(checkName == "case")
            return CompareCaseRecordProperty(checkValue);
        
        // just check if player has evidence in record by evidence ID
        if(checkName == "evidence") 
            return PlayerCaseRecord.Instance.isEvidenceInRecord(checkValue);

        if(checkName == "victim")
            return CompareVictimProperty(checkValue);
        
        if(checkName == "suspect")
        {
            var suspectProfile = PlayerCaseRecord.Instance.GetPrimarySuspect().SuspectProfile;
            return CompareProfileProperty(suspectProfile, checkValue);
        }

        if (checkName == "warrant")
        {

        }

        
        // for all others get the check params from checkValue format of propname.checkvalue
        var checkParams = checkValue.Split('.');
        if(checkParams.Length == 2)
        {
            var propName = checkParams[0];
            var propCheckValue = checkParams[1];
            switch(checkName)
            {      
                case "profile":
                    CharacterProfileData charProfile;
                    if(propName == "local")
                    {
                        // get local profile name from active npc object dialogue is attached to
                        string charID = WorldNavigator.Instance.GetActiveNPCData().characterID;
                        charProfile = PlayerCaseRecord.Instance.GetProfileByID(charID);
                    }
                    else
                    {
                        // otherwise grab from case record by name
                        charProfile = PlayerCaseRecord.Instance.GetProfileByID(propName);
                    }
                    
                    if(charProfile != null) 
                        return CompareProfileProperty(charProfile, propCheckValue);
                    else Debug.LogError("CHECK MANAGER ---- charProfile is NULL");
                    return false;
                case "lead":
                    return CompareLeadProperty(propName, propCheckValue);
                case "location":
                break;
            }
        }
        else 
        {
            Debug.LogError("CHECK MANAGER ---- Failed to split checkValue in proper name.value format");
            return false;
        }
        return false;
    }

    private static bool CompareCaseRecordProperty(string checkValue)
    {
        // either checking if player is on active case, or checking if case ID is completed in history
        if(checkValue == "active")
            return PlayerCaseRecord.Instance.onActiveCase;
        else 
        {
            var caseEntry = PlayerProgressTracker.Instance.GetCaseRecordFromHistory(checkValue);
            if(caseEntry != null) return true;
            else return false;
        }
    }


    private static bool CompareLeadProperty(string leadID, string leadParam)
    {
        if(leadParam == "has")
            return PlayerCaseRecord.Instance.isLeadInRecord(leadID);
        else if (leadParam == "resolved")
            return PlayerCaseRecord.Instance.isLeadResolved(leadID);
        else 
        {
            Debug.LogError("CHECK MANAGER ---- Lead param not found: " + leadParam);
            return false;
        }
    }

    private static bool CompareVictimProperty(string characterProperty)
    {
        // returns true if a given victim property is 'known'.
        var victimData = PlayerCaseRecord.Instance.GetVictim();
        var unknown = "???";
        switch(characterProperty)
        {
            case "portrait":
                return victimData.portrait != null;
            case "name":
                return victimData.name != unknown;
            case "age":
                return victimData.age != unknown;
            case "occupation":
                return victimData.occupation != unknown;
            case "residence":
                return victimData.residence != unknown;
            case "summary":
                return victimData.summary != unknown;
            case "cod":
                return victimData.causeOfDeath != unknown;
            case "tod":
                return victimData.timeOfDeath != unknown;
            case "lod":
                return victimData.locationOfDeath != unknown;
            default:
                Debug.LogError("CHECK MANAGER ---- Unknown victim property: " + characterProperty);
                return false;
        }
    }

    private static bool CompareProfileProperty(CharacterProfileData profileData, string characterProperty)
    {
        var unknown = "???";
        switch(characterProperty)
        {
            case "suspect":
                return PlayerCaseRecord.Instance.isCharacterASuspect(profileData.characterID);
            case "portrait":
                return profileData.portrait != null;
            case "name":
                return profileData.characterName != unknown;
            case "age":
                return profileData.age != unknown;
            case "occupation":
                return profileData.occupation != unknown;
            case "residence":
                return profileData.residence != unknown;
            case "summary":
                return profileData.summary != unknown;
            case "relationship":
                return profileData.relationshipToVictim != unknown;
            default:
                Debug.LogError("CHECK MANAGER ---- Unknown profile property: " + characterProperty);
                return false;
        }
    }

    private static bool CompareLocationProperty(string areaName, string checkValue)
    {
        if(checkValue == "active")
            return PlayerCaseRecord.Instance.isLocationActive(areaName);
        var locationStatus = PlayerCaseRecord.Instance.GetActiveLocationStatus(areaName);
        switch(checkValue)
        {
            case "available":
                return locationStatus == LocationStatus.AVAILABLE;
            case "locked":
                return locationStatus == LocationStatus.LOCKED;
            case "UNDISCOVERED":
                return locationStatus == LocationStatus.UNDISCOVERED;
        }

        Debug.LogError("CHECK MANAGER --- Unknown location check value: " + checkValue);
        return false;
    }

    private static bool CompareWarrantProperty(string warrantProperty)
    {
        switch(warrantProperty.ToLower())
        {
            case "istheoryvalid":
                return CaseManager.Instance.isWarrantTheoryValid();
        }

        return false;
    }

    /**
    * 
    *  comparison methods
    */

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
