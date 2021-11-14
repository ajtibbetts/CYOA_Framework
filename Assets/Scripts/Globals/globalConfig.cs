using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
static public class globalConfig
{
    static public class UI {
        static public readonly int MAX_MENU_SWIPE_POS_X = 128; // in pixels; if less than, menu will open
        static public readonly int MIN_CASEMENU_SWIPE_POS_X = 952; // in pixels; if greater, menu will open

        static public Color toggleInactiveBackgroundColor = new Color(0.1019608f,0.7176471f,0.8117647f); // cyan
        static public Color toggleSelectedBackgroundColor = new Color(0.8117647f,0.5098039f,0.1019607f); // orange
    
        static public class CaseUI 
        {
            static public Color profileAllyBackgroundColor = new Color(0.7989943f,0.990566f,0.8437973f); // green
            static public Color profileNeutralBackgroundColor = new Color(0.8f,0.9021274f,0.9921569f); // cyan
            static public Color profileSuspectBackgroundColor = new Color(1f,0.7971698f,0.8059194f); // red


            
        }
    }

    static public class Player {
        static public readonly int PLAYER_STARTING_HEALTH = 10;
        static public readonly string PLAYER_STARTING_NAME = "Player";
    }
}
