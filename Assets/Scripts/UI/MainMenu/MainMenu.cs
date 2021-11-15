using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : UIMenu
{
    private static MainMenu _instance;
    public static MainMenu Instance { get { return _instance; } }

    [Header("Menu Icons")]
    [SerializeField] private GameObject playerStatusButton;
    [SerializeField] private GameObject playerSkillsButton;
    [SerializeField] private GameObject gearButton;
    [SerializeField] private GameObject mapButton;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject helpButton;

    [Header("Content")]
    public GameObject contentContainer;
    public GameObject playerStatusScreen;
    public GameObject playerSkillsScreen;
    public GameObject gearScreen;
    public GameObject mapScreen;

    // menu data managers
    private PlayerStatusScreen _playerStatusManager;
    private PlayerSkillsScreen _playerSkillsManager;
    private MapScreen _mapManager;

    private void Awake() {
        //init singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        playerStatusScreen.SetActive(false);
        playerSkillsScreen.SetActive(false);
        gearScreen.SetActive(false);
        mapScreen.SetActive(false);
        SetupManagers();
        OpenPlayerStatusScreen();

        // add listener for new maps
        CaseManager.OnNewCaseMap += SetActiveMap;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    protected override void SetupManagers()
    {
        _playerStatusManager = playerStatusScreen.GetComponent<PlayerStatusScreen>();
        _playerSkillsManager = playerSkillsScreen.GetComponent<PlayerSkillsScreen>();
        _mapManager = mapScreen.GetComponent<MapScreen>();
    }

    public void OpenPlayerStatusScreen()
    {
        _playerStatusManager.UpdateData();
        SetActiveScreen(playerStatusScreen, playerStatusButton);
    }

    public void OpenPlayerSkillsScreen()
    {
        _playerSkillsManager.UpdateData();
        SetActiveScreen(playerSkillsScreen, playerSkillsButton);
    }

    public void OpenGearScreen()
    {
        SetActiveScreen(gearScreen, gearButton);
    }

    public void OpenMapScreen(string areaName = null)
    {   
        _mapManager.UpdateData();
        SetActiveScreen(mapScreen, mapButton);
        if(areaName != null && areaName.Length > 0)
        {
            var locationToOpen = CaseManager.Instance.GetMapLocations().Find(x => x.GetAreaName() == areaName);
            if(locationToOpen != null)
            {
                _mapManager.OpenConfirmLocationScreen(locationToOpen);
            }
            else Debug.LogError("MAIN MENU ---- FAILED TO OPEN MAP LOCATION CONFIRM SCREEN FOR AREA: " + areaName);
        }
    }

    public void SetActiveMap(MapObject newMap)
    {
        _mapManager.LoadMap(newMap);
    }
}
