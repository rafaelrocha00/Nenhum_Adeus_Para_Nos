using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedW : Weapon
{
    public RangedConfig rangedWConfig;

    //public GameObject bulletPref;
    //public bool auto;
    //public float maxRange;

    [HideInInspector]bool reloading = false;
    public bool Reloading { get { return reloading; } }
    [HideInInspector] protected int ammo;
    public int Ammo { get { return ammo; } }

    private void Start()
    {
        //normalRConfig = (NormalRangedConfig)weaponConfig;
        weaponConfig = rangedWConfig;
        ammo = rangedWConfig.maxAmmo;
    }

    public override float Attack()
    {
        //GameObject bulletObj = Instantiate(rangedWConfig.bulletPref, transform.position, transform.rotation) as GameObject;
        //Bullet bullet = bulletObj.GetComponent<Bullet>();
        //bullet.InitialSet(rangedWConfig.defaultDamage, gameObject.layer);
        rangedWConfig.Attack(transform, gameObject.layer);

        ammo--;
        //Debug.Log(ammo);
        return rangedWConfig.defaultAttackSpeed;
    }

    public bool IsAuto()
    {
        return rangedWConfig.auto;
    }

    public float GetMaxRange()
    {
        return rangedWConfig.maxRange;
    }

    public float GetDelayToShoot()
    {
        return rangedWConfig.delayToShoot;
    }

    public bool HasAmmo()
    {
        if (ammo > 0) return true;
        else
        {
            Reload();
            return false;
        }
    }

    public void Reload()
    {
        if (!reloading) StartCoroutine("ReloadWait");
    }
    IEnumerator ReloadWait()
    {
        Debug.Log("Reloading");
        reloading = true;
        yield return new WaitForSeconds(2.0f);
        ammo = rangedWConfig.maxAmmo;
        reloading = false;
    }

    private void OnDisable()
    {
        reloading = false;
    }

    public override void Equip(WeaponConfig config)
    {
        rangedWConfig = (RangedConfig)config;
        weaponConfig = rangedWConfig;
    }
}
