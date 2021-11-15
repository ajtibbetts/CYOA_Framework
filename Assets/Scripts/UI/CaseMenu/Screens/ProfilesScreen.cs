using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CaseDataObjects;
using UnityEngine.Events;

public class ProfilesScreen : CaseScreen
{
    [Header("Scroll Content")]
    [SerializeField] private Scrollbar _vertScrollBar;
    [SerializeField] private GameObject _contentScrollContainer;
    [SerializeField] private GameObject _characterProfilePrefab;

    [Header("Profile Screens")]
    [SerializeField] private GameObject _profileMainScreen;
    [SerializeField] private GameObject _profileDetailsScreen;
    [SerializeField] private Button _goBackButton;

    [Header("Profile Details")]
    [SerializeField] Image _characterPortrait;
    [SerializeField] TextMeshProUGUI _characterNameText;
    [SerializeField] TextMeshProUGUI _characterAgeText;
    [SerializeField] TextMeshProUGUI _characterResidenceText;
    [SerializeField] TextMeshProUGUI _characterOccupationText;
    [SerializeField] TextMeshProUGUI _characterRelationshipText;
    [SerializeField] TextMeshProUGUI _characterSummaryText;

    private List<CharacterProfileData> _profiles = new List<CharacterProfileData>();
    private List<GameObject> _contentUIObjects = new List<GameObject>();

    private CharacterProfileData _activeProfileData;
    
    private void Start() {
        ProfileEntryElement.onElementTapped += OpenProfileDetailsScreen;
    }

    public void resetContent(){
        _profileDetailsScreen.SetActive(false);
        _vertScrollBar.value = 1;
        foreach(GameObject uiObject in _contentUIObjects)
        {
            Destroy(uiObject.gameObject);
        }

        _contentUIObjects.Clear();
    }

    public void AddEmptyEntry()
    {

    }

    public void AddToContent(CharacterProfileData profileData)
    {
        GameObject profileToAdd = GameObject.Instantiate(_characterProfilePrefab, Vector3.zero, Quaternion.identity, _contentScrollContainer.transform);
        profileToAdd.GetComponent<ProfileEntryElement>().SetData(profileData);

        var portraitImageObject = profileToAdd.transform.Find("portraitMask/portraitImage").GetComponent<Image>();
        var nameTextObject = profileToAdd.transform.Find("profileNameText").GetComponent<TextMeshProUGUI>();
        var relationshipTextObject = profileToAdd.transform.Find("relationshipText").GetComponent<TextMeshProUGUI>();

        // Sprite portraitSprite;
        // if(profileData.portrait == null) portraitSprite = _unknownPortraitSprite;
        // else portraitSprite = profileData.portrait.portraitSprite;

        var portraitSprite = profileData.portrait.portraitSprite;
        var offsetX =profileData.portrait.thumbNailOffsetX + 150; // add 150 / 75 for anchor positioning offset
        var offsetY =profileData.portrait.thumbNailOffsetY + 75;
        portraitImageObject.sprite = portraitSprite;
        SetPortraitThumbnail(portraitImageObject, portraitSprite,offsetX,offsetY);

        nameTextObject.text = profileData.characterName;
        relationshipTextObject.text = profileData.relationshipToVictim;

        _contentUIObjects.Add(profileToAdd);
    }

    public override void UpdateData()
    {
        resetContent();
        _profiles = _caseRecord.GetProfiles();
        if(_profiles.Count < 1) AddEmptyEntry();
        else 
        {
            foreach(CharacterProfileData profileData in _profiles)
            {
                AddToContent(profileData);
            }
        }

        Debug.Log("Profile Screen Data Updated");
    }


    public void OpenProfileDetailsScreen(CharacterProfileData profileData)
    {
        if(!_isActiveScreen) return;
        
        _characterPortrait.sprite =profileData.portrait.portraitSprite;
        _characterNameText.text = profileData.characterName;
        _characterAgeText.text = "<b>Age:</b> " + profileData.age;
        _characterResidenceText.text = "<b>Residence:</b> " + profileData.residence;
        _characterOccupationText.text = "<b>Occupation:</b> " +  profileData.occupation;
        _characterRelationshipText.text = "<b>Relationship to Victim:</b>\n" + profileData.relationshipToVictim;
        _characterSummaryText.text = "<b>Summary:</b>\n" + profileData.summary;

        SetBackgroundColor(profileData.characterType);

        _activeProfileData = profileData;
        _profileDetailsScreen.SetActive(true);
    }

    public void CloseProfileDetailsScreen()
    {
        UpdateData();
        _profileDetailsScreen.SetActive(false);
    }

    public void SetBackgroundColor(CharacterType charType)
    {
        switch(charType)
        {
            case CharacterType.ALLY:
                _profileDetailsScreen.GetComponent<Image>().color = globalConfig.UI.CaseUI.profileAllyBackgroundColor;
            break;
            case CharacterType.NEUTRAL:
                _profileDetailsScreen.GetComponent<Image>().color = globalConfig.UI.CaseUI.profileNeutralBackgroundColor;
            break;
            case CharacterType.SUSPECT:
                _profileDetailsScreen.GetComponent<Image>().color = globalConfig.UI.CaseUI.profileSuspectBackgroundColor;
            break;
        }
    }

    public void ToggleSuspectFlag()
    {
        if(_activeProfileData.characterType == CharacterType.NEUTRAL)
        {
            _caseRecord.AddProfileToSuspects(_activeProfileData);
            _activeProfileData.characterType = CharacterType.SUSPECT;
            
        }
        else if (_activeProfileData.characterType == CharacterType.SUSPECT)
        {
            _caseRecord.RemoveProfileFromSuspects(_activeProfileData);
            _activeProfileData.characterType = CharacterType.NEUTRAL;
        }

        OpenProfileDetailsScreen(_activeProfileData);
    }
}
