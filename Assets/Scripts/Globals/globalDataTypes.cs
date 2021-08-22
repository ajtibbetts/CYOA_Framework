using System;   
using UnityEngine;

// public class globalDataTypes : MonoBehaviour
// {
//     // Start is called before the first frame update
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }
// }

 [Serializable]
public struct statPlayerStat {
    public string statName;
    public int statCurrent;
    public int statMax;

    public int statID;

}

 [Serializable]
public struct statAttribute {
    public string attributeName;
    public int attributeLevel;
    public int attributeID;
}

 [Serializable]
public struct StatAbility {
    public string abilityName;
    public int abilityLevel;
    public int[] baseAttributeIDs;
}
