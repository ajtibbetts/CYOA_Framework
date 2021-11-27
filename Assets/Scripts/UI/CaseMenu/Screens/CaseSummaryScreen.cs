using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CaseSummaryScreen : CaseScreen
{
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
    [SerializeField] private GameObject _activeSuspectContainer;
    [SerializeField] private GameObject _noActiveSuspectContainer;
    

    [SerializeField] private TextMeshProUGUI _suspectName;
    [SerializeField] private Image _suspectPortrait;
    [SerializeField] private TextMeshProUGUI _suspectRelation;
    [SerializeField] private Image _suspectMeansImage;
    [SerializeField] private TextMeshProUGUI _suspectMeansText;
    [SerializeField] private Image _suspectMotiveImage;
    [SerializeField] private TextMeshProUGUI _suspectMotiveText;
    [SerializeField] private Image _suspectOpportunityImage;
    [SerializeField] private TextMeshProUGUI _suspectOpportunityText;

    [Header("Warrant")]
    [SerializeField] private GameObject _requestWarrantButton;
    [SerializeField] private GameObject _warrantRequestScreen;
    [SerializeField] private Toggle _codToggle;
    [SerializeField] private Toggle _todToggle;
    [SerializeField] private Toggle _lodToggle;
    [SerializeField] private Toggle _suspectToggle;
    [SerializeField] private Toggle _meansToggle;
    [SerializeField] private Toggle _motiveToggle;
    [SerializeField] private Toggle _opportunityToggle;
    [SerializeField] private GameObject _submitRequestButton;
    [SerializeField] private GameObject _cancelRequestButton;
    private bool _isWarrantRequestMatched;

    
    public override void UpdateData()
    {
        if(!PlayerCaseRecord.Instance.onActiveCase) return; // exit if no case is active
        _caseTitle.text = CaseManager.Instance.GetActiveCaseTitle();
        _caseSummary.text = CaseManager.Instance.GetActiveCaseSummary();

        UpdateVictimSummaryData();
        UpdateSuspectSummaryData();
        CloseRequestWarrantScreen();
    }

    public void UpdateVictimSummaryData()
    {
        if(_caseRecord.GetVictim() == null) return;
        
        Sprite victimSprite = _caseRecord.GetVictim().portrait.portraitSprite;
        float offsetX = _caseRecord.GetVictim().portrait.thumbNailOffsetX;
        float offsetY = _caseRecord.GetVictim().portrait.thumbNailOffsetY;
        SetPortraitThumbnail(_victimPortrait, victimSprite, offsetX, offsetY);   
        
        
        _victimName.text = _caseRecord.GetVictim().name;
        _victimCOD.text = _caseRecord.GetVictim().causeOfDeath;
        _victimTOD.text = _caseRecord.GetVictim().timeOfDeath;
        _victimLOD.text = _caseRecord.GetVictim().locationOfDeath;
    }

    public void UpdateSuspectSummaryData()
    {
        if(_caseRecord.GetPrimarySuspect() == null || _caseRecord.GetSuspects().Count < 1)
        {
            _activeSuspectContainer.SetActive(false);
            _noActiveSuspectContainer.SetActive(true);
            return;
        }
        
        
        Sprite suspectSprite = _caseRecord.GetPrimarySuspect().SuspectProfile.portrait.portraitSprite;
        float offsetX = _caseRecord.GetPrimarySuspect().SuspectProfile.portrait.thumbNailOffsetX;
        float offsetY = _caseRecord.GetPrimarySuspect().SuspectProfile.portrait.thumbNailOffsetY;
        SetPortraitThumbnail(_suspectPortrait, suspectSprite, offsetX, offsetY);   

        _suspectName.text = _caseRecord.GetPrimarySuspect().SuspectProfile.characterName;
        _suspectRelation.text = _caseRecord.GetPrimarySuspect().SuspectProfile.relationshipToVictim;

        _suspectMeansImage.GetComponent<Image>().sprite = _caseRecord.GetPrimarySuspect().ProposedMeans.GetEvidencePortrait();
        _suspectMeansText.text = _caseRecord.GetPrimarySuspect().ProposedMeans.GetEvidenceName();
        _suspectMotiveImage.GetComponent<Image>().sprite = _caseRecord.GetPrimarySuspect().ProposedMotive.GetEvidencePortrait();
        _suspectMotiveText.text = _caseRecord.GetPrimarySuspect().ProposedMotive.GetEvidenceName();
        _suspectOpportunityImage.GetComponent<Image>().sprite = _caseRecord.GetPrimarySuspect().ProposedOpportunity.GetEvidencePortrait();
        _suspectOpportunityText.text = _caseRecord.GetPrimarySuspect().ProposedOpportunity.GetEvidenceName();

        _noActiveSuspectContainer.SetActive(false);
        _activeSuspectContainer.SetActive(true);
    }

    

    public void ButtonPressedInUI(string screenNameToJumpTo)
    {
        OnButtonPressed?.Invoke(screenNameToJumpTo);
        Debug.Log("Jumping to screen: " + screenNameToJumpTo);
    }



    // Warrant Screen Methods

    public void OpenRequestWarrantScreen()
    {
        CheckWarrantCriteria();
        _warrantRequestScreen.SetActive(true);
    }

    public void CheckWarrantCriteria()
    {
        bool warrantCriteriaMet = false;

        bool codDetermined = false;
        bool todDetermined = false;
        bool lodDetermined = false;

        bool suspectDetermined = false;
        bool meansDetermined = false;
        bool motiveDetermined = false;
        bool opportunityDetermined = false;
        
        if(_caseRecord.GetVictim() == null)
        {
            warrantCriteriaMet = false;
            _codToggle.isOn = false;
            _todToggle.isOn = false;
            _lodToggle.isOn = false;
        }
        else 
        {
            codDetermined = _caseRecord.GetVictim().causeOfDeath != "???";
            _codToggle.isOn = codDetermined;

            todDetermined = _caseRecord.GetVictim().timeOfDeath != "???";
            _todToggle.isOn = todDetermined;

            lodDetermined = _caseRecord.GetVictim().locationOfDeath != "???";
            _lodToggle.isOn = lodDetermined;
        }

    

        // suspect
        if(_caseRecord.GetPrimarySuspect() == null)
        {
            warrantCriteriaMet = false;
            _suspectToggle.isOn = false;
            _meansToggle.isOn = false;
            _motiveToggle.isOn = false;
            _opportunityToggle.isOn = false;
        }
        else 
        {
            suspectDetermined = true;
            _suspectToggle.isOn = true;
            meansDetermined = _caseRecord.GetPrimarySuspect().ProposedMeans.GetEvidenceID() != "unassigned";
            _meansToggle.isOn = meansDetermined;
            motiveDetermined = _caseRecord.GetPrimarySuspect().ProposedMotive.GetEvidenceID() != "unassigned";
            _motiveToggle.isOn = motiveDetermined;
            opportunityDetermined = _caseRecord.GetPrimarySuspect().ProposedOpportunity.GetEvidenceID() != "unassigned";
            _opportunityToggle.isOn = opportunityDetermined;
        }

        // check all criteria is met
        warrantCriteriaMet = codDetermined && todDetermined && lodDetermined && 
                             suspectDetermined && meansDetermined && motiveDetermined && opportunityDetermined;

        _submitRequestButton.SetActive(warrantCriteriaMet);

    }

    public void CloseRequestWarrantScreen()
    {
        _warrantRequestScreen.SetActive(false);
    }
}
