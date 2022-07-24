using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Attack",menuName ="Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    
    public float atttackRange;        /// 基本攻击距离
    public float skillRange;          /// 远程攻击距离
    public float coolDown;            /// 攻击间隔
    public int minDamge;              /// 最小攻击数值
    public int maxDamage;             /// 最大攻击数值
    public float criticalMultiplier;  /// 暴击加成百分比（爆伤）
    public float criticalChance;      /// 暴击率
}
