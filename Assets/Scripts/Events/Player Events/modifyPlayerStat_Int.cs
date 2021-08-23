using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CYOA/Events/Change Player Int Property")]
public class modifyPlayerStat_Int : CYOA_Event {
      
    public override String activateEvent(gameController controller, eventParams parameters) {
        
        

        if(parameters.propertyName != null) {
        // get player stat property and current value
            PropertyInfo propertyInfo = controller.player.stats.GetType().GetProperty(parameters.propertyName);
            int currentValue = (int)propertyInfo.GetValue(controller.player.stats, null);
            Debug.Log("Property value of " + parameters.stringParam + ": " + propertyInfo.GetValue(controller.player.stats, null));
            
            // set value and return UI message.
            // propertyInfo.SetValue(controller.player.stats, Convert.ChangeType(value, propertyInfo.PropertyType), null);
            propertyInfo.SetValue(controller.player.stats, currentValue + parameters.propertyValue, null);

                if(parameters.propertyValue > 0) {
                    controller.UI_updatePlayerStats();
                    return $"<color=\"green\">{parameters.stringParam} increased by " + parameters.propertyValue + "</color>";
                }
                else {
                    controller.UI_updatePlayerStats();
                    return $"<color=\"red\">{parameters.stringParam} decreased by " + parameters.propertyValue + "</color>";
                }
                
        }

        Debug.Log("Property Name parameter is missing.  Cannot change player property.");
        
        return null;
        
        
    }
    
}
