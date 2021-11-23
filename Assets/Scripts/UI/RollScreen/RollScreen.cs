using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RollScreen : MonoBehaviour
{
    private static RollScreen _instance;
    public static RollScreen Instance { get { return _instance; } }


    public static event Action onRollScreenReady;
    public static event Action onRollScreenCancelled;
    public static event Action onRollScreenComplete;

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
    [SerializeField] private Button goBackButton;
    [SerializeField] private List<Sprite> diceSprites = new List<Sprite>();

    [Header("Results Screen")]
    [SerializeField] private GameObject resultsContainer;
    [SerializeField] private TextMeshProUGUI resultsHeaderText;
    [SerializeField] private TextMeshProUGUI resultsDetailsText;

    private void Awake() {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        checkRollManager.onRollDataSet += SetRollScreenData;
        checkRollManager.onRollResultSent += showResults;
    }

    [ContextMenu ("Test Skill Set UI")]
    public void TestSkillSetUI() {
        headerDifficulty.text = testDifficultyValue.ToString();
        skillNameText.text = testSkillName;
        skillValueText.text = "+"+testSkillValue.ToString();

        var percentage = checkRollManager.GetProbability(testSkillValue, testDifficultyValue);
        chancePercentage.text = percentage.ToString() + "%";
        SetDifficultyColor(percentage);

        // formulaSubtitle.text = $"roll (2d6 + {testSkillValue}) to beat {testDifficultyValue}";
    }

    public void SetRollScreenData(string skillName, string rollDescription, int rollDifficulty)
    {
        headerDifficulty.text = rollDifficulty.ToString();
        headerDescription.text = rollDescription;
        skillNameText.text = skillName;

        var skillValue = Player.Instance.GetSkillValue(skillName);
        skillValueText.text = skillValue.ToString();

        var percentage = checkRollManager.GetProbability(skillValue, rollDifficulty);
        chancePercentage.text = percentage.ToString() + "%";
        SetDifficultyColor(percentage);

        // initialize buttons
        goBackButton.interactable = true;
        goBackButton.gameObject.SetActive(true);
        rollButton.onClick.RemoveAllListeners();
        rollButton.onClick.AddListener(delegate { StartRollFromUI(); });
        rollButton.GetComponentInChildren<TextMeshProUGUI>().text = "Roll";
        resultsContainer.SetActive(false);
        
        onRollScreenReady?.Invoke();
    }


    public void StartTestRoll()
    {
        resultsContainer.SetActive(false);
        TestSkillSetUI();
        StartCoroutine(StartRollSequence());
    }

    public void StartRollFromUI()
    {
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
        var passedCheck =  totalRoll > difficultyValue;
        if(passedCheck)
        {
            Debug.Log($"ROLL PASSED. ROLLED: {leftResult} + {rightResult} + {skillValue} = {totalRoll}");
        }
        else
        {
            Debug.Log($"ROLL FAILED. ROLLED: {leftResult} + {rightResult} + {skillValue}  = {totalRoll}");
        }
        leftDice.transform.rotation = Quaternion.Euler(0, 0, 0);
        rightDice.transform.rotation = Quaternion.Euler(0, 0, 0);
        // showResults(passedCheck, leftResult, rightResult, skillValue);
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
        goBackButton.gameObject.SetActive(false);
        goBackButton.interactable = false;
        rollButton.interactable = false;
        rollButton.GetComponentInChildren<TextMeshProUGUI>().text = "Wait";
        InvokeRepeating("SetRandomDiceImages", 0f, 0.05f);
        yield return new WaitForSeconds(1);
        CancelInvoke();
        checkRollManager.Instance.ProcessRollCheck(); // actual roll calculation here
    }

    void ToggleButton(bool isActive)
    {
        rollButton.interactable = isActive;
        rollButton.GetComponentInChildren<TextMeshProUGUI>().text = isActive ? "Roll" : "Wait";
    }

    void SetRandomDiceImages()
    {
        System.Random rnd = new System.Random();
        leftDice.transform.rotation = UnityEngine.Random.rotation;
        rightDice.transform.rotation = UnityEngine.Random.rotation;
        int leftResult = rnd.Next(1,7);
        SetDiceImage(leftDice, leftResult);
        int rightResult = rnd.Next(1,7);
        SetDiceImage(rightDice, rightResult);
    }

    void showResults(int leftVal, int rightVal, int skillVal, bool result)
    {
        SetDiceImage(leftDice, leftVal);
        SetDiceImage(rightDice, rightVal);
        leftDice.transform.rotation = Quaternion.Euler(0, 0, 0);
        rightDice.transform.rotation = Quaternion.Euler(0, 0, 0);
        
        
        resultsContainer.SetActive(true);
        if(result)
        {
            resultsContainer.GetComponent<Image>().color = Color.green;
            resultsHeaderText.text = "PASSED";
        }
        else{
            resultsContainer.GetComponent<Image>().color = Color.red;
            resultsHeaderText.text = "FAILED";
        }

        resultsDetailsText.text = $"Left:{leftVal}\nRight:{rightVal}\nSkill:{skillVal}\nTotal:{(leftVal+rightVal+skillVal)}";
        // Invoke("hideResults", 3.0f);

        // initialize button
        rollButton.onClick.RemoveAllListeners();
        rollButton.GetComponentInChildren<TextMeshProUGUI>().text = "OK";
        rollButton.onClick.AddListener(delegate { CloseRollScreen(); });
        rollButton.interactable = true;
    }

    void CloseRollScreen()
    {
        Debug.Log("ROLL SCREEN ---- Closing Roll Screen");
        onRollScreenComplete?.Invoke();
        checkRollManager.Instance.CompleteRollSequence();
    }

    public void CancelRollScreen()
    {
        Debug.Log("ROLL SCREEN ---- Cancelling Roll Screen");
        onRollScreenCancelled?.Invoke();
        UIManager.Instance.SlideOutRollScreen();
    }

    void hideResults()
    {
        resultsContainer.SetActive(false);
    }
}
