using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapObject", menuName = "CYOA/Map/AreaMap", order = 0)]

public class MapObject : ScriptableObject
{
    [SerializeField] private string _areaMapName;
    [SerializeField] private Sprite _mapSprite;
    [SerializeField] private float _mapOffSetX;
    [SerializeField] private float _mapOffSetY;
    [SerializeField] private List<MapLocationObject> _locations = new List<MapLocationObject>();

    public string GetMapName()
    {
        return _areaMapName;
    }

    public Sprite GetMapSprite()
    {
        return _mapSprite;
    }
    
    public float GetOffSetX()
    {
        return _mapOffSetX;
    }
    public float GetOffSetY()
    {
        return _mapOffSetY;
    }

    public List<MapLocationObject> GetLocations()
    {
        return _locations;
    }
}
