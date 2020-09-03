using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [HideInInspector] public BattleUnit myHolder { get; set; }

    [HideInInspector] protected WeaponConfig weaponConfig;
    public WeaponConfig WeaponConfig { get { return weaponConfig; } set { weaponConfig = value; } }

    public GameObject weaponModel;

    [SerializeField] bool equipped = false;
    public bool isEquipped { get { return equipped; } set { equipped = value; } }

    //public float defaultDamage;
    //public float defaultAtkSpeed;
    //public float range;

    public abstract float Attack(Animator animator = null, float attackMod = 1);

    public abstract void Equip(WeaponConfig config);

    public float GetRange()
    {
        return weaponConfig.range;
    }

    public void EnableModel(bool value)
    {
        if (weaponModel != null) weaponModel.SetActive(value);
    }
}
