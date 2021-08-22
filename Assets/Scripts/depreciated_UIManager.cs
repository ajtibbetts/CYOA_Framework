using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace depreciated_Mobile {
// SINGLETON CLASS

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    
    [SerializeField]
    private int startingValue;
    [SerializeField]
    private Text leftNumberText;
    [SerializeField]
    private Text topNumberText;
    [SerializeField]
    private Text rightNumberText;
    [SerializeField]
    private Text playerNumberText;
    [SerializeField]
    private Text numberStatusText;
    private Text textDirection = null;

    private void Awake() {
        if (_instance != null && _instance != this) {
                Destroy(this.gameObject);
            }
        else {
                _instance = this;
            }  
    }

    // Start is called before the first frame update
    void Start()
    {
        numberStatusText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPositionValue(SwipeDirection direction) {
        int initialPlayerValue = Int32.Parse(playerNumberText.text);
        int baseValue;
        int finalResult = 0;
        switch (direction)
        {
            case SwipeDirection.Left:
                textDirection = leftNumberText;
            break;
            case SwipeDirection.Up:
                textDirection = topNumberText;
            break;
            case SwipeDirection.Right:
                textDirection = rightNumberText;
            break;
            default:
            break;
        }

        if(textDirection != null /* && direction != SwipeDirection.Down */) {
            if (direction != SwipeDirection.Down) {
                baseValue = Int32.Parse(textDirection.text);
                finalResult = baseValue + initialPlayerValue;
                playerNumberText.text = finalResult.ToString();
            }
           // Debug.Log("Adding player value " + initialPlayerValue + " to base value of " + baseValue + " for final result value " + finalResult);
        } 


        if(isTriangleNumber(finalResult)) {
            numberStatusText.text = "Triangle Number :)";
            numberStatusText.color = Color.green;
        }
        else if (isPrimeNumber(finalResult)) {
            numberStatusText.text = "Prime Number :(";
            numberStatusText.color = Color.red;
        }
        else {
            numberStatusText.text = "";
            numberStatusText.color = Color.black;
        }

        // update each number direction
        leftNumberText.text = getRandomFactor(finalResult).ToString();
        rightNumberText.text = getRandomFactor(finalResult).ToString();
        topNumberText.text = getRandomFactor(finalResult).ToString();
    }

    static bool isTriangleNumber(int num) {

        // Base case
        if (num < 0)
            return false;
     
        // A Triangular number must be
        // sum of first n natural numbers
        int sum = 0;
         
        for (int n = 1; sum <= num; n++)
        {
            sum = sum + n;
            if (sum == num)
                return true;
        }
     
        return false;
    }

    static bool isPrimeNumber(int n)
    {
        // Corner case
        if (n <= 1)
            return false;
 
        // Check from 2 to n-1
        for (int i = 2; i < n; i++)
            if (n % i == 0)
                return false;
 
        return true;
    }

    public List<int> getFactors(int number) 
    {
        var factors = new List<int>();
        int max = (int)Math.Sqrt(number);  // Round down

        for (int factor = 1; factor <= max; ++factor) // Test from 1 to the square root, or the int below it, inclusive.
        {  
            if (number % factor == 0) 
            {
                factors.Add(factor);
                if (factor != number/factor) // Don't add the square root twice!  Thanks Jon
                    factors.Add(number/factor);
            }
        }
        return factors;
    }

    public int getRandomFactor(int number) {
        List<int> factorList = getFactors(number);
        var random = new System.Random();
        int index = UnityEngine.Random.Range(0, factorList.Count - 1);
       
        //return random.Next(factorList.Count);
        return factorList[index];
    }
}
}
