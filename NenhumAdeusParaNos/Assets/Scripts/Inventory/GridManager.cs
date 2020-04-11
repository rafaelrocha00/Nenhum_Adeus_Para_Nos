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
                Debug.Log(newSlot.GetComponent<InvenSlot>().transform.position);
            }
        }
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
        Debug.Log("Alocating");
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
