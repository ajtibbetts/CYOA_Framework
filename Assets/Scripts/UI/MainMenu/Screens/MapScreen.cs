using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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


    private MapLocationObject _confirmLocationData;
    
    // Start is called before the first frame update
    void Start()
    {
        MapLocationElement.onElementTapped += OpenConfirmLocationScreen;
    }

    public override void UpdateData()
    {
        if(_activeMap != null)
        {
            SetMapImage();
            SetMapName();
            SetupMapLocations();
            GoBacktoMainMapScreen();
        }
    }

    public void LoadMap(MapObject newMap)
    {
        Debug.Log("Loading new map!");
        _activeMap = newMap;
        UpdateData();
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
            GameObject newLocation = GameObject.Instantiate(_mapLocationPrefab, Vector3.zero, Quaternion.identity, _mapLocationsContainer.transform);
            newLocation.GetComponent<MapLocationElement>().UpdateData(locationData);
        }
    }

    public void OpenConfirmLocationScreen(MapLocationObject locationData)
    {
        _confirmLocationData = locationData;
        _confirmLocationName.text = locationData.GetDisplayName();
        _confirmLocationPortrait.sprite = locationData.GetPortrait();
        _confirmLocationDescription.text = locationData.GetDescription();

        // check if this location is current location
        if(_confirmLocationData.GetAreaName() == Player.Instance.CurrentAreaName)
        {
            _confirmButton.SetActive(false);
            _isCurrentLocationText.SetActive(true);
        }
        else
        {
            _confirmButton.SetActive(true);
            _isCurrentLocationText.SetActive(false);
        }
        
        _confirmLocationContainer.SetActive(true);
    }

    public void TravelToLocation()
    {
        Debug.Log("Traveling to location: " + _confirmLocationData.GetDisplayName());

        // add code here to load level on event call
        gameController.Instance.SwitchToLevel(_confirmLocationData.GetLevelName());
        // set area and close map screen
        Player.Instance.CurrentAreaName = _confirmLocationData.GetAreaName();
        GoBacktoMainMapScreen();
    }

    public void GoBacktoMainMapScreen()
    {
        _confirmLocationContainer.SetActive(false);
    }

}
