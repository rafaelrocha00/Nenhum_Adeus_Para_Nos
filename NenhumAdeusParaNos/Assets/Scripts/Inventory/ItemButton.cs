using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    [HideInInspector] Item item;
    public Item Item { get { return item; } set { item = value; } }

    public InvenSlot[,] myInvenSlots = null;

    [HideInInspector] InvenSlot[,] originSlots;
    public InvenSlot[,] OriginSlots { get { return originSlots; } }

    [HideInInspector] DropSlot originDropSlot;
    public DropSlot OriginDropSlot { get { return originDropSlot; } set { originDropSlot = value; } }

    public void SetSlot(InvenSlot[,] slot)
    {
        //if (myInvenSlots != null)
        //{
        //    for (int i = 0; i < myInvenSlots.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < myInvenSlots.GetLength(1); j++)
        //        {
        //            myInvenSlots[i, j].ClearSlot();
        //        }
        //    }
        //}
        //else myInvenSlots = new InvenSlot[item.slotSize.x, item.slotSize.y];

        myInvenSlots = new InvenSlot[item.slotSize.x, item.slotSize.y];

        for (int i = 0; i < myInvenSlots.GetLength(0); i++)
        {
            for (int j = 0; j < myInvenSlots.GetLength(1); j++)
            {
                myInvenSlots[i, j] = slot[i, j];
            }
        }
    }

    public void ClearSlots()
    {
        if (myInvenSlots != null)
        {
            originSlots = myInvenSlots;
            for (int i = 0; i < myInvenSlots.GetLength(0); i++)
            {
                for (int j = 0; j < myInvenSlots.GetLength(1); j++)
                {
                    myInvenSlots[i, j].ClearSlot();
                }
            }
            myInvenSlots = null;
        }
    }

    public void ClearOrigin()
    {
        originSlots = null;
    }

    public void RemoveAndDestroy()
    {
        if (originSlots == null && originDropSlot != null)
        {
            WeaponSlot aux = (WeaponSlot)originDropSlot;
            GameManager.gameManager.battleController.MainCharacter.EquipOriginalWeapon(aux.isRanged);
        }
        if (item is QuickUseItem) GameManager.gameManager.inventoryController.Inventory.ItemRemovedOrAdded();
        ClearSlots();
        Destroy(gameObject);
    }
}
