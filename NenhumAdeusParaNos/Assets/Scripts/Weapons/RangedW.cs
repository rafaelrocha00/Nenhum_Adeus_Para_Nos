using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedW : Weapon
{
    public NormalRangedConfig normalRConfig;

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
        weaponConfig = normalRConfig;
        ammo = normalRConfig.maxAmmo;
    }

    public override float Attack()
    {
        GameObject bulletObj = Instantiate(normalRConfig.bulletPref, transform.position, transform.rotation) as GameObject;
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.InitialSet(normalRConfig.defaultDamage, gameObject.layer);

        ammo--;
        //Debug.Log(ammo);
        return normalRConfig.defaultAttackSpeed;
    }

    public bool IsAuto()
    {
        return normalRConfig.auto;
    }

    public float GetMaxRange()
    {
        return normalRConfig.maxRange;
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
        ammo = normalRConfig.maxAmmo;
        reloading = false;
    }

    private void OnDisable()
    {
        reloading = false;
    }
}
