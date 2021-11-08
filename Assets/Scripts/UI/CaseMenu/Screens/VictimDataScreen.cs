using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictimDataScreen : CaseScreen
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _portrait;
    [SerializeField] private TextMeshProUGUI _ageText;
    [SerializeField] private TextMeshProUGUI _residenceText;
    [SerializeField] private TextMeshProUGUI _occupationText;
    [SerializeField] private TextMeshProUGUI _summaryText;
    [SerializeField] private TextMeshProUGUI _codText;
    [SerializeField] private TextMeshProUGUI _todText;
    [SerializeField] private TextMeshProUGUI _lodText;

    public override void UpdateData()
    {
        _nameText.text = _caseRecord.GetVictim().VictimName;
        _portrait.GetComponent<Image>().sprite = _caseRecord.GetVictim().VictimPortrait.portraitSprite;
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
