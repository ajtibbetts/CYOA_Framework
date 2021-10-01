using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills 
{
    private Dictionary<string, int> _skills = new Dictionary<string, int>();

    public bool AddSkill(string skillName, int startingValue = 1)
    {
        if(_skills.ContainsKey(skillName))
        {
            Debug.LogError("Skill name already exists: " + skillName);
            return false;
        }
        else
        {
            _skills.Add(skillName, startingValue);
            return true;
        }
    }
}
