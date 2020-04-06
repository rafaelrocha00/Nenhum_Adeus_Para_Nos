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
}
