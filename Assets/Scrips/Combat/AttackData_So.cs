using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New AttackData", menuName = "Character Stats/AttackData")]
public class AttackData_So : ScriptableObject
{
    public float attackRange;
    public float skillRange;
    public float coolDown;
    public int minDamgae;
    public int maxDamage;

    public float criticalMultiplie;
    public float criticalChance;
}
