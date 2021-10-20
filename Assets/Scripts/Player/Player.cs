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
    private PlayerSkills _skills;

    // level data
    [Header("Level Data")]
    public string CurrentScene;
    public List<string> visitedWorldNavObjects = new List<string>();

    

    // derived stats

    public void addWorldNavObject(string GUID)
    {
        visitedWorldNavObjects.Add(GUID);
    }
    

    public void SetTestData()
    {
        // Debug.Log($"Setting test data!");
        Physical = 6;
        Cognitive = 7;
        Volitional = 5;
        Narrative = 15;
        Social = 4;
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
