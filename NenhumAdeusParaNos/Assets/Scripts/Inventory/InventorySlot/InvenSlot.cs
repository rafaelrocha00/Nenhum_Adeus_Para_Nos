using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvenSlot : DropSlot//, IPointerEnterHandler, IPointerExitHandler
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

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (GameManager.gameManager.inventoryController.Dragging) Select();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        Deselect();
    }

    public void Select()
    {
        //GameManager.gameManager.inventoryController.ActualInvenSlot = this;
        GameManager.gameManager.inventoryController.ActualGridManager = this.myGridManager;
        if (image == null) image = GetComponent<Image>();
        //image.color = selectedColor;
        //selected = true;
    }
    public void Deselect()
    {
       // GameManager.gameManager.inventoryController.ActualInvenSlot = null;
        //if (IsEmpty())image.color = Color.white;
        //selected = false;
    }

    public override bool OnDrop(ItemButton itemButton)
    {
        if (IsEmpty())
        {
            //if (CompareTag("shop") && itemButton.OriginSlots == null) return false;


            if ((CompareTag("shop") && itemButton.CompareTag("inventory")) || 
                (CompareTag("inventory") && itemButton.CompareTag("shop")))
                return false;

            if (itemButton.Item.slotSize == Vector2Int.one)
            {
                DropItem(itemButton);
                if (itemButton.Item is QuickUseItem) myGridManager.CheckIfItemWasAdded();
                itemButton.OriginDropSlot = null;
                return true;
            }
            else
            {
                //GridManager auxGrid = myGridManager;
                //IntVector2[,] itemSlotsCoord = new IntVector2[itemButton.Item.slotSize.x, itemButton.Item.slotSize.y];
                InvenSlot[,] invenSlots = new InvenSlot[itemButton.Item.slotSize.x, itemButton.Item.slotSize.y];

                int xOffset, yOffset;

                if (itemButton.Item.slotSize.x == 1) xOffset = (GetQuadrantRed().x == 0) ? 0 : -1;
                else
                {
                    xOffset = (itemButton.Item.slotSize.x % 2 == 1 && GetQuadrantRed().x == 0) ? 1 : 0;
                    if (itemButton.Item.slotSize.x > 3) xOffset += itemButton.Item.slotSize.x / 2 - 1;
                }

                if (itemButton.Item.slotSize.y == 1) yOffset = (GetQuadrantRed().y == 0) ? 0 : -1;
                else
                {
                    yOffset = (itemButton.Item.slotSize.y % 2 == 1 && GetQuadrantRed().y == 0) ? 1 : 0;
                    if (itemButton.Item.slotSize.y > 3) yOffset += itemButton.Item.slotSize.y / 2 - 1;
                }

                int i = 0;
                for (int x = coordinates.x + GetQuadrantRed().x - xOffset; x < coordinates.x + itemButton.Item.slotSize.x + GetQuadrantRed().x - xOffset; x++)
                {
                    int j = 0;
                    for (int y = coordinates.y + GetQuadrantRed().y - yOffset; y < coordinates.y + itemButton.Item.slotSize.y + GetQuadrantRed().y - yOffset; y++)
                    {
                        Debug.Log(new Vector2Int(x, y));
                        try
                        {
                            if (myGridManager.invenGrid[x, y].IsEmpty())
                            {
                                invenSlots[i, j] = myGridManager.invenGrid[x, y];
                                Debug.Log("Adding " + new Vector2Int(x, y) + " in position " + i + " | " + j);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        catch
                        {
                            return false;
                        }
                        j++;
                    }
                    i++;
                }

                //auxSlot.DropBigItem(itemButton, invenSlots);
                myGridManager.AlocateBigItem(itemButton, invenSlots);
                if (itemButton.Item is QuickUseItem) myGridManager.CheckIfItemWasAdded();
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public void DropItem(ItemButton itemButton)
    {
        InvenSlot[,] auxMatrix = new InvenSlot[1, 1];
        auxMatrix[0, 0] = this;

        //SetFull();
        itemButton.SetSlot(auxMatrix);
        ThisItemButton = itemButton;
        Debug.Log("SettingItemPos");
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
        //image.color = Color.white;
    }

    public void SetSlotQuadrant(SlotQuadrant quadrant)
    {
        actualXRed = quadrant.xRed;
        actualYRed = quadrant.yRed;
    }

    //public void SetFull()
    //{
    //    image.color = Color.red;
    //}

    public Vector2Int GetQuadrantRed()
    {
        return new Vector2Int(actualXRed, actualYRed);
    }
}
