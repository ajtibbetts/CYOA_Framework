using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CaseDataObjects;

public class PlayerCaseRecord : MonoBehaviour
{
    private static PlayerCaseRecord _instance;
    public static PlayerCaseRecord Instance { get { return _instance; } }

    // events
    public static event Action<bool> OnCaseEnabled;
    public static event Action OnCaseDataUpdated;
    public static event Action<string> OnMessageToUI;
    public static event Action<string> OnLinkToUI;

    
    [ES3Serializable] public bool onActiveCase {get; private set;}
    public string activeCaseID;
    
    [SerializeField] private VictimData _victim;
    [SerializeField] private List<ActiveLead> _leads = new List<ActiveLead>();
    [SerializeField] private List<CharacterProfileData> _profiles = new List<CharacterProfileData>();
    [SerializeField] private List<CaseSuspect> _suspects = new List<CaseSuspect>();
    [SerializeField] private List<CaseSuspect> _inactiveSuspects = new List<CaseSuspect>();
    [SerializeField] private CaseSuspect _primarySuspect;
    [SerializeField] private List<CaseEvidence> _evidence = new List<CaseEvidence>();
    
    [SerializeField] private CaseEvidence _nullEvidence;
    [SerializeField] private List<string> _customNotes = new List<string>();

    [SerializeField] private List<ActiveLocation> _activeLocations = new List<ActiveLocation>();
    
    
    private bool _hasArrestWarrant;
    
    private void Awake() {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        AddListeners();
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        // to test, setup new case
        _primarySuspect = null;
        // StartNewCase(); // removed unless testing


        // setup test data for building UI.
        // SetTestData();
    }

    private void AddListeners()
    {
        CaseManager.OnNewCaseSet += StartNewCase;
        caseEvents.onVictimUpdated += DiscoverVictimData;
        caseEvents.onProfileAdded += AddProfile;
        caseEvents.onProfileUpdated += DiscoverProfileData;
        caseEvents.onEvidenceAdded += AddEvidenceByID;
        caseEvents.onEvidenceRemoved += RemoveEvidenceByID;
        caseEvents.onEvidenceChanged += UpgradeEvidenceByID;
        caseEvents.onLeadAdded += AddLeadByID;
        caseEvents.onLeadResolved += ResolveLeadByID;
        caseEvents.onLocationDiscovered += DiscoverLocation;
        caseEvents.onLocationStatusUpdated += UpdateLocationStatus;
    }

    public void LoadSaveData(PlayerCaseRecord savedRecord)
    {
        _instance = savedRecord;
        OnCaseDataUpdated?.Invoke();
    }

    void SetTestData()
    {
        List<string> testCharacterProfiles = new List<String>();
        // testCharacterProfiles.Add("Chad Billingsworth");
        testCharacterProfiles.Add("pierreL");
        // testCharacterProfiles.Add("chadB");
        // testCharacterProfiles.Add("Johnny Cyber");
        foreach(string name in testCharacterProfiles)
        {
            AddProfile(name);
        }

        // AddProfileToSuspects(_profiles[0]);
        // AddProfileToSuspects(_profiles[1]);
        // AddProfileToSuspects(_profiles[2]);
        // SetPrimarySuspect(2);
        // UpdateSuspectProfile(_primarySuspect,"means",_evidence[0]);
        // UpdateSuspectProfile(_primarySuspect,"motive",_evidence[1]);
        // UpdateSuspectProfile(_primarySuspect,"opportunity",_evidence[2]);

    }

    // getters
    public VictimData GetVictim()
    {
        return _victim;
    }

    public List<ActiveLead> GetLeads()
    {
        return _leads;
    }

    public List<CharacterProfileData> GetProfiles()
    {
        return _profiles;
    }

    public List<CaseSuspect> GetSuspects()
    {
        return _suspects;
    }

    public CaseSuspect GetPrimarySuspect()
    {
        return _primarySuspect;
    }

    public List<CaseEvidence> GetEvidence()
    {
        return _evidence;
    }

    public List<ActiveLocation> GetActiveLocations()
    {
        return _activeLocations;
    }

    public List<string> GetNotes()
    {
        return _customNotes;
    }


    // case functionality
    void StartNewCase()
    {
        // Debug.Log("Starting new case.");
        ClearCollectionsForNewCase(); // remove when testing with starting test data

        // get starting victim data, starting profiles, leads, and evidence
        activeCaseID = CaseManager.Instance.GetActiveCaseID();
        _victim = CaseManager.Instance.GetStartingVictimData();
        _leads.AddRange(CaseManager.Instance.GetStartingLeads());
        _profiles.AddRange(CaseManager.Instance.GetStartingCharacterProfiles());
        _evidence.AddRange(CaseManager.Instance.GetStartingEvidence());

        // get starting locations
        SetStartingLocations();

        onActiveCase = true;
        OnCaseEnabled?.Invoke(true);
        OnMessageToUI?.Invoke("New Case Started.");
        OnLinkToUI?.Invoke("openCase.null");
        OnCaseDataUpdated?.Invoke();
    }

    void ClearCollectionsForNewCase()
    {
        _leads.Clear();
        _profiles.Clear();
        _suspects.Clear();
        _inactiveSuspects.Clear();
        _primarySuspect = null;
        _evidence.Clear();
        _customNotes.Clear();
        _hasArrestWarrant = false;
    }


    // these will be called by external event we'll subscribe to from event manager
    private void DiscoverVictimData(string propertyName)
    {
        if(propertyName == "portrait")
        {
            _victim.portrait = CaseManager.Instance.UncoverVictimPortrait();
            OnMessageToUI?.Invoke("Victim's Portrait added to Victim Profile.");
            OnLinkToUI?.Invoke("openVictim." + _victim.name);
            OnCaseDataUpdated?.Invoke();
            return;
        }
        else
        {
            FieldInfo fieldInfo = _victim.GetType().GetField(propertyName, BindingFlags.Instance  | BindingFlags.Public);
            string DiscoveredValue = CaseManager.Instance.UncoverVictimProperty(propertyName);
            fieldInfo.SetValue(_victim, Convert.ChangeType(DiscoveredValue, fieldInfo.FieldType));
            
            // PropertyInfo propertyInfo = _victim.GetType().GetProperty(propertyName);
            // string DiscoveredValue = CaseManager.Instance.UncoverVictimProperty(propertyName);
            // propertyInfo.SetValue(_victim, Convert.ChangeType(DiscoveredValue, propertyInfo.PropertyType), null);
            OnMessageToUI?.Invoke($"Added {_victim.name}'s {propertyName} to Case Profile.");
            OnLinkToUI?.Invoke("openVictim." + _victim.name);
            OnCaseDataUpdated?.Invoke();
            return;
        }
    }

    // LEADS
    private void AddLeadByID(string leadID)
    {
        Debug.Log("adding lead by ID: " + leadID);
        var availableLeads = CaseManager.Instance.GetAvailableLeads();
        var leadToAdd = availableLeads.Find(x => x.GetLeadID() == leadID);
        if(leadToAdd != null)
        {
            ActiveLead newLead= new ActiveLead();
            newLead.lead = leadToAdd;
            newLead.isResolved = false;
            _leads.Add(newLead);
            OnMessageToUI?.Invoke("New Case Lead Added.");
            OnLinkToUI?.Invoke("openLeads."+ leadToAdd.GetLeadID());
            OnCaseDataUpdated?.Invoke();
        }
        else
        {
            Debug.LogError("PLAYER CASE RECORD ---- Failed to add lead by ID: " + leadID);
        }
    }

    private void ResolveLeadByID(string leadID)
    {
        var matchedLead = _leads.Find(x => x.lead.GetLeadID() == leadID);
        if(matchedLead != null)
        {            
            matchedLead.isResolved = true;
            OnMessageToUI?.Invoke("Case Lead Resolved.");
            OnLinkToUI?.Invoke("openLeads."+ leadID);
            OnCaseDataUpdated?.Invoke();
        }
        else
        {
            Debug.LogError("PLAYER CASE RECORD ---- Failed to resolve lead by ID: " + leadID);
        }
    }

    public bool isLeadInRecord(string leadID)
    {
        return _leads.Exists(x => x.lead.GetLeadID() == leadID);
    }

    public bool isLeadResolved(string leadID)
    {
        var matchedLead = _leads.Find(x => x.lead.GetLeadID() == leadID);
        if(matchedLead != null) return matchedLead.isResolved;
        else 
        {
            Debug.LogError("CASE RECORD ---- Lead ID not found: " + leadID);
            return false;
        }
    }

    // PROFILES

    private void AddProfile(string characterID)
    {
        CharacterProfileData _newCharacterData = CaseManager.Instance.GetStartingCharacterProfileData(characterID);
        // only add if not already in list
        if(!_profiles.Exists(x => x.characterID.ToLower() == characterID.ToLower()))
        {
            _profiles.Add(_newCharacterData);
            OnMessageToUI?.Invoke("New Character Profile Added: " + _newCharacterData.characterName);
            OnLinkToUI?.Invoke("openProfile."+_newCharacterData.characterID);
            OnCaseDataUpdated?.Invoke();
        }
    }

    private void DiscoverProfileData(string characterID, string propertyName)
    {
        Debug.Log($"Discovering profile data for {characterID}.{propertyName}");
        var characterToUpdate = _profiles.Find(x => x.characterID == characterID);
        if(characterToUpdate != null)
        {
            if(propertyName == "portrait")
            {
                characterToUpdate.portrait = CaseManager.Instance.UncoverCharacterPortrait(characterID);
                OnMessageToUI?.Invoke("Character Profile Portrait Uncovered: " + characterToUpdate.characterName);
                OnLinkToUI?.Invoke("openProfile."+characterToUpdate.characterID);
                OnCaseDataUpdated?.Invoke();
                return;
            }
            else
            {
                FieldInfo fieldInfo = characterToUpdate.GetType().GetField(propertyName, BindingFlags.Instance  | BindingFlags.Public);
                string DiscoveredValue = CaseManager.Instance.UncoverCharacterProperty(characterID,propertyName);
                fieldInfo.SetValue(characterToUpdate, Convert.ChangeType(DiscoveredValue, fieldInfo.FieldType));
                if(propertyName == "characterName")
                {
                     OnMessageToUI?.Invoke($"Uncovered mystery profile name: {characterToUpdate.characterName}.");
                }
                else if (propertyName == "relationshipToVictim")
                {
                    OnMessageToUI?.Invoke($"Added {characterToUpdate.characterName}'s relationship to the victim to Case Profile.");
                }
                else 
                {
                    OnMessageToUI?.Invoke($"Added {characterToUpdate.characterName}'s {propertyName} to Case Profile.");
                }
                OnLinkToUI?.Invoke("openProfile."+characterToUpdate.characterID);
                OnCaseDataUpdated?.Invoke();
                return;
            }
        }
        else Debug.LogError("CASE RECORD ---- FAILED TO DISCOVER PROFILE DATA.");
    }

    public CharacterProfileData GetProfileByID(string characterID)
    {
        var matchedProfile = _profiles.Find(x => x.characterID.ToLower() == characterID.ToLower());
        if(matchedProfile != null) return matchedProfile;
        else
        {
            Debug.LogError("CASE RECORD ---- Unable to find profile by ID: " + characterID);
            return null;
        }
    }

    // SUSPECTS
    public void AddProfileToSuspects(CharacterProfileData characterProfile)
    {
        // first update character profile to suspect
        var characterToUpdate = _profiles.Find(x => x.characterID == characterProfile.characterID);
        characterToUpdate.characterType = CharacterType.SUSPECT;

        // second check if suspect entry already exists in inactive
        // Debug.Log("this profile is alraedy inactive suspect: " + isProfileAlreadyInactiveSuspect(characterProfile));
        if(isProfileAlreadyInactiveSuspect(characterProfile))
        {
            _suspects.Add(GetInactiveSuspect(characterProfile));
        }
        else
        {
            CaseSuspect newSuspect = new CaseSuspect();
            newSuspect.SuspectProfile = characterProfile;
            UpdateSuspectProfile(newSuspect,null, EvidenceType.MEANS);
            UpdateSuspectProfile(newSuspect,null, EvidenceType.MOTIVE);
            UpdateSuspectProfile(newSuspect,null, EvidenceType.OPPORTUNITY);
            _suspects.Add(newSuspect);
        }

        
        if(_suspects.Count == 1) SetPrimarySuspect(0);
    }

    public void RemoveProfileFromSuspects(CharacterProfileData characterProfile)
    {
        var characterToUpdate = _profiles.Find(x => x.characterID == characterProfile.characterID);
        var suspectToRemove = _suspects.Find(x => x.SuspectProfile.characterID == characterProfile.characterID);
        
        // update character profile back to neutral
        characterToUpdate.characterType = CharacterType.NEUTRAL;

        // remove suspect from list and set primary as needed
        _suspects.Remove(suspectToRemove);
        if(_primarySuspect == suspectToRemove && _suspects.Count < 1) _primarySuspect = null;
        else if (_primarySuspect == suspectToRemove) SetPrimarySuspect(0);

        // finally add suspect to inactive list to retrieve if used again.
        _inactiveSuspects.Add(suspectToRemove);
    }

    private bool isProfileAlreadyInactiveSuspect(CharacterProfileData characterProfile)
    {
        return _inactiveSuspects.Find(x => x.SuspectProfile.characterID == characterProfile.characterID) != null;
    }

    private CaseSuspect GetInactiveSuspect(CharacterProfileData characterProfile)
    {
        var _suspect = _inactiveSuspects.Find(x => x.SuspectProfile.characterID == characterProfile.characterID);
        _inactiveSuspects.Remove(_suspect);
        return _suspect;
    }

    public void UpdateSuspectProfile(CaseSuspect suspect, CaseEvidence evidence, EvidenceType type)
    {
        if(evidence == null) evidence = _nullEvidence;
        switch(type)
        {
            case EvidenceType.MEANS:
                suspect.ProposedMeans = evidence;
            break;
            case EvidenceType.MOTIVE:
                suspect.ProposedMotive = evidence;
            break;
            case EvidenceType.OPPORTUNITY:
                suspect.ProposedOpportunity = evidence;
            break;
            default:
            break;
        }
    }

    public EvidenceType GetEvidenceAssignmentTypeOnSuspect(CaseSuspect suspect, CaseEvidence evidence)
    {
        if(evidence == suspect.ProposedMeans) return EvidenceType.MEANS;
        if(evidence == suspect.ProposedMotive) return EvidenceType.MOTIVE;
        if(evidence == suspect.ProposedOpportunity) return EvidenceType.OPPORTUNITY;
        return EvidenceType.UNASSIGNED;
    }

    public CaseEvidence GetEvidenceOnSuspectByType(CaseSuspect suspect, EvidenceType type)
    {
        switch(type){
            case EvidenceType.MEANS:
                return suspect.ProposedMeans;
            case EvidenceType.MOTIVE:
                return suspect.ProposedMotive;
            case EvidenceType.OPPORTUNITY:
                return suspect.ProposedOpportunity;
        }

        return _nullEvidence;
    }

    public void SetPrimarySuspect(int index)
    {
        _primarySuspect = _suspects[index];
        // update suspect lists so primary is always first entry
        _suspects.RemoveAt(index);
        _suspects.Insert(0,_primarySuspect);
    }
    
    public bool isCharacterASuspect(string characterID)
    {
        // first check if primary
        if(_primarySuspect.SuspectProfile.characterID.ToLower() == characterID.ToLower()) return true;
        else
        {
            var matchedSuspect = _suspects.Find(x => x.SuspectProfile.characterID.ToLower() == characterID.ToLower());
            if (matchedSuspect != null) return true;
            else return false;
        }
    }

    // EVIDENCE
    private void AddEvidenceByID(string evidenceID)
    {
        var availableEvidence = CaseManager.Instance.GetAvailableEvidence();
        var evidenceToAdd = availableEvidence.Find(x => x.GetEvidenceID().ToLower() == evidenceID.ToLower());
        if(!_evidence.Exists(x => x.GetEvidenceID().ToLower() == evidenceID.ToLower()))
        {
            _evidence.Add(evidenceToAdd);
            OnMessageToUI?.Invoke($"Added {evidenceToAdd.GetEvidenceName()} to Evidence.");
            OnLinkToUI?.Invoke("openEvidence."+ evidenceToAdd.GetEvidenceName());
            OnCaseDataUpdated?.Invoke();
        }
        else Debug.LogError("PLAYER CASE RECORD ---- FAILED TO ADD EVIDENCE BY ID: " + evidenceID);
    }

    private void RemoveEvidenceByID(string evidenceID)
    {
        var availableEvidence = CaseManager.Instance.GetAvailableEvidence();
        var evidenceToRemove = _evidence.Find(x => x.GetEvidenceID().ToLower() == evidenceID.ToLower());
        if(evidenceToRemove != null)
        {
            _evidence.Remove(evidenceToRemove);
            OnMessageToUI?.Invoke($"Removed {evidenceToRemove.GetEvidenceName()} from Evidence.");
            OnLinkToUI?.Invoke("openEvidence."+ evidenceToRemove.GetEvidenceName());
            OnCaseDataUpdated?.Invoke();
        }
        else Debug.LogError("PLAYER CASE RECORD ---- FAILED TO REMOVE EVIDENCE BY ID: " + evidenceID);
    }

    private void UpgradeEvidenceByID(string oldEvidenceID, string newEvidenceID)
    {
        var availableEvidence = CaseManager.Instance.GetAvailableEvidence();
        var evidenceToRemoveIndex = _evidence.FindIndex(x => x.GetEvidenceID().ToLower() == oldEvidenceID.ToLower());
        var evidenceToAdd = availableEvidence.Find(x => x.GetEvidenceID().ToLower() == newEvidenceID.ToLower());
        if(evidenceToRemoveIndex >= 0 && evidenceToAdd != null)
        {
            _evidence[evidenceToRemoveIndex] = evidenceToAdd;
            // _evidence.Remove(evidenceToRemove);
            // _evidence.Add(evidenceToAdd);
            OnMessageToUI?.Invoke($"Updated {evidenceToAdd.GetEvidenceName()} in Evidence.");
            OnLinkToUI?.Invoke("openEvidence."+ evidenceToAdd.GetEvidenceName());
            OnCaseDataUpdated?.Invoke();
        }
        else Debug.LogError($"PLAYER CASE RECORD ---- FAILED TO UPGRADE EVIDENCE IDs (OLD:{oldEvidenceID}/NEW:{newEvidenceID})");
    }

    public bool isEvidenceInRecord(string evidenceID)
    {
        return _evidence.Exists(x => x.GetEvidenceID() == evidenceID);
    }

    // LOCATIONS
    private void SetStartingLocations()
    {
        var locations = CaseManager.Instance.GetMapLocations();
        foreach(MapLocationObject location in locations)
        {
            if(location.IsAvailableOnStart())
            {
                var newLocation = new ActiveLocation(location.GetAreaName(), LocationStatus.AVAILABLE);
                _activeLocations.Add(newLocation);
            }
        }
    }

    public bool isLocationActive(string areaName)
    {
        var location = _activeLocations.Find(x => x.MapLocationAreaName == areaName);
        if(location != null) return true;
        else return false;
    }

    public LocationStatus GetActiveLocationStatus(string areaName)
    {
        var location = _activeLocations.Find(x => x.MapLocationAreaName == areaName);
        if(location != null) return location.LocationStatus;
        else 
        {
            Debug.LogError("PLAYER CASE RECORD ---- FAILED TO GET LOCATION STATUS FOR AREA: " + areaName);
            return LocationStatus.UNDISCOVERED;
        }
    }

    private void DiscoverLocation(string areaName)
    {
        var locations = CaseManager.Instance.GetMapLocations();
        var locationToAdd = locations.Find(x => x.GetAreaName().ToLower() == areaName.ToLower());
        if(locationToAdd != null)
        {
            var newLocation = new ActiveLocation(locationToAdd.GetAreaName(), LocationStatus.AVAILABLE);
            if(!_activeLocations.Exists(x => x.MapLocationAreaName == newLocation.MapLocationAreaName))
            {
                _activeLocations.Add(newLocation);
                OnMessageToUI?.Invoke("New Location Discovered: " + newLocation.MapLocationAreaName);
                OnLinkToUI?.Invoke("openMap."+ newLocation.MapLocationAreaName);
                OnCaseDataUpdated?.Invoke();
            }
        }
        else 
        {
            Debug.LogError("PLAYER CASE RECORD ---- FAILED TO DISCOVER LOCATION BY AREA NAME: " + areaName);
        }
    }

    private void UpdateLocationStatus(string areaName, LocationStatus status)
    {
        var locationToUpdate = _activeLocations.Find(x => x.MapLocationAreaName.ToLower() == areaName.ToLower());
        if(locationToUpdate != null)
        {
            locationToUpdate.LocationStatus = status;
            OnMessageToUI?.Invoke($"Location Now {status.ToString()}: " + locationToUpdate.MapLocationAreaName);
            OnLinkToUI?.Invoke("openMap."+ locationToUpdate.MapLocationAreaName);
            OnCaseDataUpdated?.Invoke();
        }
        else Debug.LogError("PLAYER CASE RECORD ---- FAILED TO UPDATE LOCATION STATUS: " + areaName);
    }
    
    private void AddCustomNote(string note)
    {
        _customNotes.Add(note);
    }
    

    public void GrantWarrant()
    {
        Debug.Log("CASE RECROD ---- WARRANT GRANTED.");
        _hasArrestWarrant = true;
        
        OnMessageToUI?.Invoke($"Arrest Warrant for {_primarySuspect.SuspectProfile.characterName} added to Case Profile.");
        OnLinkToUI?.Invoke("openCase.null");
        OnCaseDataUpdated?.Invoke();
    }

    public bool hasArrestWarrant()
    {
        return _hasArrestWarrant;
    }
}
