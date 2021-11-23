using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using globalDataTypes;

[Serializable]
public class Player : MonoBehaviour
{
    private static Player _instance;
    public static Player Instance { get { return _instance; } }
    
    // player name
    [Header("Player Name")]
    public string PlayerFirstName;
    public string PlayerLastName;
    public int Gender; // 0 male, 1 female, 2 n/a


    [Header("Player Experience")]
    public int CurrentRank;
    public int ExperienceToNextRank;
    
    // base stats
    [Header("Player Stats")]
    public int CurrentHealth;
    public int MaxHealth;
    public int CurrentEnergy;
    public int MaxEnergy;
    public int CurrentResolve;
    public int MaxResolve;
    public int CurrentFin; // gold/currency

    // base stats
    [Header("Player Aspects")]
    public int Physical;
    public int Cognitive;
    public int Volitional;
    public int Narrative;
    public int Social;

    // skills
    [Header("Player Skills")]
    //physical
    private int Athletics = 0;
    private int Fortitude = 0;
    private int Stealth = 0;
    private int HardboiledHotshot = 0;
    //cognitive
    private int Ballistics = 0;
    private int Forensics = 0;
    private int Medicine = 0;
    private int CyberSleuth = 0;
    //volitional
    private int Focus = 0;
    public int Tactics = 0;
    private int GreySight = 0;
    private int CaseChaser = 0;
    //narrative
    private int Causality = 0;
    private int Reasoning = 0;
    private int Occult = 0;
    private int AceInspector = 0;
    // social
    private int Emotion = 0;
    private int Confidence = 0;
    private int Wisdom = 0;
    private int StreetSavant = 0;

    // skills retrieval
    // private Dictionary<string, int> GetSkill = new Dictionary<string, int>();
    private Dictionary<string, Func<int>> _skillsDict = new Dictionary<string, Func<int>>();


    private void Awake() {
        //init singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        SetTestData();
        SetupSkillsDictionary();
    }

    public void LoadSaveData(Player savedPlayerData)
    {
        _instance = savedPlayerData;
    }
    

    public void SetTestData()
    {
        // Debug.Log($"Setting test data!");
        PlayerFirstName = "Jane";
        PlayerLastName = "Falco";
        Gender = 1;
        CurrentRank = 1;
        ExperienceToNextRank = 100;
        MaxHealth = 25;
        CurrentHealth = MaxHealth;
        MaxEnergy = 20;
        CurrentEnergy = MaxEnergy;
        MaxResolve = 10;
        CurrentResolve = MaxResolve;
        CurrentFin = 500;

        Physical = 2;
        Cognitive = 3;
        Volitional = 3;
        Narrative = 2;
        Social = 5;
    }

    public void SetupSkillsDictionary()
    {
        _skillsDict.Add("Athletics",()=> {return Physical + Athletics;});
        _skillsDict.Add("Fortitude",()=> {return Physical + Fortitude;});
        _skillsDict.Add("Stealth",()=> {return Physical + Stealth;});
        _skillsDict.Add("HardboiledHotshot",()=> {return Physical + HardboiledHotshot;});

        _skillsDict.Add("Ballistics",()=> {return Cognitive + Ballistics;});
        _skillsDict.Add("Forensics",()=> {return Cognitive + Forensics;});
        _skillsDict.Add("Medicine",()=> {return Cognitive + Medicine;});
        _skillsDict.Add("CyberSleuth",()=> {return Cognitive + CyberSleuth;});

        _skillsDict.Add("Focus",()=> {return Volitional + Focus;});
        _skillsDict.Add("Tactics",()=> {return Volitional + Tactics;});
        _skillsDict.Add("GreySight",()=> {return Volitional + GreySight;});
        _skillsDict.Add("CaseChaser",()=> {return Volitional + CaseChaser;});

        _skillsDict.Add("Causality",()=> {return Narrative + Causality;});
        _skillsDict.Add("Reasoning",()=> {return Narrative + Reasoning;});
        _skillsDict.Add("Occult",()=> {return Narrative + Occult;});
        _skillsDict.Add("AceInspector",()=> {return Narrative + AceInspector;});

        _skillsDict.Add("Emotion",()=> {return Social + Emotion;});
        _skillsDict.Add("Confidence",()=> {return Social + Confidence;});
        _skillsDict.Add("Wisdom",()=> {return Social + Wisdom;});
        _skillsDict.Add("StreetSavant",()=> {return Social + StreetSavant;});
    }

    public int GetFieldValue(string fieldName)
    {
        var propType = this.GetType();
        var propField = propType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);

        int propValue = Int32.Parse(propField.GetValue(this).ToString());
        Debug.Log($"Player ---- Getting field value name {fieldName} | value: {propValue}");
        return propValue;
    }

    public int GetSkillValue(string skillName)
    {
        if(_skillsDict.ContainsKey(skillName)) return _skillsDict[skillName]();
        else
        {
            Debug.LogError($"{skillName} not found. Please check for typos or that skill exists.");
            return 0;
        }
    }

    public void OutputData()
    {
        // Debug.Log($"Physical {Physical}");
        // Debug.Log($"Cognitive {Cognitive}");
        // Debug.Log($"Volitional {Volitional}");
        // Debug.Log($"Narrative {Narrative}");
        // Debug.Log($"Social {Social}");
    }
}
