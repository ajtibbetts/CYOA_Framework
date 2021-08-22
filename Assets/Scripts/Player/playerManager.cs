using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerManager : MonoBehaviour {
    
    [HideInInspector] public playerStatsManager stats;

    [SerializeField]
    private int playerHealth = globalConfig.Player.PLAYER_STARTING_HEALTH;
    [SerializeField]
    private int playerMaxHealth = globalConfig.Player.PLAYER_STARTING_HEALTH;

    public string playerName = globalConfig.Player.PLAYER_STARTING_NAME;

    private void Awake() {
        stats = GetComponent<playerStatsManager>();
        
    }
    
 
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
