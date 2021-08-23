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
    }

    void Start()
    {
        UI_updatePlayerName();
        UI_updatePlayerHealth();
        UI_updatePlayerEnergy();
        UI_updatePlayerGold();
        DisplayRoomText();
    }

    public void DisplayLoggedText()
    {
        string actionLogText = string.Join("\n", actionLog.ToArray());
        
        // update UI with current room text
        UIManager.updateContentText(actionLogText + roomNavigator.currentRoom.description);
        // update UI action options buttons
        UIManager.updateActionOptionsButtons(roomNavigator.currentRoom.exits);
        // Debug.Log(roomNavigator.currentRoom.exits[0].buttonText.ToString());

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
        DisplayLoggedText();
    }

    private void UnpackRoom() {
        roomNavigator.unpackExitsInRoom();
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
        Debug.Log("gold: " + player.stats.currentGold);
        Debug.Log("gold: " + player.stats.currentGold.ToString());
        UIManager.updatePlayerGoldText(player.stats.currentGold.ToString());
    }
}
