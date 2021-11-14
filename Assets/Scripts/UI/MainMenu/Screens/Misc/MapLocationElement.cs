using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MapLocationElement : MonoBehaviour, IPointerClickHandler
{
    
    public static event Action<MapLocationObject> onElementTapped;

    private MapLocationObject _locationData;


    public void UpdateData(MapLocationObject locationData)
    {
        _locationData = locationData;
        var offsetX = _locationData.GetOffsetX();
        var offsetY = _locationData.GetOffSetY();
        var mapIcon = this.transform.Find("mapIcon").GetComponent<Image>();
        var iconFrame = this.GetComponent<Image>();
        this.transform.localPosition = new Vector3(offsetX,offsetY,0);
        mapIcon.sprite = locationData.GetIcon();

        // set icon color based on whether current location or not
        var currentLocationColor = globalConfig.UI.toggleSelectedBackgroundColor;
        var notCurrentLocationColor = globalConfig.UI.toggleInactiveBackgroundColor;
        var currentAreaName = Player.Instance.CurrentAreaName;
        var colorToSet = currentAreaName == locationData.GetAreaName() ? currentLocationColor : notCurrentLocationColor;
        
        mapIcon.color = colorToSet;
        iconFrame.color = colorToSet;

        // set 'current' header if this is current location
        this.transform.Find("currentPanel").gameObject.SetActive(currentAreaName == locationData.GetAreaName());
        

    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        onElementTapped?.Invoke(_locationData);
    }
}
