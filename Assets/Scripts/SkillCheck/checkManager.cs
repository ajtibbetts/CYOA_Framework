using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkManager : MonoBehaviour
{
    [HideInInspector] public gameController controller;
    [Tooltip("Enter a threshold value to check against 2d6 roll.")]
    
    public int TestSkillValue = 0;
    public int TestThreshHoldValue = 0;
    

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

    [ContextMenu ("Roll 2d6 against testThreshold")]
    void RollAgainstTestThreshold () {
        Debug.Log ($"Checking probability to beat {TestThreshHoldValue} on 2d6 roll");
        int successCount = 0;
        for(int i = 1; i <= 6; i++)
        {
            for(int j = 1; j <= 6; j++)
            {
                if(i + j + TestSkillValue > TestThreshHoldValue) successCount ++;
            }
        }
        Debug.Log("Total success counts:" + successCount);
        float probabilityTotal = successCount / 36.00f;
        Debug.Log($"Probability of rolling a {TestThreshHoldValue} or higher on (2d6 + {TestSkillValue}) is {probabilityTotal.ToString("p")}");
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
