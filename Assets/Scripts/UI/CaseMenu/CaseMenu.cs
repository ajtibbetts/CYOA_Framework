using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CaseDataObjects;


public class CaseMenu : UIMenu
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
    [SerializeField] private GameObject warrantButton;
    [SerializeField] private GameObject helpButton;

    [Header("Content Screen")]
    public GameObject contentContainer;
    public GameObject caseSummaryScreen;
    public GameObject victimDataScreen;
    public GameObject leadsScreen;
    public GameObject evidenceScreen;
    public GameObject profilesScreen;
    public GameObject suspectsScreen;
    public GameObject warrantScreen;
    public GameObject helpScreen;

    // data managers
    private CaseSummaryScreen _caseSummaryManager;
    private VictimDataScreen _victimManager;
    private LeadsScreen _leadManager;
    private EvidenceScreen _evidenceManager;
    private ProfilesScreen _profilesManager;
    private SuspectsScreen _suspectsManager;
    private WarrantScreen _warrantManager;


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
        warrantScreen.SetActive(false);
        helpScreen.SetActive(false);

        PlayerCaseRecord.OnCaseDataUpdated += UpdateActiveData;
        UIManager.onCaseMenuOpened += UpdateActiveData;
    }
    void Start()
    {
        _caseRecord = PlayerCaseRecord.Instance;
        SetupManagers();
        OpenCaseSummaryScreen();
    }

    protected override void SetupManagers()
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

        _suspectsManager = suspectsScreen.GetComponent<SuspectsScreen>();
        _suspectsManager.SetCaseRecord(_caseRecord);
        _suspectsManager.onGoToProfile += GoToCharacterProfile;

        _warrantManager = warrantScreen.GetComponent<WarrantScreen>();
        _warrantManager.SetCaseRecord(_caseRecord);
        WarrantScreen.OnButtonPressed += JumpToScreenFromSubMenu;
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
        _suspectsManager.UpdateData();
        SetActiveScreen(suspectsScreen, suspectsButton);
    }
    public void OpenWarrantScreen()
    {
        _warrantManager.UpdateData();
        SetActiveScreen(warrantScreen, warrantButton);
    }
    public void OpenHelpScreen()
    {
        SetActiveScreen(helpScreen, helpButton);
    }


    // event handlers
    public void JumpToScreenFromSubMenu(string screenName)
    {
        switch(screenName)
        {
            case "victim":
                OpenVictimDataScreen();
            break;
            case "suspect":
                OpenSuspectsScreen();
                _suspectsManager.UpdateSuspectView(_caseRecord.GetPrimarySuspect());
            break;
            case "profiles":
                OpenProfilesScreen();
            break;
            default:
            break;
        }
    }

    public void GoToCharacterProfile(CharacterProfileData characterProfile)
    {
        OpenProfilesScreen();
        _profilesManager.OpenProfileDetailsScreen(characterProfile);
    }

    public void GoToCharacterProfileByID(string characterID)
    {
        var characterProfile = _caseRecord.GetProfiles().Find(x => x.characterID == characterID);
        if(characterProfile !=null)
        {
            OpenProfilesScreen();
            _profilesManager.OpenProfileDetailsScreen(characterProfile);
        }
        else
        {
            Debug.LogError("CASE MENU - failed to open profile for name: " + characterID);
        }
        
    }
}
