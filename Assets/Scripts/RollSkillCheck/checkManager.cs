using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using globalDataTypes;

public class checkManager : MonoBehaviour
{
    private static checkManager _instance;
    public static checkManager Instance { get { return _instance; } }


    // events
    public static event Action onRollCheckStart;
    public static event Action<string, string, int> onRollDataSet;

    public static event Action<int, int, int, bool> onRollResultSent;
    
    public static event Action<bool, rollCheckResultType> onRollCheckComplete;
    
    [HideInInspector] public gameController controller;
    [Tooltip("Enter a threshold value to check against 2d6 roll.")]
    
    public int TestSkillValue = 0;
    public int TestThreshHoldValue = 0;


    private string _rollSkillName;
    private int _rollSkillValue;
    private string _rollDescription;
    private int _rollDifficulty;

    private int _leftResult;
    private int _rightResult;
    private bool _passedCheck;
    

    
    
    private void Awake() {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        controller = GetComponent<gameController>();
    }

    
    // Start is called before the first frame update
    void Start()
    {
        // controller.DialogueParser.onPlayerSkillRoll = RollCheck;
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

    public static double GetProbability(int playerSkillValue, int difficultyValue)
    {
        int successCount = 0;
        for(int i = 1; i <= 6; i++)
        {
            for(int j = 1; j <= 6; j++)
            {
                if(i + j + playerSkillValue >= difficultyValue) successCount ++;
            }
        }
        float probabilityTotal = successCount / 36.00f;
        var result = Math.Round(probabilityTotal * 100, 0, MidpointRounding.AwayFromZero);
        Debug.Log("probability result: " + result);
        return result;
    }

    public void ProcessRollCheck()
    {
        System.Random rnd = new System.Random();
        _leftResult = rnd.Next(1,7);
        _rightResult = rnd.Next(1,7);
        int totalRoll = _leftResult + _rightResult + _rollSkillValue;
        _passedCheck =  totalRoll >= _rollDifficulty;
        onRollResultSent?.Invoke(_leftResult, _rightResult, _rollSkillValue, _passedCheck);
    }


    public void InitializeRollCheck(string skillName, string rollDescription, string rollDifficulty)
    {
        try {
            _rollSkillName = skillName;
            _rollSkillValue = Player.Instance.GetSkillValue(skillName);
            _rollDescription = rollDescription;
            _rollDifficulty = Int32.Parse(rollDifficulty);
        }
        catch (FormatException e)
        {
            Debug.LogError($"ERROR CONVERTING DIFFICULTY TO INT ({rollDifficulty}). Exception: {e}");
        }
        




        // enable UI once ready
        onRollDataSet?.Invoke(_rollSkillName, _rollDescription, _rollDifficulty);
        onRollCheckStart?.Invoke();
    }


    public void CompleteRollSequence()
    {
        Debug.Log("CHECK MANAGER ---- Completing roll sequence");

        onRollCheckComplete?.Invoke(_passedCheck, GetRollCheckResultType());
    }

    public rollCheckResultType GetRollCheckResultType()
    {
        // check master fumble
        if(_leftResult + _rightResult == 2 && _passedCheck) return rollCheckResultType.MASTERFUMBLE;
        
        // check master pass 
        if(_rollSkillValue + 2 >= _rollDifficulty) return rollCheckResultType.MASTERPASS;

        // check regular pass
        if(_passedCheck) return rollCheckResultType.PASS;

        // check critical fail
        if(!_passedCheck && _leftResult + _rightResult == 2) return rollCheckResultType.CRITICALFAIL;

        // all else should be regular fail
        return rollCheckResultType.FAIL;
    }

}
