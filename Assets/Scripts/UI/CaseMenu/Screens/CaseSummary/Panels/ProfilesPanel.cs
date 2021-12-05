using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ProfilesPanel : PanelElement
{
    [SerializeField] private TextMeshProUGUI _profilesCountText;
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        CaseMenu.Instance.OpenProfilesScreen();
    }

    public override void UpdateData(PlayerCaseRecord _caseRecord)
    {
        _profilesCountText.text = _caseRecord.GetProfiles().Count.ToString();
    }
}
