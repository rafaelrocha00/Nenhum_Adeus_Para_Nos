using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : RangedW
{
    ShotgunConfig shotgunConfig;

    //public float shootAngle = 20.0f;
    //[SerializeField] int shootQuant = 1;
    //public int ShootQuantEachSide { get { return shootQuant; } set { if (value < 1) shootQuant = 1; else shootQuant = value; } }

    private void Start()
    {
        //shotgunConfig = (ShotgunConfig)weaponConfig;
        shotgunConfig = (ShotgunConfig)normalRConfig;
        weaponConfig = shotgunConfig;
    }

    public override float Attack()
    {    
        for (float i = -shotgunConfig.shootAngle * shotgunConfig.ShootQuantEachSide; i <= shotgunConfig.shootAngle * shotgunConfig.ShootQuantEachSide; i+= shotgunConfig.shootAngle)
        {
            GameObject bulletObj = Instantiate(shotgunConfig.bulletPref, transform.position, transform.rotation) as GameObject;
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bulletObj.transform.Rotate(0, i, 0);
            bullet.InitialSet(shotgunConfig.defaultDamage, gameObject.layer);
        }

        ammo--;
        Debug.Log(ammo);

        return shotgunConfig.defaultAttackSpeed;
    }
}
