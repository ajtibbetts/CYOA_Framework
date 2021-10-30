using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [HideInInspector] public gameController controller;

    // events
    public static event Action onGameStartSelected;
    public static event Action onDialogueEnded;
    public static event Action<string> onOptionSelected;

    // UI elements
    [Header("Main Menu")]
    public RectTransform uGuiElement;
    public Vector2 targetPosition;
    public float animationTime;
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
    // [HideInInspector] public conditionManager conditionChecker;
    [HideInInspector] public contentManager contentManager;
    


    private GameObject UI_paragraph;
    private GameObject UI_contentImage;
    private List<GameObject> actionToggleButtons = new List<GameObject>();
    private GameObject confirmActionButton;
    private GameObject endDialogueButton;
    private GameObject startGameButton;
    private string _currentTargetNodeGUID;

    // store most recent option data for single option
    
    private string _latestTargetGUID;
    private string _latestChoiceText;

    void Awake() {
        // add event listener
        SwipeDetector.OnSwipe += onSwipe;

        // set parent controller and child components
        controller = GetComponent<gameController>();
        // conditionChecker = gameObject.AddComponent(typeof(conditionManager)) as conditionManager;
        contentManager = gameObject.AddComponent(typeof(contentManager)) as contentManager;
        // set initials content prefabs
        UI_contentImage = GameObject.Instantiate(imagePrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        UI_contentImage.SetActive(false);
        UI_paragraph = GameObject.Instantiate(paragraphPrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
    }

    void Start(){
        
        // register event listeners
        controller.CheckManager.onRollCheckStart += initRollUI;
        controller.CheckManager.onRollCheckPass += initRollPassUI;
        controller.CheckManager.onRollCheckFail += initRollFailUI;
        DialogueParser.onDialogueReachedDeadEnd += CreateEndDialogueButton;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /* MENU SWIPE METHODS */
    // slides in the main menu from left side of the screen
    public void SlideIn(){
        iTween.ValueTo(uGuiElement.gameObject, iTween.Hash(
            "from", uGuiElement.anchoredPosition,
            "to", targetPosition,
            "time", animationTime,
            "onupdatetarget", this.gameObject, 
            "onupdate", "MoveGuiElement"));
    }
    
 
    public void MoveGuiElement(Vector2 position) {
        uGuiElement.anchoredPosition = position;
    }

    public void onSwipe(SwipeData data) {
        Debug.Log("Swipe in Direction: " + data.Direction);
        if(data.Direction == SwipeDirection.Right){
            Debug.Log("Swipe is right. Start X: " + data.StartPosition.x + " End X: " + data.EndPosition.x);
            if(data.EndPosition.x <= globalConfig.UI.MAX_MENU_SWIPE_POS_X){
                Debug.Log("Open menu!");
                SlideIn();
            }
        }
    }

    /* GAME START BUTTONS / METHODS */
    
    public void initGameStartButton()
    {   
        Debug.Log("UI MANAGER ---- Adding game start button.");
        startGameButton = GameObject.Instantiate(playerActionOptionBtnPrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        startGameButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { StartGameFromUI(); });
        startGameButton.GetComponentInChildren<Text>().text = "Tap here to start.";
    }

    public void StartGameFromUI()
    {
        onGameStartSelected?.Invoke();
        Destroy(startGameButton.gameObject);
    }

    /* GAME DIALOGUE OPTION BUTTONS */

    public void initConfirmActionButton()
    {
        confirmActionButton = GameObject.Instantiate(playerActionOptionBtnPrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        confirmActionButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { processPlayerAction(); });
        confirmActionButton.GetComponentInChildren<Text>().text = "Select option then confirm.";

        checkForSingleOption();
    }

    public void CreateEndDialogueButton()
    {
        endDialogueButton = GameObject.Instantiate(playerActionOptionBtnPrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        endDialogueButton.GetComponentInChildren<Button>().onClick.AddListener(() => onDialogueEnded?.Invoke());
        endDialogueButton.GetComponentInChildren<Text>().text = "Proceed.";
    }


    public void ClearButtons()
    {
        var toggles = contentScrollContainer.GetComponentsInChildren<Toggle>();
        Debug.Log("UI MANAGER ---- Clearing Buttons. Current button count: " + toggles.Length);
        for (int i = 0; i < toggles.Length; i++)
        {
            Destroy(toggles[i].gameObject);
        }
        actionToggleButtons.Clear(); // clear list

        if(confirmActionButton != null)
        {
            Destroy(confirmActionButton.gameObject);
        }

        if(endDialogueButton != null)
        {
            Destroy(endDialogueButton.gameObject);
        }
    }

    public void CreateDialogueOptionButton(string TargetNodeGuid, string choiceText)
    {
        GameObject toggle = GameObject.Instantiate(togglePrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        Toggle toggleComponent = toggle.GetComponent<Toggle>();
        toggle.GetComponentInChildren<Text>().text = choiceText;
        toggleComponent.isOn = false; 
        toggleComponent.group = toggleGroup;
        toggleComponent.onValueChanged.AddListener(delegate { SetCurrentDialogueOption(toggleComponent, TargetNodeGuid);});
        actionToggleButtons.Add(toggle);
        toggle.name = "toggleChoice_" + actionToggleButtons.Count;
        // cache most recent entry
        _latestTargetGUID = TargetNodeGuid;
        _latestChoiceText = choiceText;

        Debug.Log($"UI MANAGER ---- Adding choice Toggle text: {choiceText} | target GUID: {TargetNodeGuid}");

    }

    public void SetCurrentDialogueOption(Toggle toggle, string TargetNodeGuid)
    {
        Debug.Log("UI MANAGER ---- Checking this toggle");
        if(toggle.isOn) {
            
            _currentTargetNodeGUID = TargetNodeGuid;
            confirmActionButton.GetComponentInChildren<Text>().text = "Confirm";
            toggle.GetComponent<Image>().color = globalConfig.UI.toggleSelectedBackgroundColor;
        }
        else {
            _currentTargetNodeGUID = null;
            confirmActionButton.GetComponentInChildren<Text>().text = "Select option then confirm.";
            toggle.GetComponent<Image>().color = globalConfig.UI.toggleInactiveBackgroundColor;
        }
    }

    public void processPlayerAction(){

        Debug.Log("UI MANAGER ---- Button clicked. Processing action for guid: " + _currentTargetNodeGUID);
        
        if(_currentTargetNodeGUID != null)
        {
            onOptionSelected?.Invoke(_currentTargetNodeGUID);
            _currentTargetNodeGUID = null;
        }
        else if (_latestTargetGUID != null && actionToggleButtons.Count == 1)
        {
            Debug.Log("UI MANAGER ---- Button clicked but no current node guid set. Processing action for latest target guid: " + _latestTargetGUID);
            onOptionSelected?.Invoke(_latestTargetGUID);
            _latestTargetGUID = null;
            _latestChoiceText = null;
        }
        else
        {
            Debug.Log("UI MANAGER ---- Button clicked but no current or latest target guid set. Check it out.");
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
        Debug.Log("UI MANAGER ----- updating content text..");
        resetScroll();
        //string parsedContent = contentManager.parseContent(content);
        
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
        playerGoldText.GetComponent<TextMeshProUGUI>().text = content;
    }


    public void resetScroll(){
        vertScrollBar.value = 1;
    }

    public void checkForSingleOption(){

        Debug.Log("UI MANAGER ---- Checking for single option. Latest targetGuid: " + _latestTargetGUID);
        Debug.Log("UI MANAGER ---- Current option length: " + actionToggleButtons.Count);
        // if single choice, change confirm to advance to next dialogue
        if(actionToggleButtons.Count == 1)
        {
            Debug.Log("Setting current guid to latest guid: " + _latestTargetGUID);
            _currentTargetNodeGUID = _latestTargetGUID;
            Debug.Log("New current guid: " + _currentTargetNodeGUID);
            confirmActionButton.GetComponentInChildren<Text>().text = _latestChoiceText;
            actionToggleButtons[0].SetActive(false);
        }
    }

    public void initRollUI()
    {
        Debug.Log("UI MANAGER: Initializing roll UI.");
    }
    
    public void initRollPassUI()
    {
        Debug.Log("UI MANAGER: Initializing roll PASS UI.");
    }

    public void initRollFailUI()
    {
        Debug.Log("UI MANAGER: Initializing roll FAIL UI.");
    }
}
