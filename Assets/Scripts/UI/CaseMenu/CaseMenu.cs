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
    public GameObject[] menuButtons;
    private GameObject activeContent;
    private int activeMenuButtonIndex;
    [Header("Content")]
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
        
    }

    public void SetCurrentScreenInactive()
    {
        if(activeContent != null) 
        {
            activeContent.SetActive(false);
            SetInactiveColor(activeMenuButtonIndex);
        }
    }

    public void SetActiveScreen(GameObject screen, int index)
    {
        if(activeContent != screen)
        {
            SetCurrentScreenInactive();
            activeContent = screen;
            activeMenuButtonIndex = index;
            activeContent.SetActive(true);
            SetActiveColor(index);
        }
    }

    void SetInactiveColor(int index)
    {
        var buttonImage = menuButtons[index].GetComponent<Image>();
        buttonImage.color = Color.white; 
    }

    void SetActiveColor(int index)
    {
        var buttonImage = menuButtons[index].GetComponent<Image>();
        buttonImage.color = Color.yellow;
    }

    public void OpenCaseSummaryScreen()
    {
        _caseSummaryManager.UpdateData();
        SetActiveScreen(caseSummaryScreen, 0);
    }
    public void OpenVictimDataScreen()
    {
        _victimManager.UpdateData();
        SetActiveScreen(victimDataScreen, 1);
    }
    public void OpenLeadsScreen()
    {
        _leadManager.UpdateData();
        SetActiveScreen(leadsScreen, 2);
    }
    public void OpenEvidenceScreen()
    {
        _evidenceManager.UpdateData();
        SetActiveScreen(evidenceScreen, 3);
    }
    public void OpenProfilesScreen()
    {
        SetActiveScreen(profilesScreen, 4);
    }
    public void OpenSuspectsScreen()
    {
        SetActiveScreen(suspectsScreen, 5);
    }
    public void OpenNotesScreen()
    {
        SetActiveScreen(notesScreen, 6);
    }
    public void OpenHelpScreen()
    {
        SetActiveScreen(helpScreen, 7);
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
