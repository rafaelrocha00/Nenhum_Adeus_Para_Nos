using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenObject : Interactives
{
    public RepairableObject ro;

    public override void Interact(Player player)
    {
        //Abrir janela de conserto
    }

    public void FinishRepair(int bonusTime)
    {
        ro.Repair(bonusTime);
        //OnExit(player);
    }
}
