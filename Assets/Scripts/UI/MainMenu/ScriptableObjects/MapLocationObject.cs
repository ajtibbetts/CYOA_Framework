using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapLocationObject", menuName = "CYOA/Map/MapLocation", order = 0)]

public class MapLocationObject : ScriptableObject
{
    [SerializeField] private string _destinationLevelName;
    [SerializeField] private string _locationDisplayName;
    [SerializeField] private Sprite _displayIcon;
    [SerializeField] private Sprite _displayPortrait;

    [SerializeField] [TextArea] private string _locationDescription;

    [SerializeField] private float _displayOffsetX;
    [SerializeField] private float _displayOffsetY;
    [SerializeField] private bool _availableOnStart;

    public string GetLevelName()
    {
        return _destinationLevelName;
    }

    public string GetDisplayName()
    {
        return _locationDisplayName;
    }

    public Sprite GetIcon()
    {
        return _displayIcon;
    }

    public Sprite GetPortrait()
    {
        return _displayPortrait;
    }

    public string GetDescription()
    {
        return _locationDescription;
    }

    public float GetOffsetX()
    {
        return _displayOffsetX;
    }

    public float GetOffSetY()
    {
        return _displayOffsetY;
    }

    public bool IsAvailableOnStart()
    {
        return _availableOnStart;
    }
}
