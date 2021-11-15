using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CaseLead", menuName = "CYOA/Case/Lead", order = 0)]
public class CaseLead : ScriptableObject {
    [SerializeField] private string _leadID;
    [SerializeField] [TextArea] private string _leadText;

    public string GetLeadID()
    {
        return _leadID;
    }

    public string GetLead()
    {
        return _leadText;
    }
}
