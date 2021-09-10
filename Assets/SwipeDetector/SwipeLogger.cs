using UnityEngine;

public class SwipeLogger : MonoBehaviour
{
    private void Awake()
    {
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
    }

    private void SwipeDetector_OnSwipe(SwipeData data)
    {
        Debug.Log("Swipe in Direction: " + data.Direction);
        if(data.Direction == SwipeDirection.Right){
            Debug.Log("Swipe is right!");
        }
    }
}