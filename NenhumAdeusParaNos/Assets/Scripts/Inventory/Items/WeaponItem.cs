using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Item/Weapon Item")]
public class WeaponItem : Item
{
    public WeaponConfig thisWeapon;

    public int durability = 100;
    [HideInInspector] int currentDurability;
    public int CurrentDurability { get { return currentDurability; } }

    //[HideInInspector] ItemDurability durability = null;
    //public ItemDurability Durability { get { return durability; } /*set { durability = value; }*/ }

    //public void StartDurability()
    //{
    //    if (durability != null) return;

    //    durability = new ItemDurability();
    //}

    public void ReduceDurability()
    {
        currentDurability--;
    }

    public float GetDurabilityRate()
    {
        return currentDurability / (float)durability;
    }

    private void OnEnable()
    {
        currentDurability = durability;
        //thisWeapon.MyItem = this;
    }
}
