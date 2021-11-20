using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using globalDataTypes;

public class gameController : MonoBehaviour
{
    private static gameController _instance;
    public static gameController Instance { get { return _instance; } }
    
    private GAMESTATE _GAMESTATE;
    public event Action<GAMESTATE> OnGameStateChanged;

    [HideInInspector] public playerManager player;
    [HideInInspector] public UIManager UIManager;
    [HideInInspector] public DialogueParser DialogueParser;
    [HideInInspector] public CYOA_EventManager EventManager;
    [HideInInspector] public checkManager CheckManager;
    [HideInInspector] public WorldNavigator worldNavigator;



    
    void Awake()
    {
        //init singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        
        // init script components
        // player = GetComponent<playerManager>();
        player = FindObjectOfType (typeof (playerManager)) as playerManager;
        UIManager = GetComponent<UIManager>();
        DialogueParser = GetComponent<DialogueParser>();
        EventManager = GetComponent<CYOA_EventManager>();
        CheckManager = GetComponent<checkManager>();
        worldNavigator = GetComponent<WorldNavigator>();
    }

    void Start()
    {
        RegisterEventListeners();
        
        // update ui
        UI_updatePlayerName();
        UI_updatePlayerHealth();
        UI_updatePlayerEnergy();
        UI_updatePlayerGold();
        
        // UIManager.initGameStartButton(); // disable start button for now and just load game
        StartGame();
        
    }

    public void StartGame()
    {
        // test player saving
        //player.LoadPlayer();
        if(player._player.CurrentScene == null)
        {
            Debug.Log("GAME CONTROLLER ---- Current scene is not saved to player starting fresh.");
             Player.Instance.CurrentScene = "testLevel";
            // player._player.CurrentScene = "testLevel";
            Player.Instance.CurrentAreaName = "detectiveHQ";
            SubSceneManager.AddScene("testLevel");
        }
        else
        {
            Debug.Log("GAME CONTROLLER ---- Loading from previously saved scene.");
            SubSceneManager.AddScene(player._player.CurrentScene);
            Player.Instance.CurrentAreaName = "detectiveHQ";
        }
    }

    void RegisterEventListeners()
    {
        // register listeners
        UIManager.onGameStartSelected += StartGame;
        UIManager.onDialogueEnded += BeginWorldNavigation;
        WorldNavigator.OnActiveNavObjectLoaded += ProcessNewScene;
        WorldNavigator.OnNavInteractableLoaded += ProcessInteractiveObject;
        // SubSceneManager.OnSceneLoaded += ProcessNewScene;
        SubSceneManager.OnSceneUnloaded += LoadNewScene;
        // DialogueParser.onDialogueReachedDeadEnd += ResetDialogueRoute;
    }

    public void SwitchToLevel(string newSceneName)
    {
        string currentLevelName = SubSceneManager.GetCurrentSceneName();

        Debug.Log($"GAME CONTROLLER ---- Switching from current level {currentLevelName} to new level {newSceneName}.");
        SubSceneManager.RemoveScene(currentLevelName, newSceneName);
        
    }

    public void LoadNewScene(string unloadedSceneName, string newSceneName)
    {
        player._player.CurrentScene = newSceneName;
        SubSceneManager.AddScene(newSceneName);
    }

    void ProcessNewScene(string sceneName)
    {
        Debug.Log("GAME CONTROLLER ---- Game manager processing new scene.");
        // check for starting dialogue, otherwise display nav object
        var dialogue = worldNavigator.GetActiveDialogue();
        if(dialogue != null)
        {
            SetGAMESTATE(GAMESTATE.DIALOGUE);
            DialogueParser.SetupNewDialogue(dialogue);
            DialogueParser.InitDialogue();
        }
        else {
            SetGAMESTATE(GAMESTATE.WORLDNAVIGATION);
            DialogueParser.DisableDialogueParser();
            worldNavigator.DisplayActiveNavObject();
        }
    }

    void ProcessInteractiveObject(DialogueContainer dialogue)
    {
        Debug.Log("GAME CONTROLLER ---- Game manager processing new dialogue from interactive object.");
        // check for starting dialogue, otherwise display nav object
        if(dialogue != null)
        {
            SetGAMESTATE(GAMESTATE.DIALOGUE);
            DialogueParser.SetupNewDialogue(dialogue);
            DialogueParser.InitDialogue();
        }
        else {
            Debug.Log("GAME CONTROLLER ---- NO DIALOGUE FOUND ON THIS INTERACTIVE OBJECT");
            SetGAMESTATE(GAMESTATE.WORLDNAVIGATION);
        }
    }

    void CleanupOldScene(string sceneName)
    {
        Debug.Log("GAME CONTROLLER ---- Game manager cleaning up old scene.");
    }

   

    

    public void UI_updatePlayerStats(){
        UI_updatePlayerName();
        UI_updatePlayerHealth();
        UI_updatePlayerEnergy();
        UI_updatePlayerGold();
    }

    public void UI_updatePlayerName(){
        UIManager.updatePlayerNameText(player.stats.playerName);
    }

    public void UI_updatePlayerHealth(){
        string currentHealth = player.stats.currentHealth.ToString();
        string maxHealth = player.stats.maxHealth.ToString();

        UIManager.updatePlayerHealthText(currentHealth + "/" + maxHealth);
    }
    public void UI_updatePlayerEnergy(){
        string currentEnergy = player.stats.currentEnergy.ToString();
        string maxEnergy = player.stats.maxEnergy.ToString();

        UIManager.updatePlayerEnergyText(currentEnergy + "/" + maxEnergy);
    }
    public void UI_updatePlayerGold(){
        UIManager.updatePlayerGoldText(player.stats.currentGold.ToString());
    }

    // Event Listeners
    public void BeginWorldNavigation()
    {
        Debug.Log("GAME CONTROLLER ---- End of dialogue graph reached. Moving to active World Nav Object.");
        SetGAMESTATE(GAMESTATE.WORLDNAVIGATION);
        DialogueParser.DisableDialogueParser();
        worldNavigator.DisplayActiveNavObject();
    }


    public GAMESTATE GetGAMESTATE()
    {
        return _GAMESTATE;
    }

    private void SetGAMESTATE(GAMESTATE newState)
    {
        _GAMESTATE = newState;
        OnGameStateChanged?.Invoke(_GAMESTATE);
    }
}
