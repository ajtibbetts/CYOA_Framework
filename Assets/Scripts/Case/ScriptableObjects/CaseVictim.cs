using System.Collections;
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
    [SerializeField] private string[] _additionalInjuries;
    [SerializeField] private string[] _additionalNotes;

    public string GetVictimName()
    {
        return _name.startAsDiscovered ? _name.propertyValue : null;
    }

    public Sprite GetVictimPortrait()
    {
        return _portrait.startAsDiscovered ? _portrait.portraitSprite : null;
    }

    public string GetVictimSummary()
    {
        return _summary.startAsDiscovered ? _summary.propertyValue : null;
    }

    public string GetVictimAge()
    {
        return _age.startAsDiscovered ? _age.propertyValue : null;
    }

    public string GetVictimResidence()
    {
        return _residence.startAsDiscovered ? _residence.propertyValue : null;
    }

    public string GetVictimOccupation()
    {
        return _occupation.startAsDiscovered ? _occupation.propertyValue : null;
    }


    public string GetCauseOfDeath()
    {
        return _causeOfDeath.startAsDiscovered ? _causeOfDeath.propertyValue : null;
    }
    public string GetTimeOfDeath()
    {
        return _timeOfDeath.startAsDiscovered ? _timeOfDeath.propertyValue : null;
    }
    public string GetLocationOfDeath()
    {
        return _locationOfDeath.startAsDiscovered ? _locationOfDeath.propertyValue : null;
    }

    public string[] GetAdditionalInjuries()
    {
        return _additionalInjuries;
    }

    public string[] GetAdditioanlNotes()
    {
        return _additionalNotes;
    }
}
