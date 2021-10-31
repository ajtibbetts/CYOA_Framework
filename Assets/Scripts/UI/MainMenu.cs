using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    
    private static MainMenu _instance;
    public static MainMenu Instance { get { return _instance; } }
    
    private void Awake() {
        //init singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
