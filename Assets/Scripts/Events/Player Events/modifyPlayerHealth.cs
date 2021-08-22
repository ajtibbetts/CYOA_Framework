using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CYOA/changehealth")]
public class modifyPlayerHealth : CYOA_Event {
      
    public override String activateEvent(gameController controller, eventParams parameters) {
        
        Debug.Log("modifying player health!");
        controller.player.stats.currentHealth += parameters.intParam;
        controller.UI_updatePlayerHealth();

        if(parameters.intParam > 0) {
            return "<color=\"green\">Health increased by " + parameters.intParam + "</color>";
        }
        else {
            return "<color=\"red\">Health decreased by " + Math.Abs(parameters.intParam) + "</color>";
        }
        
    }
}
