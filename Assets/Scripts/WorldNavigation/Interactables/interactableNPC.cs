using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaseDataObjects;

public class interactableNPC : Interactable
{
    [SerializeField] private CaseCharacterProfile _characterProfile;

    private static CaseCharacterProfile _activeNPCProfile; // shared across all npc objects
    
   
    public override void ProcessLocalEvent(string eventName)
    {

    }

    public override void ActivateNoDialogueAction()
    {
        
    }


    public CharacterProfileData GetProfileData()
    {
        return _characterProfile.GetFullData();
    }

    private static void SetActiveNPCProfile(CaseCharacterProfile profile)
    {
        _activeNPCProfile = profile;
    }

    public static CaseCharacterProfile GetActiveNPCProfile(){ return _activeNPCProfile; }

    public override void ActivateNavObject()
    {
        base.ActivateNavObject();
        SetActiveNPCProfile(_characterProfile);
    }
}
