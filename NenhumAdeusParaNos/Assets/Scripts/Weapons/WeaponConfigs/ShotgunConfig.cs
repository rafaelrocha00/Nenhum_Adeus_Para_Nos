using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shotgun", menuName = "Weapons/Shotgun")]
public class ShotgunConfig : RangedConfig
{
    public float shootAngle;
    [SerializeField] int shootQuant = 1;
    public int ShootQuantEachSide { get { return shootQuant; } set { if (value < 1) shootQuant = 1; else shootQuant = value; } }

    public override void Attack(Transform transf, int layer, float atkMod = 1)
    {
        //for (float i = -shootAngle * ShootQuantEachSide; i <= shootAngle * ShootQuantEachSide; i += shootAngle)
        //{
        //    GameObject bulletObj = Instantiate(bulletPref, transf.position, transf.rotation) as GameObject;
        //    Bullet bullet = bulletObj.GetComponent<Bullet>();
        //    bulletObj.transform.Rotate(0, i, 0);
        //    bullet.InitialSet(defaultDamage * atkMod, layer);
        //}

        for (int i = 0; i < ShootQuantEachSide / 2; i++)
        {
            GameObject bulletObj = Instantiate(bulletPref, transf.position, transf.rotation) as GameObject;
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bulletObj.transform.Rotate(Random.Range(-shootAngle / 3, shootAngle / 3), Random.Range(-shootAngle / 3, shootAngle / 3), 0);
            bullet.InitialSet(defaultDamage * atkMod, layer);
        }
        for (int i = 0; i < ShootQuantEachSide - ShootQuantEachSide / 2; i++)
        {
            GameObject bulletObj = Instantiate(bulletPref, transf.position, transf.rotation) as GameObject;
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bulletObj.transform.Rotate(Random.Range(-shootAngle, shootAngle), Random.Range(-shootAngle, shootAngle), 0);
            bullet.InitialSet(defaultDamage * atkMod, layer);
        }
        //Colocar pra não tentar começar um combate quando você tiver longe do inimigo e atacar ele sem querer, ou quando o inimigo for passivo
    }
}
