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

    
    public override void UpdateData()
    {
        _caseTitle.text = CaseManager.Instance.GetActiveCaseTitle();
        _caseSummary.text = CaseManager.Instance.GetActiveCaseSummary();

        UpdateVictimSummaryData();
        UpdateSuspectSummaryData();
    }

    public void UpdateVictimSummaryData()
    {
        if(_caseRecord.GetVictim() == null) return;
        
        Sprite victimSprite = _caseRecord.GetVictim().VictimPortrait.portraitSprite;
        float offsetX = _caseRecord.GetVictim().VictimPortrait.thumbNailOffsetX;
        float offsetY = _caseRecord.GetVictim().VictimPortrait.thumbNailOffsetY;
        SetPortraitThumbnail(_victimPortrait, victimSprite, offsetX, offsetY);   
        
        
        _victimName.text = _caseRecord.GetVictim().VictimName;
        _victimCOD.text = _caseRecord.GetVictim().CauseOfDeath;
        _victimTOD.text = _caseRecord.GetVictim().TimeofDeath;
        _victimLOD.text = _caseRecord.GetVictim().LocationOfDeath;
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

}
