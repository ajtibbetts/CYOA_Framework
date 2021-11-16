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
        _nameText.text = _caseRecord.GetVictim().name;
        _portrait.GetComponent<Image>().sprite = _caseRecord.GetVictim().portrait.portraitSprite;
        _ageText.text = _caseRecord.GetVictim().age;
        _residenceText.text = _caseRecord.GetVictim().residence;
        _occupationText.text = _caseRecord.GetVictim().occupation;
        _summaryText.text = _caseRecord.GetVictim().summary;

        _codText.text = _caseRecord.GetVictim().causeOfDeath;
        _todText.text = _caseRecord.GetVictim().timeOfDeath;
        _lodText.text = _caseRecord.GetVictim().locationOfDeath;

    }

    public void ButtonPressedInUI(string screenNameToJumpTo)
    {
        
    }
}
