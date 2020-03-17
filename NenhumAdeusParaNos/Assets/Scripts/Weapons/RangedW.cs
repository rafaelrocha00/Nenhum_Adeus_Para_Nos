using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedW : Weapon
{
    public GameObject bulletPref;

    public override float Attack()
    {
        GameObject bulletObj = Instantiate(bulletPref, transform.position, transform.rotation) as GameObject;
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.InitialSet(defaultDamage, gameObject.layer);

        return defaultAtkSpeed;
    }
}
