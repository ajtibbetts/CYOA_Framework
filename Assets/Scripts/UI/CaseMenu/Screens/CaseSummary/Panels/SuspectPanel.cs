using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class SuspectPanel : PanelElement
{
    [SerializeField] private TextMeshProUGUI _suspectNameText;
    [SerializeField] private TextMeshProUGUI _totalSuspectsText;
    [SerializeField] private Image _suspectPortrait;
    [SerializeField] private Image _meansCheck;
    [SerializeField] private Image _motiveCheck;
    [SerializeField] private Image _oppCheck;

    [SerializeField] private Sprite _doneSprite;
    [SerializeField] private Sprite _notdoneSprite;
    [SerializeField] private Sprite _unknownPortrait;
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        CaseMenu.Instance.OpenSuspectsScreen(true);
    }

    public override void UpdateData(PlayerCaseRecord _caseRecord)
    {
        _totalSuspectsText.text = _caseRecord.GetSuspects().Count + " suspect(s) total";
        
        if(_caseRecord.GetPrimarySuspect() == null || _caseRecord.GetSuspects().Count < 1)
        {
            _suspectNameText.text = "Unknown";
            _suspectPortrait.sprite = _unknownPortrait;
            SetEvidenceImage(_meansCheck, false);
            SetEvidenceImage(_motiveCheck, false);
            SetEvidenceImage(_oppCheck, false);
            return;
        }

        var suspect = _caseRecord.GetPrimarySuspect();
        _suspectNameText.text = suspect.SuspectProfile.characterName;
        _suspectPortrait.sprite = suspect.SuspectProfile.portrait.portraitSprite;

        var hasMeans = suspect.ProposedMeans.GetEvidenceID() != "unassigned";
        var hasMotive = suspect.ProposedMotive.GetEvidenceID() != "unassigned";
        var hasOpp = suspect.ProposedOpportunity.GetEvidenceID() != "unassigned";
        SetEvidenceImage(_meansCheck, hasMeans);
        SetEvidenceImage(_motiveCheck, hasMotive);
        SetEvidenceImage(_oppCheck, hasOpp);

    }

    private void SetEvidenceImage(Image evidence, bool isAssigned)
    {
        evidence.sprite = isAssigned ? _doneSprite : _notdoneSprite;
    }
}
