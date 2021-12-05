using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class NotesPanel : PanelElement
{
    [SerializeField] private TextMeshProUGUI _notesCountText;
    public override void OnPointerClick(PointerEventData pointerEventData)
    {
        CaseMenu.Instance.OpenNotesScreen();
    }

    public override void UpdateData(PlayerCaseRecord _caseRecord)
    {
        _notesCountText.text = _caseRecord.GetNotes().Count.ToString();
    }
}
