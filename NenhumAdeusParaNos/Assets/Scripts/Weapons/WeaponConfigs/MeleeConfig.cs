using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee", menuName = "Weapons/Melee")]
public class MeleeConfig : WeaponConfig
{
    public float strongAttackDamage;
    public float strongAttackCD;
}
