using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CaseDataObjects;


public class CaseMenu : MonoBehaviour
{
    private static CaseMenu _instance;
    public static CaseMenu Instance { get { return _instance; } }

    
    
    [Header("Menu Icons")]
    [SerializeField] private GameObject caseSummaryButton;
    [SerializeField] private GameObject victimButton;
    [SerializeField] private GameObject leadsButton;
    [SerializeField] private GameObject evidenceButton;
    [SerializeField] private GameObject profilesButton;
    [SerializeField] private GameObject suspectsButton;
    [SerializeField] private GameObject notesButton;
    [SerializeField] private GameObject helpButton;

    private GameObject _activeContent;
    private GameObject _activeButton;

    [Header("Content Screen")]
    public GameObject contentContainer;
    public GameObject caseSummaryScreen;
    public GameObject victimDataScreen;
    public GameObject leadsScreen;
    public GameObject evidenceScreen;
    public GameObject profilesScreen;
    public GameObject suspectsScreen;
    public GameObject notesScreen;
    public GameObject helpScreen;

    // data managers
    private CaseSummaryScreen _caseSummaryManager;
    private VictimDataScreen _victimManager;
    private LeadsScreen _leadManager;
    private EvidenceScreen _evidenceManager;
    private ProfilesScreen _profilesManager;


    private PlayerCaseRecord _caseRecord;


    private void Awake() {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        // turn all screens off
        caseSummaryScreen.SetActive(false);
        victimDataScreen.SetActive(false);
        leadsScreen.SetActive(false);
        evidenceScreen.SetActive(false);
        profilesScreen.SetActive(false);
        suspectsScreen.SetActive(false);
        notesScreen.SetActive(false);
        helpScreen.SetActive(false);

        
        
    }
    void Start()
    {
        _caseRecord = PlayerCaseRecord.Instance;
        SetupManagers();
        OpenCaseSummaryScreen();
    }

    void SetupManagers()
    {
        _caseSummaryManager = caseSummaryScreen.GetComponent<CaseSummaryScreen>();
        _caseSummaryManager.SetCaseRecord(_caseRecord);
        CaseSummaryScreen.OnButtonPressed += JumpToScreenFromSubMenu;

        _victimManager = victimDataScreen.GetComponent<VictimDataScreen>();
        _victimManager.SetCaseRecord(_caseRecord);

        _leadManager = leadsScreen.GetComponent<LeadsScreen>();
        _leadManager.SetCaseRecord(_caseRecord);

        _evidenceManager = evidenceScreen.GetComponent<EvidenceScreen>();
        _evidenceManager.SetCaseRecord(_caseRecord);

        _profilesManager = profilesScreen.GetComponent<ProfilesScreen>();
        _profilesManager.SetCaseRecord(_caseRecord);
        
    }

    public void SetCurrentScreenInactive()
    {
        if(_activeContent != null) 
        {
            _activeContent.SetActive(false);
            SetInactiveColor(_activeButton);
        }
    }

    public void SetActiveScreen(GameObject screenToActivate, GameObject buttonToActivate)
    {
        if(_activeContent != screenToActivate)
        {
            SetCurrentScreenInactive();
            _activeContent = screenToActivate;
            _activeContent.SetActive(true);
            _activeButton = buttonToActivate;
            SetActiveColor(buttonToActivate);
        }
    }

    void SetInactiveColor(GameObject button)
    {
        var buttonImage = button.GetComponent<Image>();
        buttonImage.color = Color.white; 
    }

    void SetActiveColor(GameObject button)
    {
        var buttonImage = button.GetComponent<Image>();
        buttonImage.color = Color.yellow;
    }

    public void OpenCaseSummaryScreen()
    {
        _caseSummaryManager.UpdateData();
        SetActiveScreen(caseSummaryScreen, caseSummaryButton);
    }
    public void OpenVictimDataScreen()
    {
        _victimManager.UpdateData();
        SetActiveScreen(victimDataScreen, victimButton);
    }
    public void OpenLeadsScreen()
    {
        _leadManager.UpdateData();
        SetActiveScreen(leadsScreen, leadsButton);
    }
    public void OpenEvidenceScreen()
    {
        _evidenceManager.UpdateData();
        SetActiveScreen(evidenceScreen, evidenceButton);
    }
    public void OpenProfilesScreen()
    {
        _profilesManager.UpdateData();
        SetActiveScreen(profilesScreen, profilesButton);
    }
    public void OpenSuspectsScreen()
    {
        SetActiveScreen(suspectsScreen, suspectsButton);
    }
    public void OpenNotesScreen()
    {
        SetActiveScreen(notesScreen, notesButton);
    }
    public void OpenHelpScreen()
    {
        SetActiveScreen(helpScreen, helpButton);
    }

    public void JumpToScreenFromSubMenu(string screenName)
    {
        switch(screenName)
        {
            case "victim":
                OpenVictimDataScreen();
            break;
            case "suspect":
                OpenSuspectsScreen();
            break;
            default:
            break;
        }
    }
}
