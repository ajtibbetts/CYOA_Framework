using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIMenu : MonoBehaviour
{
    protected GameObject _activeContent;
    protected GameObject _activeButton;

    protected abstract void SetupManagers();

    protected void SetInactiveColor(GameObject button)
    {
        var buttonImage = button.GetComponent<Image>();
        buttonImage.color = Color.white; 
    }

    protected void SetActiveColor(GameObject button)
    {
        var buttonImage = button.GetComponent<Image>();
        buttonImage.color = Color.yellow;
    }

    protected void SetCurrentScreenInactive()
    {
        if(_activeContent != null) 
        {
            _activeContent.GetComponent<UIScreen>().SetScreenActive(false);
            _activeContent.SetActive(false);
            SetInactiveColor(_activeButton);
        }
    }

    protected void SetActiveScreen(GameObject screenToActivate, GameObject buttonToActivate)
    {
        if(_activeContent != screenToActivate)
        {
            SetCurrentScreenInactive();
            _activeContent = screenToActivate;
            _activeContent.SetActive(true);
            _activeContent.GetComponent<UIScreen>().SetScreenActive(true);
            _activeButton = buttonToActivate;
            SetActiveColor(buttonToActivate);
        }
    }
}
