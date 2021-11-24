using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaseDataObjects;

public class interactableNPC : Interactable
{
    [SerializeField] private CaseCharacterProfile _characterProfile;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public override void ProcessLocalEvent(string eventName)
    {

    }


    public CharacterProfileData GetProfileData()
    {
        return _characterProfile.GetFullData();
    }
}
