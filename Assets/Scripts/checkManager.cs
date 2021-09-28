using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkManager : MonoBehaviour
{
    [HideInInspector] public gameController controller;

    // events
    public event Action onRollCheckStart;
    public event Action onRollCheckPass;
    public event Action onRollCheckFail;
    
    private void Awake() {
            controller = GetComponent<gameController>();  
            
    }

    
    // Start is called before the first frame update
    void Start()
    {
        controller.DialogueParser.onPlayerSkillRoll = RollCheck;
    }

    public bool RollCheck(string checkName, string checkValue)
    {
        var result = true;
        // for now
        Debug.Log($"CHECK MANAGER SKILL CHECK: {checkName} against {checkValue}");
        onRollCheckStart?.Invoke();
        if(result)
        {
            onRollCheckPass?.Invoke();
        }
        else 
        {
            onRollCheckFail?.Invoke();
        }
        return result;
    }


}
