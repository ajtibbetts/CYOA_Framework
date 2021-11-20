using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using globalDataTypes;

public class playerStatsManager : MonoBehaviour
{
    
    public playerStatBuild playerStatsTemplate;

    // Properties and referenced stats
    public string playerName {
        get { return playerStatsTemplate.playerName; }
        set { playerStatsTemplate.playerName = value; }
    }

    public string playerClass {
        get { return playerStatsTemplate.playerClass; }
        set { playerStatsTemplate.playerClass = value; }
    }

    public int experienceLevel {
        get { return playerStatsTemplate.experienceLevel; }
        set { playerStatsTemplate.experienceLevel = value; }
    }
    public int experienceTotal {
        get { return playerStatsTemplate.experienceTotal; }
        set { playerStatsTemplate.experienceTotal = value; }
    }

    public int currentHealth {
        get { return playerStatsTemplate.currentHealth; }
        set { playerStatsTemplate.currentHealth = value; }
    }
    public int maxHealth {
        get { return playerStatsTemplate.maxHealth; }
        set { playerStatsTemplate.maxHealth = value; }
    }
    public int currentEnergy {
        get { return playerStatsTemplate.currentEnergy; }
        set { playerStatsTemplate.currentEnergy = value; }
    }
    public int maxEnergy {
        get { return playerStatsTemplate.maxEnergy; }
        set { playerStatsTemplate.maxEnergy = value; }
    }
    public int currentGold {
        get { return playerStatsTemplate.currentGold; }
        set { playerStatsTemplate.currentGold = value; }
    }
    public int braveryLevel {
        get { return playerStatsTemplate.braveryLevel; }
        set { playerStatsTemplate.braveryLevel = value; }
    }
    public int alacrityLevel {
        get { return playerStatsTemplate.alacrityLevel; }
        set { playerStatsTemplate.alacrityLevel = value; }
    }
    public int tenacityLevel {
        get { return playerStatsTemplate.tenacityLevel; }
        set { playerStatsTemplate.tenacityLevel = value; }
    }
    public int brillianceLevel {
        get { return playerStatsTemplate.brillianceLevel; }
        set { playerStatsTemplate.brillianceLevel = value; }
    }
    public int intuitionLevel {
        get { return playerStatsTemplate.intuitionLevel; }
        set { playerStatsTemplate.intuitionLevel = value; }
    }
    public int confidenceLevel {
        get { return playerStatsTemplate.confidenceLevel; }
        set { playerStatsTemplate.confidenceLevel = value; }
    }
    public StatAbility[] playerAbilities {
        get { return playerStatsTemplate.playerAbilities; }
        set { playerStatsTemplate.playerAbilities = value; }
    }

    

    private static playerStatsManager stats;
    public static playerStatsManager instance
    {
        get
        {
            if (!stats)
            {
                stats = FindObjectOfType (typeof (playerStatsManager)) as playerStatsManager;

                if (!stats)
                {
                    Debug.LogError ("There needs to be one active playerStats script on a GameObject in your scene.");
                }
                else
                {
                    stats.Init (); 
                }
            }

            return stats;
        }
    }


    void Init ()
    {
        // if (eventDictionary == null)
        // {
        //     eventDictionary = new Dictionary<string, UnityEvent>();
        // }
    }
    


}
