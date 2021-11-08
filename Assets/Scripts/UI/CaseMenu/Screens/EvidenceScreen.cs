using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CaseDataObjects;

public class EvidenceScreen : CaseScreen
{
    [SerializeField] private Scrollbar vertScrollBar;
    [SerializeField] private GameObject evidenceContentScrollContainer;
    [SerializeField] private GameObject evidenceEntryPrefab;

    private List<CaseEvidence> _evidence = new List<CaseEvidence>();
    private List<GameObject> _contentUIObjects = new List<GameObject>();

    public void resetContent(){
        vertScrollBar.value = 1;

        foreach(GameObject leadObject in _contentUIObjects)
        {
            Destroy(leadObject.gameObject);
        }

        _contentUIObjects.Clear();
    }

    public void AddEvidenceToContent(CaseEvidence evidenceData)
    {
        GameObject evidenceToAdd = GameObject.Instantiate(evidenceEntryPrefab, Vector3.zero, Quaternion.identity, evidenceContentScrollContainer.transform);
        
        var evidenceImage = evidenceToAdd.transform.Find("Image");
        var evidenceTitle = evidenceToAdd.transform.Find("evidenceNameText");
        var evidenceDescription = evidenceToAdd.transform.Find("evidenceDescriptionText").GetComponent<TextMeshProUGUI>();
        
        evidenceImage.GetComponent<Image>().sprite = evidenceData.GetEvidencePortrait();
        evidenceTitle.GetComponent<TextMeshProUGUI>().text = evidenceData.GetEvidenceName();
        evidenceDescription.text = evidenceData.GetEvidenceDescription();
        
        // check if the description text is needing to expand the container panel height
        var textHeight = evidenceDescription.GetComponent<TextMeshProUGUI>().GetPreferredValues().y;
        Debug.Log("overflowing preferred height: " + textHeight);
        if(textHeight >= 25.01f) // tweaked value based on default w/o overflow.
        {
            var boxSize = evidenceToAdd.GetComponent<RectTransform>().sizeDelta;
            evidenceToAdd.GetComponent<RectTransform>().sizeDelta = new Vector2(boxSize.x, boxSize.y + textHeight);
        }
        
        // evidenceToAdd.GetComponent<TextMeshProUGUI>().text = leadData.lead.GetLead();
        // if(leadData.isResolved)
        // {
        //     evidenceToAdd.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        // }
        _contentUIObjects.Add(evidenceToAdd);
    }

    public void AddEmptyEntry()
    {
        GameObject evidenceToAdd = GameObject.Instantiate(evidenceEntryPrefab, Vector3.zero, Quaternion.identity, evidenceContentScrollContainer.transform);
        _contentUIObjects.Add(evidenceToAdd);
    }


    public override void UpdateData()
    {
       resetContent();
        _evidence = _caseRecord.GetEvidence();
        if(_evidence.Count < 1) AddEmptyEntry();
        else 
        {
            foreach(CaseEvidence evidence in _evidence)
            {
                AddEvidenceToContent(evidence);
            }
        }

    }
    
}
