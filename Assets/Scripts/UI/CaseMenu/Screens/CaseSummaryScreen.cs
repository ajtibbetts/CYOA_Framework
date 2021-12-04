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
    public static event Action<string> OnButtonPressed;
    [Header("Case Details")]
    [SerializeField] private TextMeshProUGUI _caseTitle;
    [SerializeField] private TextMeshProUGUI _caseSummary;
    

    
    public override void UpdateData()
    {
        if(!PlayerCaseRecord.Instance.onActiveCase) return; // exit if no case is active
        _caseTitle.text = CaseManager.Instance.GetActiveCaseTitle();
        _caseSummary.text = CaseManager.Instance.GetActiveCaseSummary();

    
    }

}
