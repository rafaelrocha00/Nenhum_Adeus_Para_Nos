﻿using System.Collections;
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

    public WeaponPreset[] allWeaponPresets;

    private void Start()
    {
        //normalRConfig = (NormalRangedConfig)weaponConfig;
        weaponConfig = rangedWConfig;
        ammo = rangedWConfig.maxAmmo;
    }

    public override void EnableModel(bool value)
    {
        allWeaponPresets[rangedWConfig.weaponPresetIndex].Enable(value);
        if (value) ammo = rangedWConfig.maxAmmo;
    }

    public override float Attack(Animator animator = null, float attackMod = 1)
    {
        //GameObject bulletObj = Instantiate(rangedWConfig.bulletPref, transform.position, transform.rotation) as GameObject;
        //Bullet bullet = bulletObj.GetComponent<Bullet>();
        //bullet.InitialSet(rangedWConfig.defaultDamage, gameObject.layer);
        if (animator != null) animator.SetInteger("Attacking", 1);
        rangedWConfig.Attack(transform, gameObject.layer, attackMod);
        if (rangedWConfig.clip_shoots.Length > 0) GameManager.gameManager.audioController.PlayEffect(rangedWConfig.clip_shoots[Random.Range(0, rangedWConfig.clip_shoots.Length)]);

        if (gameObject.layer == 9) weaponConfig.DecreaseDurability();

        ammo--;
        if (rangedWConfig.recoil > 0) myHolder.Knockback(rangedWConfig.recoil); 
        //Debug.Log(ammo);
        return rangedWConfig.defaultAttackSpeed;
    }

    public void BurstAttack(bool destroyGun = false)
    {
        StartCoroutine(Burst(destroyGun));
    }
    IEnumerator Burst(bool destroyGun = false)
    {
        for (int i = 0; i < rangedWConfig.maxAmmo; i++)
        {
            yield return new WaitForSeconds(Attack());
        }
        if (destroyGun) Destroy(gameObject);
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
        GameManager.gameManager.audioController.PlayEffect(rangedWConfig.clip_reload);
        yield return new WaitForSeconds(rangedWConfig.reloadTime);
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
