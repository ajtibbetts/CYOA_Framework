using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contentManager : MonoBehaviour
{
    [HideInInspector] public UIManager UIManager;

    private Dictionary<string, Func<string, string>> dict_functions = new Dictionary<string, Func<string, string>>();
    
    private Regex rx = new Regex(@"\{([^{}]+)\}",RegexOptions.Multiline );
    // this regex will capture all instances of text within {}. the parser follows format of {type.property}
    // example {p.playerName} will parse and return to player > playerName;


    void Awake() {
        UIManager = GetComponent<UIManager>();
        initDictionary();
    }

    void initDictionary()
    {
        dict_functions.Add("p", getPlayerProperty);
        dict_functions.Add("n", getNPCProperty);
        dict_functions.Add("e", getEnemyProperty);
        dict_functions.Add("s", getStoryProperty);
        dict_functions.Add("i", getItemProperty);
    }

    private string getVariableText(Match m)
    {
       // get string and split by . separator
        string x = m.ToString();
        string[] subs = x.Split('.');
        if(subs.Length > 1) {
            // get key and property from split
            string key = subs[0].Substring(1);
            string property = subs[1].Substring(0,subs[1].Length-1);
            // check for key and return
            if(dict_functions.ContainsKey(key)) {
                // calls matched dictionary function key and passes the property name following '.' to get the value
                return dict_functions[key](property);
            }
            Debug.Log($"key: {key} property: {property}");
        }
        return x;
    }
    
    public string parseContent(string content) {
       
    // THIS WORKS I THINK MAYBE IDK REGEX IS MAGIC

        string result = rx.Replace(content, new MatchEvaluator(getVariableText));
        // will replace each instance matched within {} and check for variable to replace

        Debug.Log("CONTENT MANAGER ---- PARSE RESULT:\n" + result);
        return result;
    }

    public static bool useRegex(String input)
    {
      //  Regex regex = new Regex("\{([^{}]+)\}", RegexOptions.IgnoreCase);
        //Regex regex = new Regex("^\\{[a-zA-Z]+\\}$", RegexOptions.IgnoreCase);
        //return regex.IsMatch(input);

        return false;
    }

    // retrieval functions using REFLECTION

    public string getPlayerProperty(string propertyName) {

        PropertyInfo propertyInfo = UIManager.controller.player.stats.GetType().GetProperty(propertyName);
        string currentValue = propertyInfo.GetValue(UIManager.controller.player.stats, null).ToString();
        Debug.Log("CONTENT MANAGER ---- Property value of " + propertyName + ": " + currentValue);

        return currentValue;
    }

    public string getNPCProperty(string propertyName) {

        return null;
    }

    public string getEnemyProperty(string propertyName) {

        return null;
    }

    public string getStoryProperty(string propertyName) {

        return null;
    }

    public string getItemProperty(string propertyName) {

        return null;
    }


}
