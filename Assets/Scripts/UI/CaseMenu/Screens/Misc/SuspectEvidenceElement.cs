using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CaseDataObjects;
using TMPro;

public class SuspectEvidenceElement : MonoBehaviour, IPointerClickHandler
{
    
    [SerializeField] private TextMeshProUGUI _evidenceName;
    [SerializeField] private TextMeshProUGUI _evidenceDescription;
    [SerializeField] private Image _evidenceImage;

    private EvidenceType _evidenceType;

    public static event Action<EvidenceType> onElementTapped;

    
    public void UpdateData(CaseEvidence evidenceData, EvidenceType type)
    {
        _evidenceName.text = evidenceData.GetEvidenceName();
        _evidenceDescription.text = evidenceData.GetEvidenceDescription();
        _evidenceImage.sprite = evidenceData.GetEvidencePortrait();
        _evidenceType = type;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        onElementTapped?.Invoke(_evidenceType);
    }
}
