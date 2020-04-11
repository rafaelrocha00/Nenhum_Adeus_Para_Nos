using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Weapons/Gun")]
public class RangedConfig : WeaponConfig
{
    public GameObject bulletPref;
    public float maxRange;
    public float delayToShoot = 0.1f;

    public bool auto;
    public int maxAmmo;
    //[HideInInspector] int ammo;
    //public int Ammo { get { return ammo; } }

    public virtual void Attack(Transform transf, int layer)
    {
        GameObject bulletObj = Instantiate(bulletPref, transf.position, transf.rotation) as GameObject;
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.InitialSet(defaultDamage, layer);
    }
}
