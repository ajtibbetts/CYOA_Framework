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
        evidenceToAdd.GetComponent<EvidenceEntryElement>().UpdateData(evidenceData, EvidenceType.MEANS);
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
