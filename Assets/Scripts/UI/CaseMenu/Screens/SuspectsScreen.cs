using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CaseDataObjects;

public class SuspectsScreen : CaseScreen
{
    public event Action<CharacterProfileData> onGoToProfile;
    
    [SerializeField] private GameObject _activeSuspectsContainer;
    [SerializeField] private GameObject _noSuspectsContainer;

    [Header("Suspect View")]
    [SerializeField] private TextMeshProUGUI _suspectHeaderText;
    [SerializeField] private TextMeshProUGUI _suspectNameText;
    [SerializeField] private Image _suspectPortrait;
    [SerializeField] private TextMeshProUGUI _suspectRelationshipText;
    [SerializeField] private GameObject _meansContainer;
    [SerializeField] private GameObject _motiveContainer;
    [SerializeField] private GameObject _opportunityContainer;
    [SerializeField] private GameObject _primaryButton;

    [SerializeField] private GameObject _navPrevImage;
    [SerializeField] private GameObject _navNextImage;

    [SerializeField] private CaseEvidence _nullEvidenceScriptableObject;

    [Header("Add Evidence")]
    [SerializeField] private GameObject _addEvidenceContainer;
    [SerializeField] private Scrollbar _vertScrollBarEvidence;
    [SerializeField] private GameObject _contentScrollContainerEvidence;
    [SerializeField] private GameObject _evidenceEntryPrefab;
    [SerializeField] private TextMeshProUGUI _evidenceContainerTitle;

    


    [Header("Add Suspects")]
    [SerializeField] private GameObject _addSuspectContainer;

    [SerializeField] private GameObject _addSuspectsButton;
    [SerializeField] private GameObject _cancelAddSuspectsButton;
    [SerializeField] private TextMeshProUGUI _addSuspectsTitleText;

    private string _potentialSuspectsText = "Tap a profile from people you've encountered to add them as a suspect.";
    private string _noPotentialSuspectsText = "No potential suspects available. (Allies cannot be added).";
    private string _allPotentialSuspectsAddedText = "All potential suspects have already been added.";

    [Header("Add Suspects Scroll")]
    [SerializeField] private Scrollbar _vertScrollBar;
    [SerializeField] private GameObject _contentScrollContainer;
    [SerializeField] private GameObject _characterProfilePrefab;


    private List<GameObject> _contentProfileUIObjects = new List<GameObject>();
    private List<GameObject> _contentEvidenceUIObjects = new List<GameObject>();

    private CharacterProfileData _activeSuspectProfile;
    
    // Start is called before the first frame update
    void Start()
    {
        ProfileEntryElement.onElementTapped += AddSelectedToSuspects;
        SuspectEvidenceElement.onElementTapped += OpenEvidenceScreen;
        EvidenceEntryElement.onElementTapped += AddOrUpdateEvidence;
    }

    public void resetContent(){
        _addSuspectContainer.SetActive(false);
        _cancelAddSuspectsButton.SetActive(false);
        _addSuspectsButton.SetActive(true);
        _addEvidenceContainer.SetActive(false);
        
        ClearUIList(_contentProfileUIObjects,_vertScrollBar);
    
    }

    public void ClearUIList(List<GameObject> uiList, Scrollbar scrollbar)
    {
        scrollbar.value = 1;
        foreach(GameObject uiObject in uiList)
        {
            Destroy(uiObject.gameObject);
        }
        uiList.Clear();
    }

    
    public override void UpdateData()
    {
        resetContent();
        var _suspects = _caseRecord.GetSuspects();
        // if no suspects, show inactive screen
        if(_suspects.Count == 0)
        {
            _activeSuspectsContainer.SetActive(false);
            _noSuspectsContainer.SetActive(true);
            return;
        }

        // otherwise activate and continue
        _activeSuspectsContainer.SetActive(true);
        // if no active suspect data is set, load primary
        if(_activeSuspectProfile == null) UpdateSuspectView(_caseRecord.GetPrimarySuspect());
        else
        {
            var suspectToUpdate = _suspects.Find(x => x.SuspectProfile == _activeSuspectProfile);
            UpdateSuspectView(suspectToUpdate);
        }
        
    }

    /***
    /
    /
    / SUSPECTS VIEW
    /
    ***/

    public void UpdateSuspectView(CaseSuspect suspectData)
    {
        var _suspects = _caseRecord.GetSuspects();
        var suspectName = suspectData.SuspectProfile.characterName;
        
        // first check if primary and update title
        if(suspectName == _caseRecord.GetPrimarySuspect().SuspectProfile.characterName)
        {
            _suspectHeaderText.text = "Primary Suspect";
            _primaryButton.SetActive(false);
            _navPrevImage.SetActive(false);
            Debug.Log("current suspects count: " + _suspects.Count);
            if(_suspects.Count > 1) _navNextImage.SetActive(true);
            else _navNextImage.SetActive(false);
        }
        else {
            _suspectHeaderText.text = "Suspect";
            _primaryButton.SetActive(true);
            _navPrevImage.SetActive(true);

            // check if last suspect in list
            if(suspectName == _suspects[_suspects.Count - 1].SuspectProfile.characterName)
            {
                _navNextImage.SetActive(false);
            }
            else _navNextImage.SetActive(true);
            
        }

        _suspectPortrait.sprite = suspectData.SuspectProfile.portrait.portraitSprite;

        _suspectNameText.text = suspectName;
        _suspectRelationshipText.text = suspectData.SuspectProfile.relationshipToVictim;

        UpdateEvidenceContainer(_meansContainer, suspectData.ProposedMeans, EvidenceType.MEANS);
        UpdateEvidenceContainer(_motiveContainer, suspectData.ProposedMotive, EvidenceType.MOTIVE);
        UpdateEvidenceContainer(_opportunityContainer, suspectData.ProposedOpportunity, EvidenceType.OPPORTUNITY);

        _activeSuspectProfile = suspectData.SuspectProfile;
    }

    public void UpdateEvidenceContainer(GameObject evidenceContainer, CaseEvidence evidenceData, EvidenceType type)
    {
        if(!_isActiveScreen) return;
        evidenceContainer.GetComponent<SuspectEvidenceElement>().UpdateData(evidenceData, type);
    }

    public void RemoveCurrentSuspect()
    {
        _caseRecord.RemoveProfileFromSuspects(_activeSuspectProfile);
        _activeSuspectProfile = null;
        UpdateData();
            // characterProfile.characterType = CharacterType.NEUTRAL;
    }

    public void NavToSuspect(int indexDirection)
    {
        var _suspects = _caseRecord.GetSuspects();
        
        Debug.Log("Nav to suspect direction:" + indexDirection);
        var currentSuspectIndex = _suspects.FindIndex(x => x.SuspectProfile.characterName == _activeSuspectProfile.characterName);
        Debug.Log("Current Suspect name: " + _activeSuspectProfile.characterName);
        var newSuspectIndex = currentSuspectIndex + indexDirection;
        Debug.Log("Trying to nav to suspect index:" + newSuspectIndex);
        UpdateSuspectView(_suspects[newSuspectIndex]);
    }

    public void SetCurrentSuspectAsPrimary()
    {
        var currentSuspectIndex = _caseRecord.GetSuspects().FindIndex(x => x.SuspectProfile.characterName == _activeSuspectProfile.characterName);
        _caseRecord.SetPrimarySuspect(currentSuspectIndex);
        UpdateData();
    }
    
    public void GotoActiveProfile()
    {
        UpdateData();
        onGoToProfile?.Invoke(_activeSuspectProfile);
    }

    /***
    /
    /
    / ADD / UPDATE EVIDENCE SCREEN
    /
    ***/

    public void OpenEvidenceScreen(EvidenceType type)
    {
        var _activeSuspect = _caseRecord.GetSuspects().Find(x => x.SuspectProfile.characterName == _activeSuspectProfile.characterName);
        var _evidence = _caseRecord.GetEvidence();
        foreach(CaseEvidence evidence in _evidence)
        {
            if(evidence.GetEvidenceName() == "None") continue;
            GameObject evidenceToAdd = GameObject.Instantiate(_evidenceEntryPrefab, Vector3.zero, Quaternion.identity, _contentScrollContainerEvidence.transform);
            
            evidenceToAdd.GetComponent<EvidenceEntryElement>().UpdateData(evidence, type);
            
            _contentEvidenceUIObjects.Add(evidenceToAdd);
        }

        switch(type)
        {
            case EvidenceType.MEANS:
                _evidenceContainerTitle.text = "Assign Evidence\nMEANS";
            break;
            case EvidenceType.MOTIVE:
                _evidenceContainerTitle.text = "Assign Evidence\nMOTIVE";
            break;
            case EvidenceType.OPPORTUNITY:
                _evidenceContainerTitle.text = "Assign Evidence\nOPPORTUNITY";
            break;
        }

        _addEvidenceContainer.SetActive(true);
    }

    public void AddOrUpdateEvidence(CaseEvidence evidenceData, EvidenceType type)
    {
        var _suspects = _caseRecord.GetSuspects();
        var _suspectToUpate = _suspects.Find(x => x.SuspectProfile.characterName == _activeSuspectProfile.characterName);
        GameObject evidenceContainer = GetEvidenceContainer(type);
        
        // check if evidence is already assigned to a slot and empty existing slot if needed
        var oldAssingmentType =_caseRecord.GetEvidenceAssignmentTypeOnSuspect(_suspectToUpate,evidenceData);
        if(oldAssingmentType != EvidenceType.UNASSIGNED)
        {
            GameObject oldContainer = GetEvidenceContainer(oldAssingmentType);
            UpdateEvidenceContainer(oldContainer, _nullEvidenceScriptableObject, oldAssingmentType);
            _caseRecord.UpdateSuspectProfile(_suspectToUpate,null,oldAssingmentType);
        }

        UpdateEvidenceContainer(evidenceContainer, evidenceData, type);
        _caseRecord.UpdateSuspectProfile(_suspectToUpate, evidenceData, type);
        
        // clear list and close
        ClearUIList(_contentEvidenceUIObjects, _vertScrollBarEvidence);
        _addEvidenceContainer.SetActive(false);
    }

    private GameObject GetEvidenceContainer(EvidenceType evidenceType)
    {
        switch(evidenceType)
        {
            case EvidenceType.MEANS:
                return _meansContainer;
            case EvidenceType.MOTIVE:
                return _motiveContainer;
            case EvidenceType.OPPORTUNITY:
                return _opportunityContainer;
        }

        return null;
    }

    /***
    /
    / 
    / ADD SUSPECTS SCREEN
    /
    ***/

    public void OpenAddSuspectScreen()
    {
        // first hide other screens
        _activeSuspectsContainer.SetActive(false);
        _noSuspectsContainer.SetActive(false);
        _addSuspectsButton.SetActive(false);

        _addSuspectContainer.SetActive(true);
        _cancelAddSuspectsButton.SetActive(true);

        _addSuspectsTitleText.text = _potentialSuspectsText;

        AddProfilesToScreen();
    }

    public void AddProfilesToScreen()
    {
        var _profiles = _caseRecord.GetProfiles().FindAll(x => x.characterType == CharacterType.NEUTRAL);
        if(_profiles.Count < 1) AddEmptyEntry();
        else 
        {
            foreach(CharacterProfileData profileData in _profiles)
            {
                AddToSuspectContent(profileData);
            }
        }
    }

    public void AddEmptyEntry()
    {
        if(_caseRecord.GetSuspects().Count == 0) _addSuspectsTitleText.text  = _noPotentialSuspectsText;
        else _addSuspectsTitleText.text = _allPotentialSuspectsAddedText;
    }

    public void CloseAddSuspectScreen()
    {
        UpdateData();
    }

    public void AddToSuspectContent(CharacterProfileData profileData)
    {
        GameObject profileToAdd = GameObject.Instantiate(_characterProfilePrefab, Vector3.zero, Quaternion.identity, _contentScrollContainer.transform);
        profileToAdd.GetComponent<ProfileEntryElement>().SetData(profileData);

        var portraitImageObject = profileToAdd.transform.Find("portraitMask/portraitImage").GetComponent<Image>();
        var nameTextObject = profileToAdd.transform.Find("profileNameText").GetComponent<TextMeshProUGUI>();
        var relationshipTextObject = profileToAdd.transform.Find("relationshipText").GetComponent<TextMeshProUGUI>();

        var portraitSprite = profileData.portrait.portraitSprite;
        var offsetX = profileData.portrait.thumbNailOffsetX + 150; // add 150 / 75 for anchor positioning offset
        var offsetY = profileData.portrait.thumbNailOffsetY + 75;
        portraitImageObject.sprite = portraitSprite;
        SetPortraitThumbnail(portraitImageObject, portraitSprite,offsetX,offsetY);

        nameTextObject.text = profileData.characterName;
        relationshipTextObject.text = profileData.relationshipToVictim;

        _contentProfileUIObjects.Add(profileToAdd);
    }

    public void AddSelectedToSuspects(CharacterProfileData characterProfile)
    {
        if(!_isActiveScreen) return;
        
        Debug.Log("Adding this character to suspects: " + characterProfile.characterName);
        _caseRecord.AddProfileToSuspects(characterProfile);
        // characterProfile.characterType = CharacterType.SUSPECT;
        UpdateData();
    }
}
