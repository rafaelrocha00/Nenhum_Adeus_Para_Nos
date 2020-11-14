using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenObject : Interactives
{
    [Header("Broken Object")]
    public RepairableObject repairableObject;

    public Sprite objIcon;

    //public ResourceItem baseMaterial;
    public ResourceItem.MaterialType baseMaterialType;

    [SerializeField] int minCombinedValue = 0;
    public int MinCombinedValue { get { return minCombinedValue; } }

    Player p;

    public bool waitingToAttach = false;
    int bonusT;

    private void OnEnable()
    {
        waitingToAttach = false;
    }

    public override void Interact(Player player)
    {
        DesactiveBtp();

        if (waitingToAttach)
        {
            repairableObject.HalfAttach(bonusT, minCombinedValue);
            OnExit(player);
            return;
        }

        //Abrir janela de conserto        
        p = player;
        GameManager.gameManager.MainHud.OpenCloseInventory(true);
        GameManager.gameManager.MainHud.OpenCraftSection(this);
        CheckQuest();
    }

    public void FinishRepair(int bonusTime)
    {
        OnExit(p);
        //if (!repairableObject.repairByHit) repairableObject.Repair(bonusTime);
        //else
        //{
            bonusT = bonusTime;
            waitingToAttach = true;
            StartCoroutine(ReenableInteraction());
        //}
        GameManager.gameManager.MainHud.OpenCloseInventory(false);        
    }

    IEnumerator ReenableInteraction()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        col.enabled = false;
        yield return new WaitForEndOfFrame();
        col.enabled = true;
    }
}
