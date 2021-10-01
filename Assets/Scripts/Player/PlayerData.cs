using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public int Physical;
    public int Cognitive;
    public int Volitional;
    public int Narrative;
    public int Social;

    public string CurrentScene;

    public PlayerData (Player player)
    {
        Physical = player.Physical;
        Cognitive = player.Cognitive;
        Volitional = player.Volitional;
        Narrative = player.Narrative;
        Social = player.Social;
        
        // level data
        CurrentScene = player.CurrentScene;
    }
}