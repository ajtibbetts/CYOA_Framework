using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class LeadsPanel : PanelElement
{
    [SerializeField] private TextMeshProUGUI _leadsCountText;
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        CaseMenu.Instance.OpenLeadsScreen();
    }

    public override void UpdateData(PlayerCaseRecord _caseRecord)
    {
        _leadsCountText.text = _caseRecord.GetLeads().Count.ToString();
    }
}
