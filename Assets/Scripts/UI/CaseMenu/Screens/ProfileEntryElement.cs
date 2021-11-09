using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaseDataObjects;

public class ProfileEntryElement : MonoBehaviour
{
    public static event Action<CharacterProfileData> onElementTapped;
    
    private CharacterProfileData _data;


    public void SetData(CharacterProfileData data)
    {
        _data = data;
    }

    public void ElementTapped()
    {
        onElementTapped?.Invoke(_data);
    }
}
