using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player : MonoBehaviour
{
    // base stats
    [Header("Player Stats")]
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
    public int HardBoiled;
    //cognitive
    public int Ballistics;
    public int Forensics;
    public int TechnoPath;
    //volitional
    public int Command;
    public int GreySight;
    public int CaseChaser;
    //narrative
    public int Causality;
    public int Reasoning;
    public int Mystic;
    // social
    public int Confidence;
    public int Wisdom;
    public int PeopleReader;

    // level data
    [Header("Level Data")]
    public string CurrentScene;
    public List<string> visitedWorldNavObjects = new List<string>();
    public List<string> visitedInteractableObjects = new List<string>();

    

    // derived stats

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
