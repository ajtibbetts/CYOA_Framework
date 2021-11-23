using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using globalDataTypes;

public abstract class NavObject : MonoBehaviour
{
    public static event Action<eventType, string, string> OnEventTriggered;
    
    


    
    
    [Header("Nav Object")]
    public string Name;
    public string GUID = Guid.NewGuid().ToString();

    [Header("Local Properties")]

    [SerializeField] private List<localProperty> localProperties = new List<localProperty>();
    
    [Header("Conditions as Child")]
    public conditionalStatement[] conditionsToEnable;
    public GameObject ParentNavObject;

    [Header("Events")]
    [SerializeField] private actionEvent eventOnActivation;
    [SerializeField] private actionEvent eventOnDeactivation;
    public conditionalEvent[] conditionalEvents; // these are called on attached dialogue graphs generally


    
    // context creation objects
    [ContextMenu ("Generate new GUID")]
    public void GenerateNewGUID () {
        Debug.Log ("Creating new GUID for this object.");
        GUID = Guid.NewGuid().ToString();
    }


    public virtual void ActivateNavObject()
    {
        gameObject.name = gameObject.name + "-Active";
        DialogueParser.onExposedPropertyFound += AddLocalProperty;
        InitializeLocalProperties();
        if(eventOnActivation.eventType != eventType.none)
        {
            OnEventTriggered?.Invoke(eventOnActivation.eventType, 
            eventOnActivation.eventName, eventOnActivation.eventValue);
        }
    }

    public virtual void AddNavObjectToPlayer()
    {
        AddLocalProperty("hasPlayerVisited", "true");
        // only add object if not already found 
        if(!PlayerProgressTracker.Instance.NavObjectExistsInHistory(GUID))
        {
            PlayerProgressTracker.Instance.AddNavObject(
                new NavObjectEntry{
                    NavObjectGUID = GUID,
                    NavObjectName = Name,
                    NavObjectProperties = localProperties
                }
            );
        }
        
    }
    public virtual bool HasPlayerVisitedNavObject()
    {
        return PlayerProgressTracker.Instance.NavObjectExistsInHistory(GUID);
    }

    public virtual void DeactivateNavObject()
    {
        gameObject.name = gameObject.name.Substring(0,gameObject.name.LastIndexOf("-Active"));
        StopPropertyListener();
        if(eventOnDeactivation.eventType != eventType.none)
        {
            OnEventTriggered?.Invoke(eventOnDeactivation.eventType, 
            eventOnDeactivation.eventName, eventOnDeactivation.eventValue);
        }
    }

    public void StopPropertyListener()
    {
        DialogueParser.onExposedPropertyFound -= AddLocalProperty;
    }

    public void InitializeLocalProperties()
    {
        var startingProperty = localProperties.Find(x => x.PropertyName == "hasPlayerVisited");
        if(startingProperty == null)
        {
            Debug.Log("NAV OBJECT ---- Adding default 'hasPlayerVisited' property.");
            startingProperty = new localProperty();
            startingProperty.PropertyName = "hasPlayerVisited";
            startingProperty.PropertyValue = "false";
            localProperties.Add(startingProperty);
        }
        else {
            Debug.Log("NAV OBJECT ---- Default 'hasPlayerVisited' property already on object.");
        }

        // check for any prexisting saved properties
        if(PlayerProgressTracker.Instance.NavObjectExistsInHistory(GUID))
        {
            var savedProperties = PlayerProgressTracker.Instance.GetNavObjectProperties(GUID);
            localProperties = savedProperties; 
        }
    }

    public void AddLocalProperty(string propertyName, string propertyValue)
    {
        var newLocalProperty = localProperties.Find(x => x.PropertyName.ToLower() == propertyName.ToLower());
        if(newLocalProperty == null)
        {
            Debug.Log($"NAV OBJECT ---- Adding new local property name: {propertyName} value: {propertyValue}");
            newLocalProperty = new localProperty();
            newLocalProperty.PropertyName = propertyName;
            newLocalProperty.PropertyValue = propertyValue;
            localProperties.Add(newLocalProperty);
        }
        else {
            Debug.Log($"NAV OBJECT ---- Local property {propertyName} already exists on this nav object. Updating value to {propertyValue}");
            newLocalProperty.PropertyValue = propertyValue;
        }
    }

    public void SetLocalProperty(string propertyName, string propertyValue)
    {
        var updateIndex = localProperties.FindIndex(x => x.PropertyName.ToLower() == propertyName.ToLower());
        if(updateIndex >= 0)
        {
            Debug.Log($"NAV OBJECT ----  Updating local property {propertyName} with value {propertyValue}");
            localProperties[updateIndex].PropertyValue = propertyValue;

            // add updated properties to tracker
            PlayerProgressTracker.Instance.UpdateNavObjectProperties(GUID, localProperties);
        }
        else {
            Debug.LogError($"NAV OBJECT ---- Local property {propertyName} not found.");
        }
    }

    public string GetLocalPropertyValue(string propertyName)
    {
        Debug.Log("Nav object ---- Getting local property by name: " + propertyName);
        var localProperty = localProperties.Find(x => x.PropertyName.ToLower() == propertyName.ToLower());
        if(localProperty != null) return localProperty.PropertyValue;
        else 
        {
            Debug.LogError("NAV OBJECT ---- COULD NOT FIND PROPERTY NAME: " + propertyName);
            return "ERROR";
        }
    }

}
