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
        this.transform.localPosition = new Vector3(offsetX,offsetY,0);
        mapIcon.sprite = locationData.GetIcon();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        onElementTapped?.Invoke(_locationData);
    }
}
