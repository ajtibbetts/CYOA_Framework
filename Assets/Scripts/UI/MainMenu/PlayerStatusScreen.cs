using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusScreen : MonoBehaviour
{
    [Header("Character Info")]
    public TextMeshProUGUI FirstNameText;
    public TextMeshProUGUI LastNameText;
    public TextMeshProUGUI RankText;
    public TextMeshProUGUI ToNextRankText;
    public Image Portrait;

    [Header("Character Stats")]
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI EnergyText;
    public TextMeshProUGUI ResolveText;
    public TextMeshProUGUI FundsText;

    [Header("Character Aspects")]
    public TextMeshProUGUI PhysicalText;
    public TextMeshProUGUI CognitiveText;
    public TextMeshProUGUI VolitionalText;
    public TextMeshProUGUI NarrativeText;
    public TextMeshProUGUI SocialText;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateData()
    {
        FirstNameText.text = Player.Instance.PlayerFirstName;
        LastNameText.text = Player.Instance.PlayerLastName;

        RankText.text = "Rank " + Player.Instance.CurrentRank;
        ToNextRankText.text = "next rank: " + Player.Instance.ExperienceToNextRank + " exp.";

        HealthText.text = Player.Instance.CurrentHealth + "/" + Player.Instance.MaxHealth;
        EnergyText.text = Player.Instance.CurrentEnergy + "/" + Player.Instance.MaxEnergy;
        ResolveText.text = Player.Instance.CurrentResolve + "/" + Player.Instance.MaxResolve;
        FundsText.text = Player.Instance.CurrentFin + " fin";

        PhysicalText.text = "Physical: " + Player.Instance.Physical;
        CognitiveText.text = "Cognitive: " + Player.Instance.Cognitive;
        VolitionalText.text = "Volitional: " + Player.Instance.Volitional;
        NarrativeText.text = "Narrative: " + Player.Instance.Narrative;
        SocialText.text = "Social: " + Player.Instance.Social;
    }
}
