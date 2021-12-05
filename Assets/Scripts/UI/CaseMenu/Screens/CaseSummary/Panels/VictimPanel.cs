using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class VictimPanel : PanelElement
{
    [SerializeField] private TextMeshProUGUI _victimNameText;
    [SerializeField] private TextMeshProUGUI _codText;
    [SerializeField] private TextMeshProUGUI _todText;
    [SerializeField] private TextMeshProUGUI _lodText;
    [SerializeField] private Image _victimPortrait;
    
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        CaseMenu.Instance.OpenVictimDataScreen();
    }

    public override void UpdateData(PlayerCaseRecord _caseRecord)
    {
        var victim = _caseRecord.GetVictim();
        _victimNameText.text = $"Victim: {victim.name}";
        _codText.text = victim.causeOfDeath;
        _todText.text = victim.timeOfDeath;
        _lodText.text = victim.locationOfDeath;
        _victimPortrait.sprite = victim.portrait.portraitSprite;
    }
}
