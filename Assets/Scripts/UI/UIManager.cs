using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using globalDataTypes;

public class UIManager : MonoBehaviour
{
    [HideInInspector] public gameController controller;

    // events
    public static event Action onGameStartSelected;
    public static event Action onDialogueEnded;
    public static event Action<string> onOptionSelected;

    // UI elements
    [Header("Game Menus")]
    public RectTransform mainMenuUI;
    public Vector2 startingMainMenuPosition;
    [SerializeField] private GameObject caseMenuIcon;
    public RectTransform caseMenuUI;
    public Vector2 startingCaseMenuPosition;
    public Vector2 targetPosition;
    public float animationTime;

    [Header("Roll Screen")]
    [SerializeField] private GameObject RollCanvas;

    [Header("Content")]
    [SerializeField] private GameObject contentScrollParent;
    public GameObject contentScrollContainer;
    public Scrollbar vertScrollBar;
    public GameObject paragraphPrefab;
    public GameObject imagePrefab;
    [SerializeField] GameObject _bottonMenuBar;
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
    
    private UISTATE _UISTATE;

    private GameObject UI_paragraph;
    private GameObject UI_contentImage;
    private List<String> _additionalUIMessages = new List<string>();
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
        gameController.Instance.OnGameStateChanged += UpdateUIGameState;
        DialogueParser.onDialogueReachedDeadEnd += CreateEndDialogueButton;
        PlayerCaseRecord.OnCaseEnabled += SetCaseMenuIconActive;
        PlayerCaseRecord.OnMessageToUI += addToContentText;
        PlayerCaseRecord.OnLinkToUI += addLinkToContentText;
        UIScreen.onCloseMenu += CloseUIMenu;
        contentLinkManager.OnOpenMenu += OpenUIMenu;

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
        
        

        _UISTATE = UISTATE.NORMALGAMEPLAY;
        // turn off menu icons if need be
        SetCaseMenuIconActive(PlayerCaseRecord.Instance.onActiveCase);
        DisableRollScreen();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateUIGameState(GAMESTATE GAMESTATE)
    {
        var gameStateText = _bottonMenuBar.transform.Find("gameStateText").GetComponent<TextMeshProUGUI>();
        switch(GAMESTATE)
        {
            case GAMESTATE.WORLDNAVIGATION:
                gameStateText.text = "Navigating\n(Fast Travel Available)";
            break;
            case GAMESTATE.DIALOGUE:
                gameStateText.text = "Dialogue\n(Fast Travel Unavailable)";
            break;
            case GAMESTATE.INVESTIGATING:
                gameStateText.text = "Investigating\n(Fast Travel Unavailable)";
            break;
        }
    }


    /* MENU SWIPE METHODS */
    // slides in the main menu from left side of the screen
    public void SlideInMainMenu(){
        iTween.ValueTo(mainMenuUI.gameObject, iTween.Hash(
            "from", mainMenuUI.anchoredPosition,
            "to", targetPosition,
            "time", animationTime,
            "onupdatetarget", this.gameObject, 
            "onupdate", "MoveMainMenuUI"));
    }

    public void SlideOutMainMenu()
    {
        iTween.ValueTo(mainMenuUI.gameObject, iTween.Hash(
            "from", mainMenuUI.anchoredPosition,
            "to", startingMainMenuPosition,
            "time", animationTime,
            "onupdatetarget", this.gameObject, 
            "onupdate", "MoveMainMenuUI"));
    }
    
 
    public void MoveMainMenuUI(Vector2 position) {
        mainMenuUI.anchoredPosition = position;
    }

    public void SlideInCaseMenu(){
        iTween.ValueTo(caseMenuUI.gameObject, iTween.Hash(
            "from", caseMenuUI.anchoredPosition,
            "to", targetPosition,
            "time", animationTime,
            "onupdatetarget", this.gameObject, 
            "onupdate", "MoveCaseMenuUI"));
    }

    public void SlideOutCaseMenu()
    {
        iTween.ValueTo(caseMenuUI.gameObject, iTween.Hash(
            "from", caseMenuUI.anchoredPosition,
            "to", startingCaseMenuPosition,
            "time", animationTime,
            "onupdatetarget", this.gameObject, 
            "onupdate", "MoveCaseMenuUI"));
    }

    public void MoveCaseMenuUI(Vector2 position) {
        caseMenuUI.anchoredPosition = position;
    }

    public void onSwipe(SwipeData data) {
        Debug.Log("Swipe in Direction: " + data.Direction);
        // if(data.Direction == SwipeDirection.Right && _UISTATE == UISTATE.NORMALGAMEPLAY){
        //     Debug.Log("Swipe is right. Start X: " + data.StartPosition.x + " End X: " + data.EndPosition.x);
        //     if(data.EndPosition.x <= globalConfig.UI.MAX_MENU_SWIPE_POS_X){
        //         Debug.Log("Open menu!");
        //         OpenMainMenu();
        //     }
        // }
        // else if (data.Direction == SwipeDirection.Left && _UISTATE == UISTATE.MAINMENU)
        // {
        //     Debug.Log("Swipe left detected while main menu is open. Closing main menu.");
        //     CloseMainMenu();
        // }

        switch(data.Direction)
        {
            case SwipeDirection.Left:
                if(_UISTATE == UISTATE.MAINMENU) CloseMainMenu();
                else if(data.EndPosition.x >= globalConfig.UI.MIN_CASEMENU_SWIPE_POS_X) OpenCaseMenu();
            break;
            case SwipeDirection.Right:
                if(_UISTATE == UISTATE.CASEMENU) CloseCaseMenu();
                else if(data.EndPosition.x <= globalConfig.UI.MAX_MENU_SWIPE_POS_X) OpenMainMenu();
            break;
        }
    }

    public void OpenMainMenu()
    {
        Debug.Log("Main menu button touched. Opening main menu.");
        _UISTATE = UISTATE.MAINMENU;
        SlideInMainMenu();
    }

    public void CloseMainMenu()
    {
        Debug.Log("Main menu close button touched. Closing main menu.");
        _UISTATE = UISTATE.NORMALGAMEPLAY;
        SlideOutMainMenu();
    }

    public void OpenCaseMenu()
    {
        if(!PlayerCaseRecord.Instance.onActiveCase) return; // only open menu if a case is active
        Debug.Log("Case menu button touched. Opening main menu.");
        _UISTATE = UISTATE.CASEMENU;
        SlideInCaseMenu();
    }

    public void CloseCaseMenu()
    {
        Debug.Log("Case menu close button touched. Closing main menu.");
        _UISTATE = UISTATE.NORMALGAMEPLAY;
        SlideOutCaseMenu();
    }

    public void SetCaseMenuIconActive(bool isActive)
    {
        caseMenuIcon.SetActive(isActive);
    }

    public void CloseUIMenu(MENUTYPE menuType)
    {
        switch(menuType)
        {
            case MENUTYPE.MAINMENU:
                CloseMainMenu();
            break;
            case MENUTYPE.CASEMENU:
                CloseCaseMenu();
            break;
        }
    }
    public void OpenUIMenu(MENUTYPE menuType)
    {
        switch(menuType)
        {
            case MENUTYPE.MAINMENU:
                OpenMainMenu();
            break;
            case MENUTYPE.CASEMENU:
                OpenCaseMenu();
            break;
        }
    }

    public void ToggleRollScreen()
    {
        if(RollCanvas.GetComponent<Canvas>().enabled)
        {
            DisableRollScreen();
        }
        else EnableRollScreen();
    }

    [ContextMenu ("Enable Roll Screen")]
    public void EnableRollScreen()
    {
        RollCanvas.GetComponent<Canvas>().enabled = true;
        RollCanvas.SetActive(true);
        contentScrollParent.SetActive(false);
    }
    [ContextMenu ("Disable Roll Screen")]
    public void DisableRollScreen()
    {
        RollCanvas.GetComponent<Canvas>().enabled = false;
        RollCanvas.SetActive(false);
        contentScrollParent.SetActive(true);
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
            // // ensure all other toggles are off (USING TOGGLE GROUP ON CONTENT IN SCENE)
            
            _currentTargetNodeGUID = TargetNodeGuid;
            confirmActionButton.GetComponentInChildren<Text>().text = "Confirm";
            confirmActionButton.GetComponentInChildren<Button>().interactable = true;
            toggle.GetComponent<Image>().color = globalConfig.UI.toggleSelectedBackgroundColor;
        }
        else {
            _currentTargetNodeGUID = null;
            confirmActionButton.GetComponentInChildren<Text>().text = "Select option then confirm.";
            confirmActionButton.GetComponentInChildren<Button>().interactable = false;
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
        resetContent();
        var contentText = UI_paragraph.GetComponent<TextMeshProUGUI>();
        // add any additional messages
        foreach(string message in _additionalUIMessages)
        {
            contentText.text += message;
        }
        _additionalUIMessages.Clear();
        contentText.text += contentManager.parseContent(content) + "\n";
    }

    public void addToContentText(string content)
    {
        Debug.Log("UI MANAGER ----- adding to content text list" + content);
        _additionalUIMessages.Add("<color=" + globalConfig.UI.EventHexColor +"><align=\"center\"><i>" + contentManager.parseContent(content) + "</i></align></color>\n");
    }

    public void addLinkToContentText(string link)
    {
        Debug.Log("UI MANAGER ----- adding link to content text list" + link);
        var linkMessage = "<color="+ globalConfig.UI.LinkHexColor +"><align=\"center\"><u><link=\""+ link +"\">View Details</link></u></align></color>\n\n";
        _additionalUIMessages.Add(linkMessage);
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


    public void resetContent(){
        UI_paragraph.GetComponent<TextMeshProUGUI>().text = "";
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
        else if (actionToggleButtons.Count > 1) {
            // if not, disable the button initially forcing user to select an option.
            confirmActionButton.GetComponentInChildren<Button>().interactable = false;
        }
        else {
            Debug.LogError("UI MANAGER ---- ACTION TOGGLE BUTTON COUNT when checking for single option: " + actionToggleButtons.Count);
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
