using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private gameController controller;

    // UI elements
    
    [Header("Content")]
    public GameObject contentScrollContainer;
    public Scrollbar vertScrollBar;
    public GameObject paragraphPrefab;
    [Header("Player Stats")]
    public GameObject playerNameText;
    public GameObject playerHealthText;
    public GameObject playerEnergyText;
    public GameObject playerGoldText;
    [Header("Player Action Buttons")]
    public GameObject playerActionOptionBtnPrefab;
    public ToggleGroup toggleGroup;
    public GameObject togglePrefab;
    


    private GameObject UI_paragraph;
    private List<GameObject> actionOptionButtons = new List<GameObject>();
    private List<GameObject> actionToggleButtons = new List<GameObject>();
    private GameObject confirmActionButton;
    private GameObject lastSelectedButton = null;
    private int currentActionOption;

    void Awake() {
        // set parent controller
        controller = GetComponent<gameController>();
        // set initials content paragraphs
        UI_paragraph = GameObject.Instantiate(paragraphPrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        initActionOptionButtons();
        initConfirmActionButton();
    }

    void Start(){
        
        toggleGroup.SetAllTogglesOff();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initActionOptionButtons(){
        for (int i = 0; i < globalConfig.UI.MAX_ACTION_OPTIONS; i++)
        {
            int localIndex = i; 
            //GameObject optionButton = GameObject.Instantiate(togglePrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
            // setup each button base on config limit
            
           // optionButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { setCurrentActionOption(localIndex); });
           // optionButton.SetActive(false);

           // actionOptionButtons.Add(optionButton);
          //  Debug.Log("button list size: " + actionOptionButtons.Count);


            // just add toggles
            GameObject toggle = GameObject.Instantiate(togglePrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
            Toggle toggleComponent = toggle.GetComponent<Toggle>();
            
            
            toggleComponent.group = toggleGroup;
            toggleComponent.onValueChanged.AddListener(delegate { setCurrentToggleOption(toggleComponent, localIndex);});
            toggle.name = "toggleButton_" + localIndex;
            actionToggleButtons.Add(toggle);
            toggle.SetActive(false); 
        }    
    }

    public void initConfirmActionButton(){
        confirmActionButton = GameObject.Instantiate(playerActionOptionBtnPrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        confirmActionButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { processPlayerAction(currentActionOption); });
        confirmActionButton.GetComponentInChildren<Text>().text = "Select option then confirm.";
    }

    public void setCurrentToggleOption(Toggle toggle, int index) {
        //Debug.Log("current toggle" + toggle.isOn);
        //Debug.Log("toggle index" + index);
        if(toggle.isOn) {
            currentActionOption = index;
        }
        else {
            currentActionOption = -1;
        }
    }

    public void setCurrentActionOption(int i){
        if(lastSelectedButton != null) {
           // lastSelectedButton.GetComponentInChildren<Button>().
        }
       currentActionOption = i;
       confirmActionButton.GetComponentInChildren<Text>().text = "Confirm";
       //Debug.Log("Current action option is: " + currentActionOption);
    }

    public void processPlayerAction(int buttonNumber){
        Debug.Log("checking for action option: " + currentActionOption + "at time " + Time.deltaTime);
        if(currentActionOption >= 0 ) {
            controller.roomNavigator.changeRoom(currentActionOption);
            confirmActionButton.GetComponentInChildren<Text>().text = "Select option then confirm.";
            currentActionOption = -1;
            //Debug.Log("set to -1 at time " + Time.deltaTime);
            checkForSingleOption();
        }
        else {
            //Debug.Log("curren action value is less than zero at time " + Time.deltaTime);
        }
    }

    public void updateContentText(string content) {
        Debug.Log("updating content text..");
        resetScroll();

        UI_paragraph.GetComponent<TextMeshProUGUI>().text = content;

    }

    public void updatePlayerNameText(string name) {
        playerNameText.GetComponent<TextMeshProUGUI>().text = name;
    }
    public void updatePlayerHealthText(string content) {
        playerHealthText.GetComponent<TextMeshProUGUI>().text = content;
    }
    public void updatePlayerEnergyText(string content) {
        playerEnergyText.GetComponent<TextMeshProUGUI>().text = content;
    }
    public void updatePlayerGoldText(string content) {
        Debug.Log("UI gold: " + content);
        playerGoldText.GetComponent<TextMeshProUGUI>().text = content;
    }


    public void resetScroll(){
        vertScrollBar.value = 1;
    }

    public void resetButtons(){

        //toggleGroup.SetAllTogglesOff();
        for (int i = 0; i < actionToggleButtons.Count; i++)
        {
            actionToggleButtons[i].SetActive(true);      
            actionToggleButtons[i].GetComponent<Toggle>().isOn = false;      
            actionToggleButtons[i].SetActive(false);      
        }  
    }

    public void updateActionOptionsButtons(Exit[] actionOptions){

        resetButtons();
       // Debug.Log("button list size when called: " + actionOptionButtons.Count);
        
        //Debug.Log("action option size: " + actionOptions.Length);
        for (int i = 0; i < actionOptions.Length; i++) {
         //   Debug.Log("current action:" + actionOptions[i].buttonText.ToString());
            actionToggleButtons[i].SetActive(true);
            actionToggleButtons[i].GetComponentInChildren<Text>().text = actionOptions[i].buttonText;
           // actionOptionButtons[i].SetActive(true);
           // actionOptionButtons[i].GetComponentInChildren<Text>().text = actionOptions[i].buttonText;
        }

    //    // if single option, hide buttons and setup action button
    //     if(actionOptions.Length == 1) {
    //         Debug.Log("length is 1");
    //        // setCurrentActionOption(0);
    //         Debug.Log("current action option: " + currentActionOption);
    //         confirmActionButton.GetComponentInChildren<Text>().text = actionOptions[0].buttonText;
    //         actionToggleButtons[0].SetActive(false);
    //         setCurrentActionOption(0);
    //        // actionToggleButtons[0].GetComponent<Toggle>().isOn = true;
    //     }
    }

    public void checkForSingleOption(){
        //Debug.Log("checking for single action");
        if(!actionToggleButtons[1].activeSelf) {
            setCurrentActionOption(0);
            actionToggleButtons[0].SetActive(false);
            confirmActionButton.GetComponentInChildren<Text>().text = actionToggleButtons[0].GetComponentInChildren<Text>().text;
            //Debug.Log("single action found");
        }
    }
}
