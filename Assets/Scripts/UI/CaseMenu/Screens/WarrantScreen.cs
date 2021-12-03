using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using globalDataTypes;
using CaseDataObjects;

public class WarrantScreen : CaseScreen
{
    public static event Action<string> OnButtonPressed;
    [Header("Case Details")]
    [SerializeField] private TextMeshProUGUI _warrantStatus;
    [SerializeField] private TextMeshProUGUI _warrantInfoText;
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
    [SerializeField] private GameObject _changeTheoryButton;

    [Header("Warrant")]
    [SerializeField] TextMeshProUGUI _requestButtonInfoText;
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

    
    // static texts
    private string _warrantStatusText_pending = "To request a warrant:";
    private string _warrantStatusText_granted = "Arrest Warrant Granted";
    private string _warrantStatusText_arrested = "Suspect Arrested";
    private string _warrantStatusText_complete = "Case Solved";
    private string _warrantInfoText_pending = "Determine the victim's cause, time, and location of death.\nAssign a Primary Suspect and attach evidence for Means, Motive, and Opportunity.";
    private string _warrantInfoText_granted = "Locate the suspect and arrest them.";
    private string _warrantInfoText_arrested = "Interrogate the suspect and get a confession.";
    private string _warrantInfoText_complete = "Homicide solved. Great work Detective!";


    private string _requestLabel_pending = "You may request an\narrest warrant when <b>Investigating</b>.";
    private string _requestLabel_granted = "Arrest warrant granted for:\n";
    private string _requestLabel_arrested = "Suspect arrested:\n";
    private string _requestLabel_complete = "Case solved.";

    public override void UpdateData()
    {
        if(!PlayerCaseRecord.Instance.onActiveCase) return; // exit if no case is active
        UpdateWarrantStatus();
        UpdateVictimSummaryData();
        UpdateSuspectSummaryData();
        CloseRequestWarrantScreen();
        SetWarrantButton();
    }

    public void UpdateWarrantStatus()
    {
        switch(_caseRecord.caseStatus)
        {
            case CaseStatus.PENDING_WARRANT:
                _warrantStatus.text = _warrantStatusText_pending;
                _warrantInfoText.text = _warrantInfoText_pending;
            return;
            case CaseStatus.PENDING_ARREST:
                _warrantStatus.text = _warrantStatusText_granted;
                _warrantInfoText.text = _warrantInfoText_granted;
            return;
            case CaseStatus.PENDING_CONFESSION:
                _warrantStatus.text = _warrantStatusText_arrested;
                _warrantInfoText.text = _warrantInfoText_arrested;
            return;
            case CaseStatus.COMPLETE:
                _warrantStatus.text = _warrantStatusText_complete;
                _warrantInfoText.text = _warrantInfoText_complete;
            return;
        }
    }

    public void SetWarrantButton()
    {
        switch(_caseRecord.caseStatus)
        {
            case CaseStatus.PENDING_WARRANT:
                if(gameController.Instance.GetGameState() == GAMESTATE.WORLDNAVIGATION)
                {
                    _requestButtonInfoText.text = _requestLabel_pending;
                    _requestWarrantButton.GetComponentInChildren<TextMeshProUGUI>().text = "Request Arrest Warrant";
                    _requestWarrantButton.GetComponent<Button>().interactable = true;
                }
                else 
                {
                    _requestWarrantButton.GetComponentInChildren<TextMeshProUGUI>().text = "Warrant Request Unavailable";
                    _requestWarrantButton.GetComponent<Button>().interactable = false;
                }
            return;
            case CaseStatus.PENDING_ARREST:
                _requestButtonInfoText.text = _requestLabel_granted;
                _requestWarrantButton.GetComponentInChildren<TextMeshProUGUI>().text = "Arrest Warrant Granted";
                _requestWarrantButton.GetComponent<Button>().interactable = false;
            return;
            case CaseStatus.PENDING_CONFESSION:
                _requestButtonInfoText.text = _requestLabel_arrested;
                _requestWarrantButton.GetComponentInChildren<TextMeshProUGUI>().text = "Suspect Arrested";
                _requestWarrantButton.GetComponent<Button>().interactable = false;
            return;
            case CaseStatus.COMPLETE:
                _requestButtonInfoText.text = _requestLabel_complete;
                _requestWarrantButton.GetComponentInChildren<TextMeshProUGUI>().text = "Case Solved";
                _requestWarrantButton.GetComponent<Button>().interactable = false;
            return;
        }
        
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

        if(_caseRecord.caseStatus != CaseStatus.PENDING_WARRANT) _changeTheoryButton.SetActive(false);
        else _changeTheoryButton.SetActive(true);
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

    public void RequestArrestWarrant()
    {
        if(gameController.Instance.GetGameState() == GAMESTATE.DIALOGUE)
        {
            Debug.Log("WARRANT SCREEN ---- Cannot Request Warrant while in Dialogue");
            return;
        }
        Debug.Log("WARRANT SCREEN ---- Submitting request for warrant!");
        WorldNavigator.Instance.GetActiveNavObject().DeactivateNavObject();
        gameController.Instance.SwitchToLevel("warrantRequest");
        CloseMenu(MENUTYPE.CASEMENU);
    }
}
