using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvenSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image image;

    [HideInInspector] ItemButton thisItemButton;
    public ItemButton ThisItemButton { get { return thisItemButton; } set { thisItemButton = value; } }

    public Color selectedColor;

    [SerializeField] Vector2Int coordinates;
    public Vector2Int Coordinates { get { return coordinates; } set { coordinates = value; } }

    [HideInInspector] GridManager myGridManager;
    public GridManager MyGridManager { get { return myGridManager; } set { myGridManager = value; } }    

    //[HideInInspector] bool selected = false;
    //public bool Selected { get { return selected; } }

    int actualXRed;
    int actualYRed;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.gameManager.inventoryController.Dragging) Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Deselect();
    }

    public void Select()
    {
        GameManager.gameManager.inventoryController.ActualInvenSlot = this;
        GameManager.gameManager.inventoryController.ActualGridManager = this.myGridManager;
        if (image == null) image = GetComponent<Image>();
        image.color = selectedColor;
        //selected = true;
    }
    public void Deselect()
    {
        GameManager.gameManager.inventoryController.ActualInvenSlot = null;
        if (IsEmpty())image.color = Color.white;
        //selected = false;
    }

    public void DropItem(ItemButton itemButton)
    {
        InvenSlot[,] auxMatrix = new InvenSlot[1, 1];
        auxMatrix[0, 0] = this;

        SetFull();
        itemButton.SetSlot(auxMatrix);
        ThisItemButton = itemButton;
        itemButton.transform.position = transform.position;
        itemButton.transform.SetParent(myGridManager.itemHolder);
    }

    //public void DropBigItem(ItemButton itemButton, /*IntVector2[,] coords*/InvenSlot[,] slots)
    //{
    //    //InvenSlot[,] invenSlots = new InvenSlot[coords.GetLength(0), coords.GetLength(1)];
    //    //for (int i = 0; i < coords.GetLength(0); i++)
    //    //{
    //    //    for (int j = 0; j < coords.GetLength(1); j++)
    //    //    {
    //    //        myGridManager.invenGrid[coords[i, j].x, coords[i, j].y].thisItemButton = itemButton;
    //    //        myGridManager.invenGrid[coords[i, j].x, coords[i, j].y].SetFull();
    //    //        invenSlots[i,j] = (myGridManager.invenGrid[coords[i, j].x, coords[i, j].y]);
    //    //    }
    //    //}

    //    for (int i = 0; i < slots.GetLength(0); i++)
    //    {
    //        for (int j = 0; j < slots.GetLength(1); j++)
    //        {
    //            slots[i, j].ThisItemButton = itemButton;
    //            slots[i, j].SetFull();
    //        }
    //    }

    //    itemButton.SetSlot(slots);
    //    itemButton.transform.position = myGridManager.ItemPosition(slots);
    //}

    public bool IsEmpty()
    {
        if (ThisItemButton == null) return true;
        else return false;
    }

    public void ClearSlot()
    {
        ThisItemButton = null;
        image.color = Color.white;
    }

    public void SetSlotQuadrant(SlotQuadrant quadrant)
    {
        actualXRed = quadrant.xRed;
        actualYRed = quadrant.yRed;
    }

    public void SetFull()
    {
        image.color = Color.red;
    }

    public Vector2Int GetQuadrantRed()
    {
        return new Vector2Int(actualXRed, actualYRed);
    }
}
