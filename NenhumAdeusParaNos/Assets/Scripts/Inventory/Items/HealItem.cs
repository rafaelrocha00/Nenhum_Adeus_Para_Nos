using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healing Item", menuName = "Item/Healing Item")]
public class HealItem : QuickUseItem
{
    public float healingStrenght;

    public override bool Effect()
    {
        //Cura o player baseado na força da cura
        MainCharacter.Heal(healingStrenght);
        Debug.Log("Curando");
        return true;
    }
}
