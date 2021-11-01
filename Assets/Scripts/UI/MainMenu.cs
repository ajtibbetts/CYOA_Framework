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
    public GameObject caseScreen;
    public GameObject mapScreen;


    
    private void Awake() {
        //init singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        playerStatusScreen.SetActive(false);
        caseScreen.SetActive(false);
        mapScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void OpenPlayerStatusScreen()
    {
        if(activeContent != null) 
        {
            activeContent.SetActive(false);
            SetInactiveColor(activeMenuButtonIndex);
        }
        activeContent = playerStatusScreen;
        activeMenuButtonIndex = 0;
        activeContent.SetActive(true);
        SetActiveColor(0);
    }

    public void OpenCaseScreen()
    {
        if(activeContent != null)
        {
            activeContent.SetActive(false);
            SetInactiveColor(activeMenuButtonIndex);
        }
        activeContent = caseScreen;
        activeMenuButtonIndex = 1;
        activeContent.SetActive(true);
        SetActiveColor(1);
    }

    public void OpenMapScreen()
    {
        if(activeContent != null)
        {
            activeContent.SetActive(false);
            SetInactiveColor(activeMenuButtonIndex);
        }
        activeContent = mapScreen;
        activeMenuButtonIndex = 3;
        activeContent.SetActive(true);
        SetActiveColor(3);
    }
}
