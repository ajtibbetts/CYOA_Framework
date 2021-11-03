using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSkillsScreen : MonoBehaviour
{
    [Header("Physical Skills")]
    public TextMeshProUGUI PhysicalSkillValueText;

    public TextMeshProUGUI AthleticsSkillValueText;
    public TextMeshProUGUI FortitudeSkillValueText;
    public TextMeshProUGUI StealthSkillValueText;
    public TextMeshProUGUI HardboiledHotshotSkillValueText;

    [Header("Cognitive Skills")]
    public TextMeshProUGUI CognitiveSkillValueText;

    public TextMeshProUGUI BallisticsSkillValueText;
    public TextMeshProUGUI ForensicsSkillValueText;
    public TextMeshProUGUI MedicineSkillValueText;
    public TextMeshProUGUI CyberSleuthSkillValueText;

    [Header("Volitional Skills")]
    public TextMeshProUGUI VolitionalSkillValueText;

    public TextMeshProUGUI FocusSkillValueText;
    public TextMeshProUGUI TacticsSkillValueText;
    public TextMeshProUGUI GreySightSkillValueText;
    public TextMeshProUGUI CaseChaserSkillValueText;

    [Header("Narrative Skills")]
    public TextMeshProUGUI NarrativeSkillValueText;

    public TextMeshProUGUI CausalitySkillValueText;
    public TextMeshProUGUI ReasoningSkillValueText;
    public TextMeshProUGUI OccultSkillValueText;
    public TextMeshProUGUI AceInvestigatorSkillValueText;

    [Header("Social Skills")]
    public TextMeshProUGUI SocialSkillValueText;

    public TextMeshProUGUI EmotionSkillValueText;
    public TextMeshProUGUI ConfidenceSkillValueText;
    public TextMeshProUGUI WisdomSkillValueText;
    public TextMeshProUGUI StreetSavantSkillValueText;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateData()
    {
        PhysicalSkillValueText.text = Player.Instance.Physical.ToString();
            AthleticsSkillValueText.text = Player.Instance.GetSkillValue("Athletics").ToString();
            FortitudeSkillValueText.text = Player.Instance.GetSkillValue("Fortitude").ToString();
            StealthSkillValueText.text = Player.Instance.GetSkillValue("Stealth").ToString();
            HardboiledHotshotSkillValueText.text = Player.Instance.GetSkillValue("HardboiledHotshot").ToString();


        CognitiveSkillValueText.text = Player.Instance.Cognitive.ToString();
            BallisticsSkillValueText.text = Player.Instance.GetSkillValue("Ballistics").ToString();
            ForensicsSkillValueText.text = Player.Instance.GetSkillValue("Forensics").ToString();
            MedicineSkillValueText.text = Player.Instance.GetSkillValue("Medicine").ToString();
            CyberSleuthSkillValueText.text = Player.Instance.GetSkillValue("CyberSleuth").ToString();


        VolitionalSkillValueText.text = Player.Instance.Volitional.ToString();
            FocusSkillValueText.text = Player.Instance.GetSkillValue("Focus").ToString();
            TacticsSkillValueText.text = Player.Instance.GetSkillValue("Tactics").ToString();
            GreySightSkillValueText.text = Player.Instance.GetSkillValue("GreySight").ToString();
            CaseChaserSkillValueText.text = Player.Instance.GetSkillValue("CaseChaser").ToString();


        NarrativeSkillValueText.text = Player.Instance.Narrative.ToString();
            CausalitySkillValueText.text = Player.Instance.GetSkillValue("Causality").ToString();
            ReasoningSkillValueText.text = Player.Instance.GetSkillValue("Reasoning").ToString();
            OccultSkillValueText.text = Player.Instance.GetSkillValue("Occult").ToString();
            AceInvestigatorSkillValueText.text = Player.Instance.GetSkillValue("AceInspector").ToString();


        SocialSkillValueText.text = Player.Instance.Social.ToString();
            EmotionSkillValueText.text = Player.Instance.GetSkillValue("Emotion").ToString();
            ConfidenceSkillValueText.text = Player.Instance.GetSkillValue("Confidence").ToString();
            WisdomSkillValueText.text = Player.Instance.GetSkillValue("Wisdom").ToString();
            StreetSavantSkillValueText.text = Player.Instance.GetSkillValue("StreetSavant").ToString();


    }
}
