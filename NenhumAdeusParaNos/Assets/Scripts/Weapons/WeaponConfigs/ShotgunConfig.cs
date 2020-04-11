using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shotgun", menuName = "Weapons/Shotgun")]
public class ShotgunConfig : RangedConfig
{
    public float shootAngle;
    [SerializeField] int shootQuant = 1;
    public int ShootQuantEachSide { get { return shootQuant; } set { if (value < 1) shootQuant = 1; else shootQuant = value; } }

    public override void Attack(Transform transf, int layer)
    {
        for (float i = -shootAngle * ShootQuantEachSide; i <= shootAngle * ShootQuantEachSide; i += shootAngle)
        {
            GameObject bulletObj = Instantiate(bulletPref, transf.position, transf.rotation) as GameObject;
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bulletObj.transform.Rotate(0, i, 0);
            bullet.InitialSet(defaultDamage, layer);
        }
    }
}
