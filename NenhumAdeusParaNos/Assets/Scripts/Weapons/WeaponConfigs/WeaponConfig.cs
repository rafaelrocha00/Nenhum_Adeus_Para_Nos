using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponConfig : ScriptableObject
{
    public string weaponName;

    public float defaultDamage;
    public float defaultAttackSpeed;
    public float range;

    //[SerializeField] WeaponItem myItem = null;
    //public WeaponItem MyItem { get { return myItem; } set { myItem = value; } }

    public void DecreaseDurability()
    {
        //myItem.durability--;
        //Debug.Log("Durabilidade: " + myItem.durability);

        GameManager.gameManager.battleController.MainCharacter.ReduceCurrentWeaponDurability();
    }
}
