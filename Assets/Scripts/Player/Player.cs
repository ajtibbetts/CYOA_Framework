using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    // base stats
    public int Physical {get; set;}
    public int Cognitive {get; set;}
    public int Volitional {get; set;}
    public int Narrative {get; set;}
    public int Social {get; set;}

    // level data
    public string CurrentScene;

    // skills
    private PlayerSkills _skills;

    // derived stats
    

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
