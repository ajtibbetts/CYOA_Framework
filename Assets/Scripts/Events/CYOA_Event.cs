using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CYOA_Event : ScriptableObject {

    public string eventName;


    public virtual string activateEvent(gameController controller, eventParams parameters) {
        
        return "This event is called";
    }
}
