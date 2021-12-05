using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class EvidencePanel : PanelElement
{
    [SerializeField] private TextMeshProUGUI _evidenceCountText;
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        CaseMenu.Instance.OpenEvidenceScreen();
    }

    public override void UpdateData(PlayerCaseRecord _caseRecord)
    {
        _evidenceCountText.text = _caseRecord.GetEvidence().Count.ToString();
    }
}
