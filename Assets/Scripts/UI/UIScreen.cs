using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIScreen : MonoBehaviour
{
    protected bool _isActiveScreen;

    public abstract void UpdateData();


    public void SetScreenActive(bool isActive)
    {
        _isActiveScreen = isActive;
    }

}
