using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Dictionary<string, int> GetSkill = new Dictionary<string, int>();

    // level data
    [Header("Level Data")]
    public string CurrentScene;
    public string CurrentAreaName;
    public List<string> visitedWorldNavObjects = new List<string>();
    public List<string> visitedInteractableObjects = new List<string>();

    

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

    public void addWorldNavObject(string GUID)
    {
        visitedWorldNavObjects.Add(GUID);
    }

    public void addInteractableObject(string GUID)
    {
        visitedInteractableObjects.Add(GUID);
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
        GetSkill.Add("Athletics",Physical + Athletics);
        GetSkill.Add("Fortitude",Physical + Fortitude);
        GetSkill.Add("Stealth",Physical + Stealth);
        GetSkill.Add("HardboiledHotshot",Physical + HardboiledHotshot);

        GetSkill.Add("Ballistics",Cognitive + Ballistics);
        GetSkill.Add("Forensics",Cognitive + Forensics);
        GetSkill.Add("Medicine",Cognitive + Medicine);
        GetSkill.Add("CyberSleuth",Cognitive + CyberSleuth);

        GetSkill.Add("Focus",Volitional + Focus);
        GetSkill.Add("Tactics",Volitional + Tactics);
        GetSkill.Add("GreySight",Volitional + GreySight);
        GetSkill.Add("CaseChaser",Volitional + CaseChaser);

        GetSkill.Add("Causality",Narrative + Causality);
        GetSkill.Add("Reasoning",Narrative + Reasoning);
        GetSkill.Add("Occult",Narrative + Occult);
        GetSkill.Add("AceInspector",Narrative + AceInspector);

        GetSkill.Add("Emotion",Social + Emotion);
        GetSkill.Add("Confidence",Social + Confidence);
        GetSkill.Add("Wisdom",Social + Wisdom);
        GetSkill.Add("StreetSavant",Social + StreetSavant);
    }

    public int GetSkillValue(string skillName)
    {
        if(GetSkill.ContainsKey(skillName))
        {
            return GetSkill[skillName];
        }
        else
        {
            Debug.Log($"{skillName} not found. Please check for typos or that skill exists.");
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
