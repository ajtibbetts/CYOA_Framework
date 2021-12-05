using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using CaseDataObjects;

public class WarrantPanel : PanelElement
{
    [SerializeField] private TextMeshProUGUI _caseStatusText;
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        CaseMenu.Instance.OpenWarrantScreen();
    }

    public override void UpdateData(PlayerCaseRecord _caseRecord)
    {
        _caseStatusText.text = GetStatusText(_caseRecord.caseStatus);
    }

    private string GetStatusText(CaseStatus status)
    {
        switch(status)
        {
            case CaseStatus.PENDING_WARRANT:
                return "Arrest Warrant Pending. Investigate & Interrogate";
            case CaseStatus.PENDING_ARREST:
                return "Arrest Warrant Granted. Pending Suspect Arrest.";
            case CaseStatus.PENDING_CONFESSION:
                return "Suspect Arrested. Pending Murder Confession.";
            case CaseStatus.COMPLETE:
                return "Murder Confession Obtained.";
            default:
                return null;
        }
    }
}
