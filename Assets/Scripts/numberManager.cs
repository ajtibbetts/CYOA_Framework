using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class numberManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake() {
        SwipeDetector.OnSwipe += routeSwipeDirection;
    }

    private void routeSwipeDirection(SwipeData data)
    {
       depreciated_Mobile.UIManager.Instance.setPositionValue(data.Direction);
    }
}
