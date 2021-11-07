using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictimDataScreen : MonoBehaviour
{
    private PlayerCaseRecord _caseRecord;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _portrait;
    [SerializeField] private TextMeshProUGUI _ageText;
    [SerializeField] private TextMeshProUGUI _residenceText;
    [SerializeField] private TextMeshProUGUI _occupationText;
    [SerializeField] private TextMeshProUGUI _summaryText;
    [SerializeField] private TextMeshProUGUI _codText;
    [SerializeField] private TextMeshProUGUI _todText;
    [SerializeField] private TextMeshProUGUI _lodText;

    

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCaseRecord(PlayerCaseRecord record)
    {
        _caseRecord = record;
    }

    public void UpdateData()
    {
        _nameText.text = _caseRecord.GetVictim().VictimName;
        _portrait.GetComponent<Image>().sprite = _caseRecord.GetVictim().VictimPortrait;
        _ageText.text = _caseRecord.GetVictim().VictimAge;
        _residenceText.text = _caseRecord.GetVictim().VictimResidence;
        _occupationText.text = _caseRecord.GetVictim().VictimOccupation;
        _summaryText.text = _caseRecord.GetVictim().VictimSummary;

        _codText.text = _caseRecord.GetVictim().CauseOfDeath;
        _todText.text = _caseRecord.GetVictim().TimeofDeath;
        _lodText.text = _caseRecord.GetVictim().LocationOfDeath;

    }

    public void ButtonPressedInUI(string screenNameToJumpTo)
    {
        
    }
}
