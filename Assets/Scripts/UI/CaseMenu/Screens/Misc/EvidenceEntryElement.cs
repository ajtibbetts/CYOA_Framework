using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CaseDataObjects;
using TMPro;

public class EvidenceEntryElement : MonoBehaviour, IPointerClickHandler
{
    
    private CaseEvidence _data;
    private EvidenceType _type;

    public static event Action<CaseEvidence, EvidenceType> onElementTapped;
    // Start is called before the first frame update
    public void UpdateData(CaseEvidence evidenceData, EvidenceType type)
    {
        _data = evidenceData;
        _type = type;

        var evidenceImage = this.transform.Find("Image");
        var evidenceTitle = this.transform.Find("evidenceNameText");
        var evidenceDescription = this.transform.Find("evidenceDescriptionText").GetComponent<TextMeshProUGUI>();
        
        evidenceImage.GetComponent<Image>().sprite = evidenceData.GetEvidencePortrait();
        evidenceTitle.GetComponent<TextMeshProUGUI>().text = evidenceData.GetEvidenceName();
        evidenceDescription.text = evidenceData.GetEvidenceDescription();
        
        // check if the description text is needing to expand the container panel height
        var textHeight = evidenceDescription.GetComponent<TextMeshProUGUI>().GetPreferredValues().y;
        Debug.Log("overflowing preferred height: " + textHeight);
        if(textHeight >= 25.01f) // tweaked value based on default w/o overflow.
        {
            var boxSize = this.GetComponent<RectTransform>().sizeDelta;
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(boxSize.x, boxSize.y + textHeight);
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        onElementTapped?.Invoke(_data, _type);
    }
}
