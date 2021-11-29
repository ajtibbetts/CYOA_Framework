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

    [HideInInspector] public Player _player;
    [SerializeField] private PlayerProgressTracker _progressTracker;
    [HideInInspector] public UIManager UIManager;
    [HideInInspector] public DialogueParser DialogueParser;
    [HideInInspector] public CYOA_EventManager EventManager;
    [HideInInspector] public checkRollManager CheckRollManager;
    [HideInInspector] public WorldNavigator worldNavigator;


    private string _optionalTargetNavObjectGUID = null;
    
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
        // player = FindObjectOfType (typeof (Player)) as Player;
        _player = Player.Instance;
        _progressTracker = PlayerProgressTracker.Instance;
        UIManager = GetComponent<UIManager>();
        DialogueParser = GetComponent<DialogueParser>();
        EventManager = GetComponent<CYOA_EventManager>();
        CheckRollManager = GetComponent<checkRollManager>();
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
        if(_progressTracker.CurrentScene == null)
        {
            Debug.Log("GAME CONTROLLER ---- Current scene is not saved to player starting fresh.");
             _progressTracker.CurrentScene = "testLevel";
            // player._player.CurrentScene = "testLevel";
            _progressTracker.CurrentAreaName = "detectiveHQ";
            SubSceneManager.AddScene("testLevel");
        }
        else
        {
            Debug.Log("GAME CONTROLLER ---- Loading from previously saved scene.");
            SubSceneManager.AddScene(_progressTracker.CurrentScene);
            _progressTracker.CurrentAreaName = "detectiveHQ";
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

    public GAMESTATE GetGameState()
    {
        return _GAMESTATE;
    }

    public void SwitchToLevel(string newSceneName)
    {
        // first check if we need to get previous scene
        if(newSceneName == "previousScene") 
        {
            newSceneName = _progressTracker.PreviousScene;
            _optionalTargetNavObjectGUID = _progressTracker.PreviousNavObjectGUID;
        }
        
        // cache current valuest to previous before swapping.
        string currentLevelName = SubSceneManager.GetCurrentSceneName();
        string currentNavObjectGUID = WorldNavigator.Instance.GetActiveNavObject().GUID;
        _progressTracker.PreviousScene = currentLevelName;
        _progressTracker.PreviousNavObjectGUID = currentNavObjectGUID;
        _progressTracker.PreviousAreaName = _progressTracker.CurrentAreaName;
       
        

        Debug.Log($"GAME CONTROLLER ---- Switching from current level {currentLevelName} to new level {newSceneName}.");
        SubSceneManager.RemoveScene(currentLevelName, newSceneName);
        
    }

    public void LoadNewScene(string unloadedSceneName, string newSceneName)
    {
        _progressTracker.CurrentScene = newSceneName;
        SubSceneManager.AddScene(newSceneName);
    }

    void ProcessNewScene(string sceneName)
    {
        // first check for any previous target nav object nodes to hit.
        if(_optionalTargetNavObjectGUID != null)
        {
            Debug.Log("GAME CONTROLLER ---- Optional target nav found -- skipping to: " + _optionalTargetNavObjectGUID);
            worldNavigator.SkipToNavObjectByGUID(_optionalTargetNavObjectGUID);
            _optionalTargetNavObjectGUID = null;
        }
        
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
        // UIManager.updatePlayerNameText(_player.PlayerFirstName);
    }

    public void UI_updatePlayerHealth(){
        // string currentHealth = player.stats.currentHealth.ToString();
        // string maxHealth = player.stats.maxHealth.ToString();

        // UIManager.updatePlayerHealthText(currentHealth + "/" + maxHealth);
    }
    public void UI_updatePlayerEnergy(){
        // string currentEnergy = player.stats.currentEnergy.ToString();
        // string maxEnergy = player.stats.maxEnergy.ToString();

        // UIManager.updatePlayerEnergyText(currentEnergy + "/" + maxEnergy);
    }
    public void UI_updatePlayerGold(){
        // UIManager.updatePlayerGoldText(player.stats.currentGold.ToString());
    }

    // Event Listeners
    public void BeginWorldNavigation()
    {
        Debug.Log("GAME CONTROLLER ---- End of dialogue graph reached. Moving to active World Nav Object.");
        // first will want to check for returning dialogue on the nav object, can just process new scene
        // ProcessNewScene(null);

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
