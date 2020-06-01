using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject slotPref;
    public Transform itemHolder;

    public int xSize;
    public int ySize;

    public InvenSlot[,] invenGrid;

    public bool generateOnStart = true;

    private void Start()
    {
        if (generateOnStart) Generate();
    }

    public void Generate()
    {
        invenGrid = new InvenSlot[xSize, ySize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newSlot = Instantiate(slotPref, this.transform, false) as GameObject;
                invenGrid[x, y] = newSlot.GetComponent<InvenSlot>();
                invenGrid[x, y].Coordinates = new Vector2Int(x, y);
                invenGrid[x, y].MyGridManager = this;
                newSlot.name = "slot[" + x + "," + y + "]";
                //Debug.Log(newSlot.GetComponent<InvenSlot>().transform.position);
            }
        }

        if (generateOnStart) Invoke("LoadInvenItems", 0.05f);
    }
    void LoadInvenItems()
    {
        if (!GameManager.gameManager.itemsSaver.SavedInven()) GameManager.gameManager.inventoryController.Inventory.iGen.GenRandomItem();
        GameManager.gameManager.itemsSaver.SetInventoryItems();
    }

    public bool TryAlocateItem(ItemButton itemB)
    {
        //bool itemWasPlaced = false;
        //for (int i = 0; i < invenGrid.GetLength(0); i++)
        Debug.Log(invenGrid == null);
        for (int i = invenGrid.GetLength(1) - 1; i >= 0; i--)
        {
            //for (int j = 0; j < invenGrid.GetLength(1); j++)
            for (int j = 0; j < invenGrid.GetLength(0); j++)
            {
                //Debug.Log(invenGrid[j, i].transform.position);
                if (invenGrid[j, i].IsEmpty())
                {
                    if (itemB.Item.slotSize == Vector2.one)
                    {
                        //ItemButton auxIB = itemGenerator.GenItem(item);
                        invenGrid[j, i].DropItem(itemB);
                        //itemWasPlaced = true;
                        return true;
                    }
                    else
                    {
                        int xSize = itemB.Item.slotSize.x;
                        int ySize = itemB.Item.slotSize.y;
                        InvenSlot[,] itemSlots = new InvenSlot[xSize, ySize];
                        bool canAlocateItem = true;
                        //for (int x = 0; x < xSize; x++)
                        for (int y = 0; y < ySize; y++)
                        {
                            //for (int y = 0; y < ySize; y++)
                            for (int x = 0; x < xSize; x++)
                            {
                                try
                                {
                                    if (invenGrid[j + x, i - y].IsEmpty()) itemSlots[x, y] = invenGrid[j + x, i - y];
                                    else canAlocateItem = false;
                                }
                                catch { canAlocateItem = false; }
                                if (!canAlocateItem) break;

                            }
                            if (!canAlocateItem) break;
                        }
                        if (canAlocateItem)
                        {
                            //ItemButton auxIB = itemGenerator.GenItem(item);
                            AlocateBigItem(itemB, itemSlots);
                            //itemWasPlaced = true;
                            return true;
                        }
                    }
                    //if (itemWasPlaced) break;
                }
            }
            //if (itemWasPlaced) break;
        }
        return false;
    }

    public void AlocateBigItem(ItemButton itemButton, InvenSlot[,] slots)
    {
        for (int i = 0; i < slots.GetLength(0); i++)
        {
            for (int j = 0; j < slots.GetLength(1); j++)
            {
                slots[i, j].ThisItemButton = itemButton;
                slots[i, j].SetFull();
            }
        }

        itemButton.SetSlot(slots);
        itemButton.transform.position = ItemPosition(slots);
        itemButton.transform.SetParent(itemHolder);
        if (itemButton.OriginDropSlot != null) itemButton.OriginDropSlot = null;
        Debug.Log("Alocating");
    }

    public void AlocateByCoord(Vector2Int[,] coords, ItemButton itemB)
    {
        if (itemB.Item.slotSize == Vector2Int.one)
        {
            invenGrid[coords[0, 0].x, coords[0, 0].y].DropItem(itemB);
        }
        else
        {
            InvenSlot[,] slots = new InvenSlot[itemB.Item.slotSize.x, itemB.Item.slotSize.y];
            for (int x = 0; x < itemB.Item.slotSize.x; x++)
            {
                for (int y = 0; y < itemB.Item.slotSize.y; y++)
                {
                    slots[x, y] = invenGrid[coords[x, y].x, coords[x, y].y];
                }
            }

            AlocateBigItem(itemB, slots);
        }
    }

    public void RemoveItem(string itemName)
    {
        InvenSlot auxSlot = FindItem(itemName);

        if (auxSlot == null) return;
        else
        {
            auxSlot.ThisItemButton.RemoveAndDestroy();
        }
    }

    public InvenSlot FindItem(string itemName)
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if (invenGrid[i, j].ThisItemButton != null && invenGrid[i, j].ThisItemButton.Item.itemName.Equals(itemName))
                    return invenGrid[i, j];
            }
        }
        return null;
    }

    public int CheckItemQuant(string itemName)
    {
        int aux = 0;
        Item item = null;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if (invenGrid[i, j].ThisItemButton != null && invenGrid[i, j].ThisItemButton.Item.itemName.Equals(itemName))
                {
                    item = invenGrid[i, j].ThisItemButton.Item;
                    aux++;
                }
            }
        }
        try
        {
            aux /= item.slotSize.x * item.slotSize.y;
        }
        catch { Debug.Log("Item not found"); }

        return aux;
    }

    public void CheckIfItemWasAdded()
    {
        GameManager.gameManager.inventoryController.Inventory.ItemRemovedOrAdded();
    }

    public Vector3 ItemPosition(InvenSlot[,] slots)
    {
        //Vector3 corner1 = invenGrid[coords[0, 0].x, coords[0, 0].y].transform.position;
        //Vector3 corner2 = invenGrid[coords[coords.GetLength(0) - 1, coords.GetLength(1) - 1].x, coords[coords.GetLength(0) - 1, coords.GetLength(1) - 1].y].transform.position;
        Vector3 corner1 = slots[0,0].transform.position;
        Vector3 corner2 = slots[slots.GetLength(0) - 1, slots.GetLength(1) - 1].transform.position;

        return (corner1 - corner2) * 0.5f + corner2;
    }
}
