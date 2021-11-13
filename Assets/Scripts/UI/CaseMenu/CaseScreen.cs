using UnityEngine;
using UnityEngine.UI;


public abstract class CaseScreen : UIScreen
{
    protected PlayerCaseRecord _caseRecord;

    public void SetCaseRecord(PlayerCaseRecord record)
    {
        _caseRecord = record;
    }

    public void SetPortraitThumbnail(Image portraitObject, Sprite portraitSprite, float offsetX, float offsetY)
    {
        portraitObject.sprite = portraitSprite;
        // Debug.Log("Setting sprite positon x/y: " + offsetX + "/" + offsetY);
        portraitObject.gameObject.transform.localPosition = new Vector3(offsetX,offsetY,0);
    }
}
