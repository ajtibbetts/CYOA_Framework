using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIViewport : MonoBehaviour, IPointerClickHandler
{
    
    public static event Action onViewPortTapped;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // Debug.Log("View port tapped!");
        onViewPortTapped?.Invoke();
    }
}
