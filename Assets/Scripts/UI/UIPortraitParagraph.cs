using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIPortraitParagraph : MonoBehaviour
{
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TextMeshProUGUI _topText;
    [SerializeField] private TextMeshProUGUI _bottomText;

    [ContextMenu("UpdatePreferredHeight")]
    public void UpdatePreferredHeight()
    {
        StartCoroutine(SetPreferredHeight());
    }

    private IEnumerator SetPreferredHeight()
    {
        yield return new WaitForEndOfFrame();
        var textHeight = _bottomText.GetPreferredValues().y;
        this.GetComponent<LayoutElement>().preferredHeight = textHeight + 250f; // offset image height
    }

    public void UpdateText(string text)
    {
        _topText.text = text;
    }

    public void UpdatePortrait(Sprite portrait)
    {
        _portraitImage.sprite = portrait;
    }
}
