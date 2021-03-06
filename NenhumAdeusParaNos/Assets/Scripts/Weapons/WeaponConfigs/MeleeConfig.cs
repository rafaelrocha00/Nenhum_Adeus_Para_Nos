﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee", menuName = "Weapons/Melee")]
public class MeleeConfig : WeaponConfig
{
    //public float strongAttackDamage;
    //public float strongAttackCD;
    //public float strongAttackAnimTime = 1.5f;
    public bool stopToAtk = false;
    public bool multAtk = false;

    public int weaponPresetIndex = 0;
    public int defaultAtkAnim = 2;
    public MeleeSpecial special;
}
