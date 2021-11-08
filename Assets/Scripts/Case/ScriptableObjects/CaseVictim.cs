using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using CaseDataObjects;


[CreateAssetMenu(fileName = "CaseVictim", menuName = "CYOA/Case/Victim", order = 0)]
[System.Serializable]
public class CaseVictim : ScriptableObject {
    [Header("Victim Info")]
    [SerializeField] private CaseProperty _name;
    [SerializeField] private CaseImage _portrait;
    [SerializeField] private CaseProperty _summary;
    [SerializeField] private CaseProperty _age;
    [SerializeField] private CaseProperty _residence;
    [SerializeField] private CaseProperty _occupation;

    [Header("Homicide Info")]
    [SerializeField] private CaseProperty _causeOfDeath;
    [SerializeField] private CaseProperty _timeOfDeath;
    [SerializeField] private CaseProperty _locationOfDeath;
    [SerializeField] private List<string> _additionalInjuries;
    [SerializeField] private List<string> _additionalNotes;

    private string _undiscoveredText = "???";

    public string GetVictimName()
    {
        return _name.startAsDiscovered ? _name.propertyValue : _undiscoveredText;
    }

    public CharacterPortrait GetVictimPortrait()
    {
        return _portrait.startAsDiscovered ? _portrait.portrait : new CharacterPortrait();
    }

    public string GetVictimSummary()
    {
        return _summary.startAsDiscovered ? _summary.propertyValue : _undiscoveredText;
    }

    public string GetVictimAge()
    {
        return _age.startAsDiscovered ? _age.propertyValue : _undiscoveredText;
    }

    public string GetVictimResidence()
    {
        return _residence.startAsDiscovered ? _residence.propertyValue : _undiscoveredText;
    }

    public string GetVictimOccupation()
    {
        return _occupation.startAsDiscovered ? _occupation.propertyValue : _undiscoveredText;
    }


    public string GetCauseOfDeath()
    {
        return _causeOfDeath.startAsDiscovered ? _causeOfDeath.propertyValue : _undiscoveredText;
    }
    public string GetTimeOfDeath()
    {
        return _timeOfDeath.startAsDiscovered ? _timeOfDeath.propertyValue : _undiscoveredText;
    }
    public string GetLocationOfDeath()
    {
        return _locationOfDeath.startAsDiscovered ? _locationOfDeath.propertyValue : _undiscoveredText;
    }

    public List<string> GetAdditionalInjuries()
    {
        return _additionalInjuries;
    }

    public List<string> GetAdditionalNotes()
    {
        return _additionalNotes;
    }

    // reveal victim info using reflection
    public CharacterPortrait RevealVictimPortrait()
    {
        return _portrait.portrait;
    }
    
    public string RevealVictimProperty(string propertyName)
    {
        return this.GetType().GetProperty(propertyName).GetValue(this,null).ToString();
    }
}
