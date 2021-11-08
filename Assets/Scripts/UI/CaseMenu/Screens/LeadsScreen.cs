using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CaseDataObjects;

public class LeadsScreen : CaseScreen
{
    [SerializeField] private Scrollbar vertScrollBar;
    [SerializeField] private GameObject leadContentScrollContainer;
    [SerializeField] private GameObject leadEntryPrefab;

    private List<ActiveLead> _leads = new List<ActiveLead>();
    private List<GameObject> _leadUIObjects = new List<GameObject>();

    
    public void resetContent(){
        vertScrollBar.value = 1;

        foreach(GameObject leadObject in _leadUIObjects)
        {
            Destroy(leadObject.gameObject);
        }

        _leadUIObjects.Clear();
    }

    public void AddLeadToContent(ActiveLead leadData)
    {
        GameObject leadToAdd = GameObject.Instantiate(leadEntryPrefab, Vector3.zero, Quaternion.identity, leadContentScrollContainer.transform);
        leadToAdd.GetComponent<TextMeshProUGUI>().text = leadData.lead.GetLead();
        if(leadData.isResolved)
        {
            leadToAdd.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        }
        _leadUIObjects.Add(leadToAdd);
    }

    public void AddEmptyEntry()
    {
        GameObject emptyLead = GameObject.Instantiate(leadEntryPrefab, Vector3.zero, Quaternion.identity, leadContentScrollContainer.transform);
        emptyLead.GetComponent<TextMeshProUGUI>().text = "No leads on this case yet. Get to investigating!";
        _leadUIObjects.Add(emptyLead);
    }

    public override void UpdateData()
    {
        resetContent();
        _leads = _caseRecord.GetLeads();
        if(_leads.Count < 1) AddEmptyEntry();
        else 
        {
            foreach(ActiveLead lead in _leads)
            {
                AddLeadToContent(lead);
            }
        }
    }
}
