using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CaseDataObjects;

public class ProfileEntryElement : MonoBehaviour, IPointerClickHandler
{
    public static event Action<CharacterProfileData> onElementTapped;
    
    private CharacterProfileData _data;


    public void SetData(CharacterProfileData data)
    {
        _data = data;
        SetBackgroundColor(data.characterType);
    }

    public void SetBackgroundColor(CharacterType charType)
    {
        switch(charType)
        {
            case CharacterType.ALLY:
                gameObject.GetComponent<Image>().color = globalConfig.UI.CaseUI.profileAllyBackgroundColor;
            break;
            case CharacterType.NEUTRAL:
                gameObject.GetComponent<Image>().color = globalConfig.UI.CaseUI.profileNeutralBackgroundColor;
            break;
            case CharacterType.SUSPECT:
                gameObject.GetComponent<Image>().color = globalConfig.UI.CaseUI.profileSuspectBackgroundColor;
            break;
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        onElementTapped?.Invoke(_data);
    }
}
