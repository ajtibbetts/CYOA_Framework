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
    public int Athletics;
    public int Fortitude;
    public int Stealth;
    public int HardBoiled;
    //cognitive
    public int Ballistics;
    public int Forensics;
    public int Medicine;
    public int TechnoPath;
    //volitional
    public int Focus;
    public int Tactics;
    public int GreySight;
    public int CaseChaser;
    //narrative
    public int Causality;
    public int Reasoning;
    public int Occult;
    public int GreatDetective;
    // social
    public int Drama;
    public int Confidence;
    public int Wisdom;
    public int StreetSmart;

    // level data
    [Header("Level Data")]
    public string CurrentScene;
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

        Physical = 5;
        Cognitive = 5;
        Volitional = 5;
        Narrative = 5;
        Social = 5;
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
