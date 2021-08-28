using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contentManager : MonoBehaviour
{
    [HideInInspector] public UIManager UIManager;

    private Dictionary<string, Func<string, string>> getStrings = new Dictionary<string, Func<string, string>>();
    
    private Regex rx = new Regex(@"\{([^{}]+)\}",RegexOptions.Multiline );


    void Awake() {
        UIManager = GetComponent<UIManager>();
        initDictionary();
    }

    void initDictionary(){
        

    }

    static string getVariableText(Match m)
    {
        // Get the matched string and position of . separator
        string x = m.ToString();
        string[] subs = x.Split('.');
        if(subs.Length > 1) {
            string group = subs[0].Substring(1);
            string key = subs[1].Substring(0,subs[1].Length-1);
            // do some shit with group/key either reflection / dict/whatever
            
            Debug.Log($"group: {group} key: {key}");
        }
        
        //string group = subs[0].Substring(1);
        //string key = subs[1].Substring(0,subs[1].Length-2);

        //Debug.Log($"group: {group} key: {key}");
        // If the first char is lower case...
        // if (char.IsLower(x[0]))
        // {
        //     // Capitalize it.
        //     return char.ToUpper(x[0]) + x.Substring(1, x.Length - 1);
        // }
        return x;
    }
    
    public string parseContent(string content) {
       // string ns= Regex.Replace(content,"\{([^{}]+)\}","$1 = MessageBox.Show");
    // THIS WORKS I THINK MAYBE

        string result = rx.Replace(content, new MatchEvaluator(contentManager.getVariableText));

        Debug.Log("PARSE RESULT:\n" + result);
        return result;
    }

    public static bool useRegex(String input)
    {
      //  Regex regex = new Regex("\{([^{}]+)\}", RegexOptions.IgnoreCase);
        //Regex regex = new Regex("^\\{[a-zA-Z]+\\}$", RegexOptions.IgnoreCase);
        //return regex.IsMatch(input);

        return false;
    }


}
