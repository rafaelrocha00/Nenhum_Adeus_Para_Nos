using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Normal Gun", menuName = "Weapons/Normal Gun")]
public class NormalRangedConfig : WeaponConfig
{
    public GameObject bulletPref;
    public float maxRange;
    public float delayToShoot = 0.1f;

    public bool auto;
    public int maxAmmo;
    //[HideInInspector] int ammo;
    //public int Ammo { get { return ammo; } }
}
