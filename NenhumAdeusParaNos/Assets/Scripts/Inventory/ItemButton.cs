using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] Item item;
    [HideInInspector] int page;
    public Item Item { get { return item; } set { item = value; } }
    public int Page { get { return page; } set { page = value; } }

    public InvenSlot[,] myInvenSlots = null;
    public Vector2Int[,] myCoords = null;

    [HideInInspector] InvenSlot[,] originSlots;
    public InvenSlot[,] OriginSlots { get { return originSlots; } }

    [HideInInspector] DropSlot originDropSlot;
    public DropSlot OriginDropSlot { get { return originDropSlot; } set { originDropSlot = value; } }

    [HideInInspector] bool wasInMaterialSlot;
    public bool WasInMaterialSlot { get { return wasInMaterialSlot; } set { wasInMaterialSlot = value; } }

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
        myCoords = new Vector2Int[item.slotSize.x, item.slotSize.y];

        for (int i = 0; i < myInvenSlots.GetLength(0); i++)
        {
            for (int j = 0; j < myInvenSlots.GetLength(1); j++)
            {
                myInvenSlots[i, j] = slot[i, j];
                myCoords[i, j] = slot[i, j].Coordinates;
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
        if (originSlots == null && originDropSlot != null && originDropSlot is WeaponSlot)
        {
            WeaponSlot aux = (WeaponSlot)originDropSlot;
            GameManager.gameManager.battleController.MainCharacter.EquipOriginalWeapon(aux.slotID);
        }
        if (item is QuickUseItem) GameManager.gameManager.inventoryController.Inventory.ItemRemovedOrAdded();
        ClearSlots();
        Destroy(gameObject);
    }

    public void ShowDesc()
    {
        GameManager.gameManager.MainHud.ShowItemDesc(item, transform.position);
    }
    public void HideDesc()
    {
        GameManager.gameManager.MainHud.HideItemDesc();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Invoke("ShowDesc", 0.3f);
        GameManager.gameManager.ChangeCursor(1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelInvoke();
        HideDesc();
        if (!GameManager.gameManager.inventoryController.Dragging) GameManager.gameManager.ChangeCursor(0);
    }

    private void OnDisable()
    {
        CancelInvoke();
        HideDesc();
    }
}
