using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CaseLead", menuName = "CYOA/Case/Lead", order = 0)]
public class CaseLead : ScriptableObject {
    [SerializeField] [TextArea] private string _leadText;

    public string GetLead()
    {
        return _leadText;
    }
}
