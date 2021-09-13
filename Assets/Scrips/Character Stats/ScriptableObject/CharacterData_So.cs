using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName ="Character/Data")]
public class CharacterData_So:ScriptableObject
{
    // Start is called before the first frame update
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;
    [Header("killPoint")]
    public int killPoint;

    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;
    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }

    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp>=baseExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
       
           currentLevel =Mathf.Clamp( currentLevel + 1,0,maxLevel);
        baseExp +=(int)( baseExp * LevelMultiplier);

        maxHealth= (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;

    }
}
