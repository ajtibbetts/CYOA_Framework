using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
static public class globalConfig
{
    static public class UI {
        static public readonly int MAX_ACTION_OPTIONS = 10; 
        static public readonly int MAX_MENU_SWIPE_POS_X = 128; // in pixels;
    }

    static public class Player {
        static public readonly int PLAYER_STARTING_HEALTH = 10;
        static public readonly string PLAYER_STARTING_NAME = "Player";
    }
}
