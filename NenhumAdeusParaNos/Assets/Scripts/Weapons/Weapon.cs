﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float defaultDamage;
    public float defaultAtkSpeed;
    public float range;

    public abstract float Attack();
}