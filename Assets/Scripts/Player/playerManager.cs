using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerManager : MonoBehaviour {
    
    [HideInInspector] public playerStatsManager stats;
    [SerializeField] public Player _player;

    [SerializeField]
    private int playerHealth = globalConfig.Player.PLAYER_STARTING_HEALTH;
    [SerializeField]
    private int playerMaxHealth = globalConfig.Player.PLAYER_STARTING_HEALTH;

    public string playerName = globalConfig.Player.PLAYER_STARTING_NAME;

    public string CurrentScene;

    private void Awake() {
        stats = GetComponent<playerStatsManager>();
        _player = GetComponent<Player>();
        Debug.Log("Starting fresh player.");
        _player.OutputData();
        _player.SetTestData();
        _player.OutputData();
        
    }
    

    // public void SavePlayer()
    // {
        
    //     Debug.Log("Saving player.");
    //     _player.OutputData();
    //     SaveSystem.SavePlayer(_player);
    // }

    // public void LoadPlayer()
    // {
    //     Debug.Log("Loading player.");
    //     PlayerData data = SaveSystem.LoadPlayer();
    //     _player.Physical = data.Physical;
    //     _player.Cognitive = data.Cognitive;
    //     _player.Volitional = data.Volitional;
    //     _player.Narrative = data.Narrative;
    //     _player.Social = data.Social;

    //     // level data
    //     _player.CurrentScene = data.CurrentScene;

    //     _player.OutputData();
    // }
 
    public void modifyPlayerHealth (int amount) {
        stats.currentHealth += amount;
    }

    public void modifyPlayerMaxHealth (int amount) {
        stats.maxHealth += amount;
    }

    public int getPlayerHealth(){
        return playerHealth;
    }

    public int getPlayerMaxHealth(){
        return playerMaxHealth;
    }
}
