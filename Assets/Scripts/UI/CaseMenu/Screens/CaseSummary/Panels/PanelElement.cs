using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PanelElement : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame updat

    public abstract void OnPointerClick(PointerEventData pointerEventData);

    public abstract void UpdateData(PlayerCaseRecord _caseRecord);

}
