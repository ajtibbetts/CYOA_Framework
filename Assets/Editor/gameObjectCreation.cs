using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class gameObjectCreation : Editor
{
    /* MENU CONTEXT CREATION ITEMS */
    [MenuItem("GameObject/CYOA/World Nav Object", false, -100)]
    static void CreateWorldNavObject(MenuCommand menuCommand)
    {
        var parent = menuCommand.context as GameObject;
        GameObject newObject = new GameObject ("New Nav Object");
        newObject.AddComponent<WorldNavObject>();
        
        SetParentNavObject(parent, newObject);
    }
    
    [MenuItem("GameObject/CYOA/NPC", false, -100)]
    static void CreateNPC(MenuCommand menuCommand)
    {
        var parent = menuCommand.context as GameObject;
        GameObject newObject = new GameObject ("New NPC");
        newObject.AddComponent<interactableNPC>();
        
        SetParentNavObject(parent, newObject);
    }

    [MenuItem("GameObject/CYOA/Door", false, -100)]
    static void CreateDoor(MenuCommand menuCommand)
    {
        var parent = menuCommand.context as GameObject;
        GameObject newObject = new GameObject ("New Door");
        newObject.AddComponent<interactableDoor>();
        
        SetParentNavObject(parent, newObject);
    }

    static void SetParentNavObject(GameObject parent, GameObject newObject)
    {
        if(parent != null) 
        {
            WorldNavObject navParent = parent.GetComponent<WorldNavObject>();
            if(navParent != null)
            {
                newObject.transform.SetParent(parent.transform);
            }
            else 
            {
                Debug.Log("The contextual parent object is not a World Nav Object. New Object placed at top of hierarchy.");
            }
        }
    }
}
