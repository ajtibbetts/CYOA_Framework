using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    
    private static MainMenu _instance;
    public static MainMenu Instance { get { return _instance; } }

    [Header("Menu Icons")]
    public GameObject[] menuButtons;

    private GameObject activeContent;
    private int activeMenuButtonIndex;
    [Header("Content")]
    public GameObject contentContainer;
    public GameObject playerStatusScreen;
    public GameObject playerSkillsScreen;
    public GameObject gearScreen;
    public GameObject mapScreen;

    // menu data managers
    private PlayerStatusScreen _playerStatus;
    private PlayerSkillsScreen _playerSkills;


    
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
        GetManagers();
        OpenPlayerStatusScreen();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    void GetManagers()
    {
        _playerStatus = playerStatusScreen.GetComponent<PlayerStatusScreen>();
        _playerSkills = playerSkillsScreen.GetComponent<PlayerSkillsScreen>();
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

    public void OpenPlayerStatusScreen()
    {
        _playerStatus.UpdateData();
        SetActiveScreen(playerStatusScreen, 0);
    }

    public void OpenPlayerSkillsScreen()
    {
        _playerSkills.UpdateData();
        SetActiveScreen(playerSkillsScreen, 1);
    }

    public void OpenGearScreen()
    {
        SetActiveScreen(gearScreen, 2);
    }

    public void OpenMapScreen()
    {
        SetActiveScreen(mapScreen, 3);
    }
}
