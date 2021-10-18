using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameController : MonoBehaviour
{
    
    //private static int TEXTLIMIT = 500;
    public TextMeshProUGUI displayTextTMP;
    private GameObject UI_paragraph;
    
    [HideInInspector] public playerManager player;
    [HideInInspector] public roomNavigation roomNavigator;
    [HideInInspector] public UIManager UIManager;
    [HideInInspector] public DialogueParser DialogueParser;
    [HideInInspector] public CYOA_EventManager EventManager;
    [HideInInspector] public checkManager CheckManager;
    [HideInInspector] public WorldNavigator worldNavigator;
    
    
    [HideInInspector] public List<string> interactionDescriptionsInRoom = new List<string>();
    [HideInInspector] public List<string> buttonChoicesTexts = new List<string>();
    List<string> actionLog = new List<string>();
    
    void Awake()
    {
        // init script components
        // player = GetComponent<playerManager>();
        player = FindObjectOfType (typeof (playerManager)) as playerManager;
        roomNavigator = GetComponent<roomNavigation>();
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
        
        UIManager.initGameStartButton();
        
    }

    public void StartGame()
    {
        // test player saving
        //player.LoadPlayer();
        if(player._player.CurrentScene == null)
        {
            Debug.Log("GAME CONTROLLER ---- Current scene is not saved to player starting fresh.");
            player._player.CurrentScene = "testLevel";
            SubSceneManager.AddScene("testLevel");
        }
        else
        {
            Debug.Log("GAME CONTROLLER ---- Loading from previously saved scene.");
            SubSceneManager.AddScene(player._player.CurrentScene);
        }
    }

    void RegisterEventListeners()
    {
        // register listeners
        UIManager.onGameStartSelected += StartGame;
        UIManager.onDialogueEnded += BeginWorldNavigation;
        WorldNavigator.OnActiveNavObjectLoaded += ProcessNewScene;
        // SubSceneManager.OnSceneLoaded += ProcessNewScene;
        // SubSceneManager.OnSceneUnloaded += CleanupOldScene;
        // DialogueParser.onDialogueReachedDeadEnd += ResetDialogueRoute;
    }

    void ProcessNewScene(string sceneName)
    {
        Debug.Log("GAME CONTROLLER ---- Game manager processing new scene.");
        // check for starting dialogue, otherwise display nav object
        var dialogue = worldNavigator.GetActiveDialogue();
        if(dialogue != null)
        {
            DialogueParser.SetupNewDialogue(dialogue);
            DialogueParser.InitDialogue();
        }
        else {
            DialogueParser.DisableDialogueParser();
            worldNavigator.DisplayActiveNavObject();
        }
    }

    void CleanupOldScene(string sceneName)
    {
        Debug.Log("GAME CONTROLLER ---- Game manager cleaning up old scene.");
    }

    public void DisplayLoggedText()
    {
        string actionLogText = string.Join("\n", actionLog.ToArray());
        
        // check for page image and activate
        // if(roomNavigator.hasPageImage()) {
        //     UIManager.updatePageImage(roomNavigator.currentRoom.pageImage);
        // }
        // else {
        //     UIManager.hidePageImage();
        // }

        // update UI with current room text
        //UIManager.updateContentText(actionLogText + roomNavigator.currentRoom.description);
        // update UI action options buttons
        //UIManager.updateActionOptionsButtons(roomNavigator.currentRoom.playerActionOptions);
        // Debug.Log(roomNavigator.currentRoom.exits[0].buttonText.ToString());
        UI_updatePlayerStats();
        actionLog.Clear();
       // Debug.Log("logged text displayed..");
    
    }

    public void DisplayRoomText() 
    {
        //Debug.Log("Dispalying room text..");
        clearCollectionsForNewRoom();
        UnpackRoom();

                

        // string joinedInteractionDescriptions = string.Join("\n",interactionDescriptionsInRoom.ToArray());

        // string combinedText = roomNavigator.currentRoom.description + "\n" + joinedInteractionDescriptions;
        //LogStringWithReturn(combinedText);
        
        DisplayLoggedText(); // temp remove to test graph
        
    }

    private void UnpackRoom() {
        roomNavigator.triggerPageEvents();
        roomNavigator.unpackPlayerActionOptionsInRoom();
    }

    private void clearCollectionsForNewRoom(){
        
        interactionDescriptionsInRoom.Clear();
        buttonChoicesTexts.Clear();   
    }

    public void LogStringWithReturn(string stringToAdd)
    {
        actionLog.Add(stringToAdd + "\n");
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
        DialogueParser.DisableDialogueParser();
        worldNavigator.DisplayActiveNavObject();
    }

    public void InitializePlayer()
    {

    }
}
