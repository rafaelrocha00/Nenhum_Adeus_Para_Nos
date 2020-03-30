using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : RangedW
{
    public float shootAngle = 20.0f;

    public override float Attack()
    {    
        for (float i = -shootAngle; i <= shootAngle; i+= shootAngle)
        {
            GameObject bulletObj = Instantiate(bulletPref, transform.position, transform.rotation) as GameObject;
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bulletObj.transform.Rotate(0, i, 0);
            bullet.InitialSet(defaultDamage, gameObject.layer);
        }

        return defaultAtkSpeed;
    }
}
