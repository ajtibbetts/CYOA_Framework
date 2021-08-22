using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CYOA/changename")]
public class modifyPlayerName: CYOA_Event {
      
    public override string activateEvent(gameController controller, eventParams parameters) {
        
        Debug.Log("modifying player name!");
        controller.player.playerName = parameters.stringParam;
        controller.UI_updatePlayerName();

        return "<color=\"yellow\">Name changed to " + parameters.stringParam + "</color>";
    }
}
