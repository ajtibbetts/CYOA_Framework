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

    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    // events
    public static event Action onGameStartSelected;
    public static event Action onDialogueEnded;
    public static event Action<string> onOptionSelected;
    public static event Action onProceedToNextNode;
    public static event Action onMainMenuOpened;
    public static event Action onCaseMenuOpened;

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
    public Vector2 startingRollScreenPosition;
    public float rollScreenAnimationTime;

    [Header("Content")]

    [Tooltip("Speed that game will auto scroll scroll to additive paragarphs.")]
    public float autoScrollSpeed = 5f;
    private bool isAutoScrolling;
    [SerializeField] private GameObject contentScrollParent;
    public GameObject contentScrollContainer;
    public Scrollbar vertScrollBar;
    public GameObject paragraphPrefab;
    public GameObject paragraphPortraitPrefab;
    public GameObject imagePrefab;
    [SerializeField] GameObject _topMenuBar;
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
    public GameObject additiveContinueBtnPrefab;

    // internal components
    // [HideInInspector] public conditionManager conditionChecker;
    [HideInInspector] public contentManager contentManager;
    
    private UISTATE _UISTATE;

    private List<GameObject> _contentParagraphs = new List<GameObject>();
    private GameObject _additiveContinueButton;
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

    private bool _inAdditiveDialogueState = false;

    void Awake() {
        //init singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        // add event listener
        SwipeDetector.OnSwipe += onSwipe;
        gameController.Instance.OnGameStateChanged += UpdateUIGameState;
        DialogueParser.onDialogueReachedDeadEnd += CreateEndDialogueButton;
        PlayerCaseRecord.OnCaseEnabled += SetCaseMenuIconActive;
        PlayerCaseRecord.OnMessageToUI += addToContentText;
        PlayerCaseRecord.OnLinkToUI += addLinkToContentText;
        UIScreen.onCloseMenu += CloseUIMenu;
        contentLinkManager.OnOpenMenu += OpenUIMenu;
        UIViewport.onViewPortTapped += CheckAdditiveState;

        RollScreen.onRollScreenReady += EnableRollScreen;
        RollScreen.onRollScreenComplete += SlideOutRollScreen;

        // set parent controller and child components
        controller = GetComponent<gameController>();
        // conditionChecker = gameObject.AddComponent(typeof(conditionManager)) as conditionManager;
        contentManager = gameObject.AddComponent(typeof(contentManager)) as contentManager;
        // set initials content prefabs
        UI_contentImage = GameObject.Instantiate(imagePrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        UI_contentImage.SetActive(false);
        // UI_paragraph = GameObject.Instantiate(paragraphPrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        CreateContentParagraph("");
    }

    void Start(){
        
        // register event listeners
        
        // controller.CheckManager.onRollCheckStart += initRollUI;
        // controller.CheckManager.onRollCheckPass += initRollPassUI;
        // controller.CheckManager.onRollCheckFail += initRollFailUI;
        
        

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
                _topMenuBar.GetComponent<Image>().color = globalConfig.UI.Gameplay.menuBarWorldNavColor;
                _bottonMenuBar.GetComponent<Image>().color = globalConfig.UI.Gameplay.menuBarWorldNavColor;
            break;
            case GAMESTATE.DIALOGUE:
                gameStateText.text = "Dialogue\n(Fast Travel Unavailable)";
                _topMenuBar.GetComponent<Image>().color = globalConfig.UI.Gameplay.menuBarDialogueColor;
                _bottonMenuBar.GetComponent<Image>().color = globalConfig.UI.Gameplay.menuBarDialogueColor;
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
        onMainMenuOpened?.Invoke();
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
        onCaseMenuOpened?.Invoke();
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
        SlideInRollScreen();
        contentScrollParent.SetActive(false);
    }

    public void SlideInRollScreen()
    {
        iTween.ValueTo(RollCanvas.gameObject, iTween.Hash(
            "from", RollCanvas.GetComponent<RectTransform>().anchoredPosition,
            "to", targetPosition,
            "time", rollScreenAnimationTime,
            "onupdatetarget", this.gameObject, 
            "onupdate", "MoveRollScreenUI"));
    }

    [ContextMenu ("Disable Roll Screen")]
    public void DisableRollScreen()
    {
        Debug.Log("UI Manager ---- Disabling Roll Screen");
        contentScrollParent.SetActive(true);
        Debug.Log("content screen active: " + contentScrollParent.activeSelf);
        RollCanvas.GetComponent<Canvas>().enabled = false;
        RollCanvas.SetActive(false);
        contentScrollParent.SetActive(true);
    }

    public void SlideOutRollScreen()
    {
        iTween.ValueTo(RollCanvas.gameObject, iTween.Hash(
            "from", RollCanvas.GetComponent<RectTransform>().anchoredPosition,
            "to", startingRollScreenPosition,
            "time", rollScreenAnimationTime,
            "onupdatetarget", this.gameObject, 
            "onupdate", "MoveRollScreenUI",
            "oncompletetarget", this.gameObject,
            "oncomplete","DisableRollScreen"));
    }

    public void MoveRollScreenUI(Vector2 position) {
        RollCanvas.GetComponent<RectTransform>().anchoredPosition = position;
    }
    /* GAME START BUTTONS / METHODS */
    
    public void CreateContentParagraph(string contentToAdd)
    {
        Debug.Log("UI Manager --- Adding content paragraph with text: " + contentToAdd);
        var newParagraph = GameObject.Instantiate(paragraphPrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        var contentText = newParagraph.GetComponent<TextMeshProUGUI>();
        contentText.text = "";
        // add any additional messages
        foreach(string message in _additionalUIMessages)
        {
            contentText.text += message;
        }
        _additionalUIMessages.Clear();
        contentText.text += contentManager.parseContent(contentToAdd) + "\n";

        _contentParagraphs.Add(newParagraph);
        SetPreviousParagraphsColor();

        CheckForAutoScroll();
    }

    public void CreateContentPortraitParagraph(string contentToAdd, Sprite portraitSprite)
    {
        Debug.Log("UI Manager --- Adding portrait paragraph with text: " + contentToAdd);
        var newPortraitParagraph = GameObject.Instantiate(paragraphPortraitPrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        var portraitMgr = newPortraitParagraph.GetComponent<UIPortraitParagraph>();
        var contentText = contentManager.parseContent(contentToAdd) + "\n";
        portraitMgr.UpdateText(contentText);
        portraitMgr.UpdatePortrait(portraitSprite);
        portraitMgr.UpdatePreferredHeight();

        _contentParagraphs.Add(newPortraitParagraph);
        SetPreviousParagraphsColor();

        // check for any additional UI messages and create content paragraph for those.
        if(_additionalUIMessages.Count > 0) CreateContentParagraph("");
        CheckForAutoScroll();
    }

    private void CheckForAutoScroll()
    {
        // if we have more than two paragraphs (i.e additive content, scroll to start of new paragraph)
        if(_contentParagraphs.Count >= 2 && !isAutoScrolling)
        {
            isAutoScrolling = true;
            StartCoroutine(SetScrollHeight());
        }
        else contentScrollParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
    }

    private IEnumerator SetScrollHeight()
    {
        yield return new WaitForEndOfFrame(); // wait for UI to draw everything
        var isScrollBarActive = vertScrollBar.isActiveAndEnabled;
        Debug.Log("Scroll bar currently enabled: " + isScrollBarActive);
        var contentPanel = contentScrollContainer.GetComponent<RectTransform>();
        var scrollRect = contentScrollParent.GetComponent<ScrollRect>();
        // set content posY to equal the heigh of all paragarphs combined not counting last one
        float totalHeight = 0f;
        foreach(var item in _contentParagraphs)
        {
            if(_contentParagraphs.IndexOf(item) == _contentParagraphs.Count - 1) continue; // ignore most recent entry
            totalHeight += item.GetComponent<RectTransform>().rect.height;
        }

        var targetPosition = new Vector2(0, totalHeight + 25f); // 50 for spacing offset from vertical layout group
        // Debug.Log("target pos y: " + targetPosition.y);
        
        // then set the posY of scroll rect
        // contentPanel.anchoredPosition = targetPosition; 
        while(contentPanel.anchoredPosition.y < targetPosition.y && isScrollBarActive) // only scroll if scroll bar is active
        {
            var currentYPos = contentPanel.anchoredPosition.y;
            // Debug.Log("target pos y: " + targetPosition.y);
            // Debug.Log("current pos y: "+ currentYPos);
            contentPanel.anchoredPosition = new Vector2(0, currentYPos + (Time.deltaTime * autoScrollSpeed));
            if(scrollRect.verticalNormalizedPosition <= 0) break; // stop scrolling if we've hit the end of scrollbar/scrollrect
            yield return null;
        }

        isAutoScrolling = false;
    }

    public void SetAdditiveDialogueState()
    {
        // first create content paragraph with continue text
        if(_additiveContinueButton != null)
        {
            Destroy(_additiveContinueButton.gameObject);
        }
        _additiveContinueButton = GameObject.Instantiate(additiveContinueBtnPrefab, Vector3.zero, Quaternion.identity, contentScrollContainer.transform);
        _additiveContinueButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { CheckAdditiveState(); });
        _inAdditiveDialogueState = true;

        
    }

    private void SetPreviousParagraphsColor()
    {
        if(_contentParagraphs.Count < 2) return; // only concerned with > 2 paragraphs
        // set all but current paragraph text to gray
        foreach(var paragraph in _contentParagraphs)
        {
            if(_contentParagraphs.IndexOf(paragraph) == _contentParagraphs.Count - 1) continue;
            var text = paragraph.GetComponent<TextMeshProUGUI>();
            if(text != null) text.color = globalConfig.UI.OldParagraphTextColor;
            else 
            {
                var texts = paragraph.GetComponentsInChildren<TextMeshProUGUI>();
                foreach(var t in texts)
                {
                    t.color = globalConfig.UI.OldParagraphTextColor;
                }
            }
        }
    }

    public void CheckAdditiveState()
    {
        if(_inAdditiveDialogueState)
        {
            Debug.Log("UI Manager ---- Viewport tapped while in additive state. proceeding to next node");
            _inAdditiveDialogueState = false;
            if(_additiveContinueButton != null)
            {
                Destroy(_additiveContinueButton.gameObject);
            }
            onProceedToNextNode?.Invoke();
        }
    }

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


    public void ClearContentAndButtons()
    {
        foreach(var paragraph in _contentParagraphs)
        {
            Destroy(paragraph);
        }
        _contentParagraphs.Clear();
        
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

        if(_additiveContinueButton != null)
        {
            Destroy(_additiveContinueButton.gameObject);
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
            // try destroying instead
            Destroy(actionToggleButtons[0]);
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
