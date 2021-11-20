using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using globalDataTypes;

[RequireComponent(typeof(TMP_Text))]
public class contentLinkManager : MonoBehaviour, IPointerClickHandler {

    public static event Action<MENUTYPE> OnOpenMenu;

    private Dictionary<string, Action<string>> eventsDictionary = new Dictionary<string, Action<string>>();

    private void Awake() {
        eventsDictionary.Add("openCase", OpenCaseSummary);
        eventsDictionary.Add("openProfile", OpenCaseProfile);
        eventsDictionary.Add("openEvidence", OpenEvidence);
        eventsDictionary.Add("openLeads",OpenLeads);
        eventsDictionary.Add("openMap",OpenMap);
        eventsDictionary.Add("openVictim", OpenVictimProfile);
    }

    public void OnPointerClick(PointerEventData eventData) {
        TMP_Text pTextMeshPro = GetComponent<TMP_Text>();
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, null);  // If you are not in a Canvas using Screen Overlay, put your camera instead of null
        if (linkIndex != -1) { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
            // Application.OpenURL(linkInfo.GetLinkID());
            Debug.Log("link clicked: " + linkInfo.GetLinkID());
            CallEventText(linkInfo.GetLinkID());
        }
    }

    private void CallEventText(string linkText)
    {
        string[] splitText = linkText.Split('.');
        if(splitText.Length == 2)
        {
            ProcessEvent(splitText[0],splitText[1]);
        }
        else
        {
            Debug.LogError("Tried to process link event text but got incorrect number of array entries. Entries count: " + splitText.Length);
        }
    }

    private void ProcessEvent(string eventName, string eventValue)
    {
        if(eventsDictionary.ContainsKey(eventName))
        {
            eventsDictionary[eventName](eventValue);
        }
        else
        {
            Debug.LogError("LINK MANAGER ---- Attempted to process event for key but missing key: " + eventName);
        }
    }

    // EVENTS

    private void OpenCaseSummary(string notused)
    {
        Debug.Log("LINK MANAGER - OPENING CASE SUMMARY SCREEN. ");
        OnOpenMenu?.Invoke(MENUTYPE.CASEMENU);
        CaseMenu.Instance.OpenCaseSummaryScreen();
    }

    private void OpenVictimProfile(string victimName)
    {
        Debug.Log("LINK MANAGER - OPENING CASE PROFILE FOR: " + victimName);
        OnOpenMenu?.Invoke(MENUTYPE.CASEMENU);
        CaseMenu.Instance.OpenVictimDataScreen();
    }

    private void OpenCaseProfile(string characterID)
    {
        Debug.Log("LINK MANAGER - OPENING CASE PROFILE FOR: " + characterID);
        OnOpenMenu?.Invoke(MENUTYPE.CASEMENU);
        CaseMenu.Instance.GoToCharacterProfileByID(characterID);
    }

    private void OpenEvidence(string evidenceName)
    {
        Debug.Log("LINK MANAGER - OPENING EVIDENCE PROFILE FOR: " + evidenceName);
        OnOpenMenu?.Invoke(MENUTYPE.CASEMENU);
        CaseMenu.Instance.OpenEvidenceScreen();
    }

    private void OpenLeads(string leadID)
    {
        Debug.Log("LINK MANAGER - OPENING LEADS MENU FOR LEADID: " + leadID);
        OnOpenMenu?.Invoke(MENUTYPE.CASEMENU);
        CaseMenu.Instance.OpenLeadsScreen();

    }

    private void OpenMap(string areaName)
    {
        Debug.Log("LINK MANAGER - OPENING MAP FOR AREA NAME: " + areaName);
        OnOpenMenu?.Invoke(MENUTYPE.MAINMENU);
        MainMenu.Instance.OpenMapScreen(areaName);
    }

}
