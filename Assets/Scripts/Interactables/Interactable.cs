using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    
    public string Name;
    public string GUID = Guid.NewGuid().ToString();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    
}
