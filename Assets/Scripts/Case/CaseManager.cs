using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseManager : MonoBehaviour
{
    
    private static CaseManager _instance;
    public static CaseManager Instance { get { return _instance; } }

    [SerializeField] private CaseScenario _activeCase;

    private void Awake() {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    public void GetNewCase(CaseScenario newScenario)
    {
        _activeCase = newScenario;
    }

    public void IniitializeCase(PlayerCaseRecord caseRecord)
    {

    }

    public bool ValidateArrestWarrant(CaseSuspect suspect)
    {
        return true;
    }

    public void BeginInterrogation()
    {

    }

    public void CompleteCase()
    {
        
    }
    
}
