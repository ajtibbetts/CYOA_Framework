using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using globalDataTypes;

public abstract class UIScreen : MonoBehaviour
{
    public static event Action<MENUTYPE> onCloseMenu;
    
    protected bool _isActiveScreen;

    public abstract void UpdateData();


    public void SetScreenActive(bool isActive)
    {
        _isActiveScreen = isActive;
    }

    public virtual void CloseMenu(MENUTYPE menuType)
    {
        onCloseMenu?.Invoke(menuType);
    }

}
