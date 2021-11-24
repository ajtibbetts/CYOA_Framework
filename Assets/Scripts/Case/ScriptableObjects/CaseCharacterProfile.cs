using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CaseDataObjects;

[CreateAssetMenu(fileName = "CaseCharacterProfile", menuName = "CYOA/Case/CharacterProfile", order = 0)]
public class CaseCharacterProfile : ScriptableObject {
    [Header("Character Info")]
    [SerializeField] private string _characterID;
    [SerializeField] private CharacterType _characterType;
    [SerializeField] private CaseProperty _characterName;
    [SerializeField] private CaseImage _portrait;
    [SerializeField] private int thumbNailOffsetX;
    [SerializeField] private int thumbNailOffsetY;
    [SerializeField] private CaseProperty _age;
    [SerializeField] private CaseProperty _occupation;
    [SerializeField] private CaseProperty _residence;
    [SerializeField] private CaseProperty _summary;
    [SerializeField] private CaseProperty _relationshipToVictim;
    [SerializeField] private List<string> _additionalNotes;

    [Header("As Suspect Info")]
    public string AsCulpritCompleteResponse;
    public string AsCulpritPartialResponse;
    public string AsInnocentResponse;
    public bool HasAlibi;
    public string AlibiText;

    private string _undiscoveredText = "???";

    public CharacterType GetCharacterType()
    {
        return _characterType;
    }

    public string GetCharacterID()
    {
        return _characterID;
    }

    public string GetCharacterName(bool overrideFlag = false)
    {
        if(overrideFlag) return _characterName.propertyValue;
        else
        {
            return _characterName.startAsDiscovered ? _characterName.propertyValue : _undiscoveredText;
        }
    }

    public CharacterPortrait GetPortrait()
    {
        return _portrait.startAsDiscovered ? _portrait.portrait : null;
    }

    public string GetAge()
    {
        return _age.startAsDiscovered ? _age.propertyValue : _undiscoveredText;
    }

    public string GetOccupation()
    {
        return _occupation.startAsDiscovered ? _occupation.propertyValue : _undiscoveredText;
    }

    public string GetResidence()
    {
        return _residence.startAsDiscovered ? _residence.propertyValue : _undiscoveredText;
    }

    public string GetSummary()
    {
        return _summary.startAsDiscovered ? _summary.propertyValue : _undiscoveredText;
    }

    public string GetRelationShipToVictim()
    {
        return _relationshipToVictim.startAsDiscovered ? _relationshipToVictim.propertyValue : _undiscoveredText;
    }

    public List<string> GetAdditionalNotes()
    {
        return _additionalNotes;
    }

    public CharacterPortrait RevealCharacterPortrait()
    {
        return _portrait.portrait;
    }
    
    public string RevealCharacterProperty(string propertyName)
    {
        string fieldToFetch = "_"+propertyName;
        // Debug.Log("CASE PROFILE ---- ATTEMPTING TO RETURN PROPERTY: " + fieldToFetch);
        var propType = this.GetType();
        // Debug.Log("CASE PROFILE ---- PROP TYPE: " + propType.ToString());
        var propField = propType.GetField(fieldToFetch, BindingFlags.Instance | BindingFlags.NonPublic);
        // Debug.Log("PROP FIELD: " + propField.ToString());
        CaseProperty propValue = (CaseProperty)propField.GetValue(this);
        Debug.Log("CASE CHARACTER PROFILE ---- VALUE: " + propValue.propertyValue);
        return propValue.propertyValue;
    }

    public CharacterProfileData GetFullData()
    {
        CharacterProfileData profileData = new CharacterProfileData
        {
            characterID = _characterID,
            characterName = _characterName.propertyValue,
            portrait = _portrait.portrait,
            age = _age.propertyValue,
            occupation = _occupation.propertyValue,
            residence = _residence.propertyValue,
            summary = _summary.propertyValue,
            relationshipToVictim = _relationshipToVictim.propertyValue,
            additionalNotes = _additionalNotes
        };

        return profileData;
    }
}
