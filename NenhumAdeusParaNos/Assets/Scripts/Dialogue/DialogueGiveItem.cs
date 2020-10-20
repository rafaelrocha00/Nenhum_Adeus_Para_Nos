using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue give item", menuName = "Dialogue/Dialogue give item")]
public class DialogueGiveItem : Dialogue
{
    public Item itemToGive;
    public int idToTrigger = 0;

    public override string NextString()
    {
        string r = base.NextString();

        if (actualID == idToTrigger)
        {
            myNPC.ReceiveItem();
            if (itemToGive != null) GameManager.gameManager.inventoryController.Inventory.RemoveItem(itemToGive.itemName);
        }

        return r;
    }
}
