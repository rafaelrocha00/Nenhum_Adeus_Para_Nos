using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Item/Weapon Item")]
public class WeaponItem : Item
{
    public WeaponConfig thisWeapon;

    //[HideInInspector] ItemDurability durability = null;
    //public ItemDurability Durability { get { return durability; } /*set { durability = value; }*/ }

    //public void StartDurability()
    //{
    //    if (durability != null) return;

    //    durability = new ItemDurability();
    //}

    //private void OnEnable()
    //{
    //    durability = null;

    //    thisWeapon.MyItem = this;
    //}
}
