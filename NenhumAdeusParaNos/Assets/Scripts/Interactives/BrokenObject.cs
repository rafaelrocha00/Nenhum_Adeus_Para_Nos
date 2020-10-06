using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenObject : Interactives
{
    public RepairableObject repairableObject;

    public Sprite objIcon;

    //public ResourceItem baseMaterial;
    public ResourceItem.MaterialType baseMaterialType;

    [SerializeField] int minCombinedValue = 0;
    public int MinCombinedValue { get { return minCombinedValue; } }

    public override void Interact(Player player)
    {
        //Abrir janela de conserto
        DesactiveBtp();
        GameManager.gameManager.MainHud.OpenCloseInventory(true);
        GameManager.gameManager.MainHud.OpenCraftSection(this);
        CheckQuest();
    }

    public void FinishRepair(int bonusTime)
    {
        repairableObject.Repair(bonusTime);
        GameManager.gameManager.MainHud.OpenCloseInventory(false);        
    }
}
