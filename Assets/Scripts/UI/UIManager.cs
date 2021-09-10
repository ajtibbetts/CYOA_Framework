using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [HideInInspector] public gameController controller;

    // UI elements
    
    [Header("Content")]
    public GameObject contentScrollContainer;
    public Scrollbar vertScrollBar;
    public GameObject paragraphPrefab;
    public GameObject imagePrefab;
    [Header("Player Stats")]
    public GameObject playerNameText;
    public GameObject playerHealthText;
    public GameObject playerEnergyText;
    public GameObject playerGoldText;
    [Header("Player Action Buttons")]
    public GameObject playerActionOptionBtnPrefab;
    public ToggleGroup toggleGroup;
    public GameObject togglePrefab;

    // internal components
    [HideInInspector] public conditionManager conditionChecker;
    [HideInInspector] public contentManager contentManager;
    


    private GameObject UI_paragraph;
    private GameObject UI_contentImage;
    private List<GameObject> actionOptionButtons = new List<GameObject>();
    private List<GameObject> actionToggleButtons = new List<GameObject>();
    private GameObject confirmActionButton;
    private GameObject lastSelectedButton = null;
    private int currentActionOption;

    void Awake() {
        // add event listener
        SwipeDetector.OnSwipe += onSwipe;

        // set parent controller and child components
        controller = GetComponent<gameController>();
        conditionChecker = gameObject.AddComponent(typeof(conditionManager)) as conditionManager;
        contentManager = gameObject.AddComponent(typeof(contentManager)) as contentManager;
        // set initials content prefabs
        UI_contentImage = GameObject.Instantiate(imagePrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        UI_contentImage.SetActive(false);
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

    public void onSwipe(SwipeData data) {
        Debug.Log("Swipe in Direction: " + data.Direction);
        if(data.Direction == SwipeDirection.Right){
            Debug.Log("Swipe is right. Start X: " + data.StartPosition.x + " End X: " + data.EndPosition.x);
            if(data.EndPosition.x <= globalConfig.UI.MAX_MENU_SWIPE_POS_X){
                Debug.Log("Open menu!");
            }
        }
    }

    public void initActionOptionButtons(){
        for (int i = 0; i < globalConfig.UI.MAX_ACTION_OPTIONS; i++)
        {
            int localIndex = i; 
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
            checkForSingleOption(); // CALLED AT END TO REMOVE EXTRA OPTIONS
        }
        else {
            //Debug.Log("curren action value is less than zero at time " + Time.deltaTime);
        }
    }

    public void updatePageImage(Sprite image){
        UI_contentImage.GetComponent<Image>().sprite = image;
        UI_contentImage.SetActive(true);
    }

    public void hidePageImage(){
        UI_contentImage.SetActive(false);
    }
    public void updateContentText(string content) {
        Debug.Log("updating content text..");
        resetScroll();
        string parsedContent = contentManager.parseContent(content);
        
        // UI_paragraph.GetComponent<TextMeshProUGUI>().text = content;
        UI_paragraph.GetComponent<TextMeshProUGUI>().text = contentManager.parseContent(content);

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

    public void resetButton(int i){
        if(actionToggleButtons[i] != null) {
            actionToggleButtons[i].SetActive(true);      
            actionToggleButtons[i].GetComponent<Toggle>().isOn = false;      
            actionToggleButtons[i].SetActive(false); 
        }
    }

    public void updateActionOptionsButtons(actionOption[] actionOptions){

        resetButtons();
       // Debug.Log("button list size when called: " + actionOptionButtons.Count);
        
        //Debug.Log("action option size: " + actionOptions.Length);
        for (int i = 0; i < actionOptions.Length; i++) {
         //   Debug.Log("current action:" + actionOptions[i].buttonText.ToString());
            actionToggleButtons[i].SetActive(true);
            // check for conditional options on this action, otherwise default text
            if(actionOptions[i].conditionalRequirement.conditionsToMeet.Length > 0){
                Debug.Log($"condition found on option {i}. checking...");
                // get the label based on the first conditions property
                string conditionLabelText;
                // int satisfiedCondition = conditionChecker.checkConditions(actionOptions[i]);
                if(conditionChecker.areConditionsMet(actionOptions[i])){
                    conditionLabelText = conditionChecker.getConditionLabel(actionOptions[i].conditionalRequirement.conditionsToMeet[0].propertyName);
                    Debug.Log($"Condition met on option {i} condition.");
                    actionToggleButtons[i].GetComponentInChildren<Text>().text = "<color=\"green\">[" + conditionLabelText +"]</color> " + actionOptions[i].conditionalRequirement.passButtonText;
                    actionOptions[i].conditionalRequirement.conditionMet = true;
                    actionToggleButtons[i].GetComponentInChildren<Toggle>().interactable = true;
                    
                }
                else {
                    Debug.Log("No conditional requirements met for this action option.");
                    actionOptions[i].conditionalRequirement.conditionMet = false;
                    // if condition is set to be hidden when not met, hide it
                    if(actionOptions[i].conditionalRequirement.hideUnlessMet) {
                        resetButton(i);
                    }
                    else {
                        conditionLabelText = conditionChecker.getConditionLabel(actionOptions[i].conditionalRequirement.conditionsToMeet[0].propertyName);
                        actionToggleButtons[i].GetComponentInChildren<Text>().text = "<color=\"red\">[" + conditionLabelText +"]</color>" + actionOptions[i].conditionalRequirement.failButtonText;
                        actionToggleButtons[i].GetComponentInChildren<Toggle>().interactable = false;
                    }
                }
            }
            else {
                actionToggleButtons[i].GetComponentInChildren<Text>().text = actionOptions[i].buttonText;
                actionToggleButtons[i].GetComponentInChildren<Toggle>().interactable = true;
            }
        }
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
