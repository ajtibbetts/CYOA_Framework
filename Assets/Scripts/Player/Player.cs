using System;
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
    public string PlayerGender;


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
    public int Athletics = 0;
    public int Fortitude = 0;
    public int Stealth = 0;
    public int HardboiledHotshot = 0;
    //cognitive
    public int Ballistics = 0;
    public int Forensics = 0;
    public int Medicine = 0;
    public int CyberSleuth = 0;
    //volitional
    public int Focus = 0;
    public int Tactics = 0;
    public int GreySight = 0;
    public int CaseChaser = 0;
    //narrative
    public int Causality = 0;
    public int Reasoning = 0;
    public int Occult = 0;
    public int AceInspector = 0;
    // social
    public int Emotion = 0;
    public int Confidence = 0;
    public int Wisdom = 0;
    public int StreetSavant = 0;

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
        PlayerGender = "female";
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
