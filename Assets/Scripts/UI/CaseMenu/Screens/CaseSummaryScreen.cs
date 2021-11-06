using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CaseSummaryScreen : MonoBehaviour
{
    private PlayerCaseRecord _caseRecord;

    public static event Action<string> OnButtonPressed;
    [Header("Case Details")]
    [SerializeField] private TextMeshProUGUI _caseTitle;
    [SerializeField] private TextMeshProUGUI _caseSummary;
    [Header("Victim Data")]
    [SerializeField] private TextMeshProUGUI _victimName;
    [SerializeField] private Image _victimPortrait;
    [SerializeField] private TextMeshProUGUI _victimCOD;
    [SerializeField] private TextMeshProUGUI _victimTOD;
    [SerializeField] private TextMeshProUGUI _victimLOD;
    [Header("Suspect Data")]
    [SerializeField] private TextMeshProUGUI _suspectName;
    [SerializeField] private Image _suspectPortrait;
    [SerializeField] private TextMeshProUGUI _suspectRelation;
    [SerializeField] private Image _suspectMeansImage;
    [SerializeField] private TextMeshProUGUI _suspectMeansText;
    [SerializeField] private Image _suspectMotiveImage;
    [SerializeField] private TextMeshProUGUI _suspectMotiveText;
    [SerializeField] private Image _suspectOpportunityImage;
    [SerializeField] private TextMeshProUGUI _suspectOpportunityText;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCaseRecord(PlayerCaseRecord record)
    {
        _caseRecord = record;
    }

    public void UpdateData()
    {
        _caseTitle.text = CaseManager.Instance.GetActiveCaseTitle();
        _caseSummary.text = CaseManager.Instance.GetActiveCaseSummary();

        _victimName.text = _caseRecord.GetVictim().VictimName;
        _victimPortrait.GetComponent<Image>().sprite = _caseRecord.GetVictim().VictimPortrait;
        _victimCOD.text = _caseRecord.GetVictim().CauseOfDeath;
        _victimTOD.text = _caseRecord.GetVictim().TimeofDeath;
        _victimLOD.text = _caseRecord.GetVictim().LocationOfDeath;

        _suspectName.text = _caseRecord.GetPrimarySuspect().SuspectProfile._characterName;
        _suspectPortrait.GetComponent<Image>().sprite = _caseRecord.GetPrimarySuspect().SuspectProfile._portrait;
        _suspectRelation.text = _caseRecord.GetPrimarySuspect().SuspectProfile._relationshipToVictim;

        _suspectMeansImage.GetComponent<Image>().sprite = _caseRecord.GetPrimarySuspect().ProposedMeans.GetEvidencePortrait();
        _suspectMeansText.text = _caseRecord.GetPrimarySuspect().ProposedMeans.GetEvidenceName();
        _suspectMotiveImage.GetComponent<Image>().sprite = _caseRecord.GetPrimarySuspect().ProposedMotive.GetEvidencePortrait();
        _suspectMotiveText.text = _caseRecord.GetPrimarySuspect().ProposedMotive.GetEvidenceName();
        _suspectOpportunityImage.GetComponent<Image>().sprite = _caseRecord.GetPrimarySuspect().ProposedOpportunity.GetEvidencePortrait();
        _suspectOpportunityText.text = _caseRecord.GetPrimarySuspect().ProposedOpportunity.GetEvidenceName();
    }

    public void ButtonPressedInUI(string screenNameToJumpTo)
    {
        OnButtonPressed?.Invoke(screenNameToJumpTo);
        Debug.Log("Jumping to screen: " + screenNameToJumpTo);
    }
}
