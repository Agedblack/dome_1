using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    /// <summary>
    /// 最大血量
    /// 当前血量
    /// 基础防御
    /// 当前防御
    /// </summary>
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;//掉落经验
    [Header("Level")]
    public int currentLevel;//当前等级
    public int maxLevel;//最大等级
    public int baseExp;//升级所需经验
    public int currentExp;//当前经验
    public float levelBuff;
    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }
    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
            leveUp();
    }

    private void leveUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1,0,maxLevel);
        baseExp += (int)(baseExp * LevelMultiplier);
        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;

        Debug.Log("LEVEL UP" + currentLevel + "Max Health:" + maxHealth);
    }
}
