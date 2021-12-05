using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HelpPanel : PanelElement
{
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        CaseMenu.Instance.OpenHelpScreen();
    }

    public override void UpdateData(PlayerCaseRecord _caseRecord)
    {
        // do nothing for help
    }
}
