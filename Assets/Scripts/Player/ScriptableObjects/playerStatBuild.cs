using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CYOA/Player/Stats")]
public class playerStatBuild : ScriptableObject
{
    [Header("General")]
    public string playerName;
    public string playerClass;
    public int experienceLevel;
    public int experienceTotal;
    [Header("Resources")]
    public int currentHealth;
    public int maxHealth;
    public int currentEnergy;
    public int maxEnergy;
    public int currentGold;
    [Header("Attributes")]
    public int braveryLevel;
    public int alacrityLevel;
    public int tenacityLevel;
    public int brillianceLevel;
    public int intuitionLevel; // 3
    public int confidenceLevel; // 4
    [Header("Abilities")]
    public StatAbility[] playerAbilities;

    



}
