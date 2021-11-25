using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using globalDataTypes;

public class MapScreen : MenuScreen
{
    [Header("Main Map Screen")]
    [SerializeField] private MapObject _activeMap;
    [SerializeField] private Image _MapUIImage;

    [SerializeField] private TextMeshProUGUI _mapTitleText;
    [SerializeField] private GameObject _mapLocationsContainer;
    [SerializeField] private GameObject _mapLocationPrefab;

    [Header("Confirm Location Screen")]
    [SerializeField] private GameObject _confirmLocationContainer;
    [SerializeField] private TextMeshProUGUI _confirmLocationName;
    [SerializeField] private Image _confirmLocationPortrait;
    [SerializeField] private TextMeshProUGUI _confirmLocationDescription;
    [SerializeField] private GameObject _confirmButton;
    [SerializeField] private GameObject _isCurrentLocationText;
    [SerializeField] private GameObject _fastTravelDisabledText;
    [SerializeField] private GameObject _locationLockedText;

    private List<GameObject> _locationUIObjects = new List<GameObject>();


    private MapLocationObject _confirmLocationData;
    
    private void Awake() {
        PlayerCaseRecord.OnCaseDataUpdated += UpdateData;
    }

    // Start is called before the first frame update
    void Start()
    {
        MapLocationElement.onElementTapped += OpenConfirmLocationScreen;
        SetNoCaseScreen();
    }

    public override void UpdateData()
    {
        ResetScreen();
        if(_activeMap != null)
        {
            SetMapImage();
            SetMapName();
            SetupMapLocations();
            GoBacktoMainMapScreen();
        }
        else 
        {
            SetNoCaseScreen();
        }
    }

    public void ResetScreen()
    {
        foreach(GameObject UIObject in _locationUIObjects)
        {
            Destroy(UIObject.gameObject);
        }
        _locationUIObjects.Clear();
    }

    

    public void LoadMap(MapObject newMap)
    {
        Debug.Log("Loading new map!");
        _activeMap = newMap;
        UpdateData();
    }

    public void SetNoCaseScreen()
    {
        _mapTitleText.text = "Map Unavailable";
        _confirmLocationContainer.SetActive(false);
    }

    public void SetMapImage()
    {
        Debug.Log("Setting up new map screen for map: " + _activeMap.GetMapName());
        _MapUIImage.sprite = _activeMap.GetMapSprite();
        var offsetX = _activeMap.GetOffSetX();
        var offsetY = _activeMap.GetOffSetY();
        _MapUIImage.gameObject.transform.localPosition = new Vector3(offsetX,offsetY,0);
    }

    public void SetMapName()
    {
        _mapTitleText.text = _activeMap.GetMapName();
    }


    public void SetupMapLocations()
    {
        var _locations = _activeMap.GetLocations();
        foreach(MapLocationObject locationData in _locations)
        {
            if(PlayerCaseRecord.Instance.isLocationActive(locationData.GetAreaName()))
            {
                GameObject newLocation = GameObject.Instantiate(_mapLocationPrefab, Vector3.zero, Quaternion.identity, _mapLocationsContainer.transform);
                newLocation.GetComponent<MapLocationElement>().UpdateData(locationData);
                _locationUIObjects.Add(newLocation);
            }
        }
    }

    public void OpenConfirmLocationScreen(MapLocationObject locationData)
    {
        _confirmLocationData = locationData;
        _confirmLocationName.text = locationData.GetDisplayName();
        _confirmLocationPortrait.sprite = locationData.GetPortrait();
        _confirmLocationDescription.text = locationData.GetDescription();

        bool isLocked = PlayerCaseRecord.Instance.GetActiveLocationStatus(locationData.GetAreaName()) == CaseDataObjects.LocationStatus.LOCKED;

        _confirmButton.SetActive(false);
        _fastTravelDisabledText.SetActive(false);
        _isCurrentLocationText.SetActive(false);
        _locationLockedText.SetActive(false);


        // check if this location is current location or not world nav gamestate, or locked in the current story state
        if(_confirmLocationData.GetAreaName() == PlayerProgressTracker.Instance.CurrentAreaName) _isCurrentLocationText.SetActive(true);
        else if (isLocked) _locationLockedText.SetActive(true);
        else if(gameController.Instance.GetGAMESTATE() != GAMESTATE.WORLDNAVIGATION) _fastTravelDisabledText.SetActive(true);
        else _confirmButton.SetActive(true);

        
        _confirmLocationContainer.SetActive(true);
    }

    public void TravelToLocation()
    {
        Debug.Log("Traveling to location: " + _confirmLocationData.GetDisplayName());

        // add code here to load level on event call
        gameController.Instance.SwitchToLevel(_confirmLocationData.GetLevelName());
        // set area and close map screen & main menu
        PlayerProgressTracker.Instance.CurrentAreaName = _confirmLocationData.GetAreaName();
        UpdateData();
        CloseMenu(MENUTYPE.MAINMENU);
    }

    public void GoBacktoMainMapScreen()
    {
        _confirmLocationContainer.SetActive(false);
    }

}
