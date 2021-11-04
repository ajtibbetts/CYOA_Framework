using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CaseEvidence", menuName = "CYOA/Case/Evidence", order = 0)]
public class CaseEvidence : ScriptableObject {
    [SerializeField] private string _evidenceName;
    [SerializeField] private Sprite _evidencePortrait;

    [SerializeField] [TextArea] string _evidenceDescription;
    [SerializeField] private DialogueContainer _evidenceDialogue;

    public string GetEvidenceName()
    {
        return _evidenceName;
    }

    public Sprite GetEvidencePortrait()
    {
        return _evidencePortrait;
    }
    
    public string GetEvidenceDescription()
    {
        return _evidenceDescription;
    }

    public void ExamineEvidence()
    {
        // use dialogue container here
    }
}
