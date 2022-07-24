using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdatealthBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    [HideInInspector]
    public bool isCritical;//是否暴击

    private void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
    }
    #region Read from Data_SO
    public int MaxHealth
    {
        get
        {
            if (characterData != null)
                return characterData.maxHealth;
            else return 0;
        }
        set
        {
            characterData.maxHealth = value;
        }
    }
    public int CurrentHealth
    {
        get
        {
            if (characterData != null)
                return characterData.currentHealth;
            else return 0;
        }
        set
        {
            characterData.currentHealth = value;
        }
    }
    public int BaseDefence
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefence;
            else return 0;
        }
        set
        {
            characterData.baseDefence = value;
        }
    }
    public int CurrentDefence
    {
        get
        {
            if (characterData != null)
                return characterData.currentDefence;
            else return 0;
        }
        set
        {
            characterData.currentDefence = value;
        }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacker,CharacterStats defener)
    {
        //计算伤害值
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (attacker.isCritical)
        {
            //攻击者暴击，触发受伤者动画
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        //Update UI 
        UpdatealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //经验升级
        if (CurrentHealth <= 0)
            attacker.characterData.UpdateExp(characterData.killPoint);
    }
    /// <summary>
    /// TakeDamage方法的重载
    /// </summary>
    /// <param name="damage">伤害值 </param>
    /// <param name="defener"> 受伤对象</param>
    public void TakeDamage(int damage,CharacterStats defener)
    {
        int currentDamge = Mathf.Max(damage - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamge, 0);
        UpdatealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0)//石头人被石头击败也会获得经验值
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
    }
    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamge, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击" + coreDamage);
        }
        return (int)coreDamage;
    }
    #endregion
}
