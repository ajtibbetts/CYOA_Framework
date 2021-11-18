using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RollScreen : MonoBehaviour
{
    private static RollScreen _instance;
    public static RollScreen Instance { get { return _instance; } }

    [Header("Test Values")]
    public string testSkillName;
    public int testSkillValue;
    public int testDifficultyValue;
    
    [Header("Header UI")]
    [SerializeField] private TextMeshProUGUI headerDescription;
    [SerializeField] private TextMeshProUGUI headerDifficulty;
    [SerializeField] private Image headerContainerColor;

    [Header("Probability UI")]
    [SerializeField] private TextMeshProUGUI chanceDescription;
    [SerializeField] private TextMeshProUGUI chancePercentage;
    [SerializeField] private TextMeshProUGUI formulaSubtitle;
    [Header("Player Skill UI")]
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillValueText;

    [Header("Dice Roll UI")]
    [SerializeField] private Image leftDice;
    [SerializeField] private Image rightDice;
    [SerializeField] private Button rollButton;
    [SerializeField] private List<Sprite> diceSprites = new List<Sprite>();

    private void Awake() {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    [ContextMenu ("Test Skill Set UI")]
    public void TestSkillSetUI() {
        headerDifficulty.text = testDifficultyValue.ToString();
        skillNameText.text = testSkillName;
        skillValueText.text = "+"+testSkillValue.ToString();

        var percentage = checkManager.GetProbability(testSkillValue, testDifficultyValue);
        chancePercentage.text = percentage.ToString() + "%";
        SetDifficultyColor(percentage);

        // formulaSubtitle.text = $"roll (2d6 + {testSkillValue}) to beat {testDifficultyValue}";
    }


    public void StartTestRoll()
    {
        TestSkillSetUI();
        StartCoroutine(StartRollSequence());
    }

    private void RollDice(int skillValue, int difficultyValue)
    {
        System.Random rnd = new System.Random();
        int leftResult = rnd.Next(1,7);
        SetDiceImage(leftDice, leftResult);
        int rightResult = rnd.Next(1,7);
        SetDiceImage(rightDice, rightResult);
        int totalRoll = leftResult+ rightResult + skillValue;
        if(totalRoll > difficultyValue)
        {
            Debug.Log($"ROLL PASSED. ROLLED: {leftResult} + {rightResult} + {skillValue} = {totalRoll}");
        }
        else
        {
            Debug.Log($"ROLL FAILED. ROLLED: {leftResult} + {rightResult} + {skillValue}  = {totalRoll}");
        }
        ToggleButton(true);
    }

    private void SetDiceImage(Image diceElement, int result)
    {
        diceElement.sprite = diceSprites[result - 1]; // sets to match result offset for list index
    }

    private void SetDifficultyColor(double percentage)
    {
        // int value = System.Convert.ToInt32(percentage);
    
        if(percentage > 75) headerContainerColor.color = Color.green;
        else if (percentage >= 50) headerContainerColor.color = Color.gray;
        else if (percentage >= 25) headerContainerColor.color = Color.yellow;
        else headerContainerColor.color = Color.red;
        
    }

    IEnumerator StartRollSequence()
    {
        ToggleButton(false);
        InvokeRepeating("SetRandomDiceImages", 0f, 0.1f);
        yield return new WaitForSeconds(1);
        CancelInvoke();
        RollDice(testSkillValue, testDifficultyValue);
    }

    void ToggleButton(bool isActive)
    {
        rollButton.interactable = isActive;
        rollButton.GetComponentInChildren<TextMeshProUGUI>().text = isActive ? "Roll" : "Wait";
    }

    void SetRandomDiceImages()
    {
        System.Random rnd = new System.Random();
        int leftResult = rnd.Next(1,7);
        SetDiceImage(leftDice, leftResult);
        int rightResult = rnd.Next(1,7);
        SetDiceImage(rightDice, rightResult);
    }
}
