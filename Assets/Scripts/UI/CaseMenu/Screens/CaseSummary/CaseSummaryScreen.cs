using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using globalDataTypes;
using CaseDataObjects;

public class CaseSummaryScreen : CaseScreen
{
    // public static event Action<string> OnButtonPressed;
    [Header("Case Details")]
    [SerializeField] private TextMeshProUGUI _caseTitle;
    [SerializeField] private TextMeshProUGUI _caseSummary;
    
    [SerializeField] private List<PanelElement> _panels = new List<PanelElement>();

    
    public override void UpdateData()
    {
        if(!PlayerCaseRecord.Instance.onActiveCase) return; // exit if no case is active
        _caseTitle.text = CaseManager.Instance.GetActiveCaseTitle();
        _caseSummary.text = CaseManager.Instance.GetActiveCaseSummary();

        foreach(var panel in _panels)
        {
            panel.UpdateData(_caseRecord);
        }
    }

}
