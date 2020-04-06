using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Image image;
    Vector3 originPos;
    ItemButton itemButton;

    private void Start()
    {
        image = GetComponent<Image>();
        itemButton = GetComponent<ItemButton>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("BeginDrag");
        GameManager.gameManager.inventoryController.Dragging = true;
        GameManager.gameManager.inventoryController.ItemDragged = this;
        transform.SetParent(GameManager.gameManager.MainHud.transform);
        //itemButton.InvenSlot = null;
        itemButton.ClearSlots();
        originPos = transform.position;
        image.raycastTarget = false;
        image.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Dragging");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Drop");
        GameManager.gameManager.inventoryController.Dragging = false;
        InvenSlot auxSlot = GameManager.gameManager.inventoryController.ActualInvenSlot;

        if (auxSlot != null && auxSlot.IsEmpty())
        {
            if (itemButton.Item.slotSize == Vector2Int.one)
            {
                auxSlot.DropItem(itemButton);
                ResetIcon();
                return;
            }
            else
            {
                GridManager auxGrid = auxSlot.MyGridManager;
                //IntVector2[,] itemSlotsCoord = new IntVector2[itemButton.Item.slotSize.x, itemButton.Item.slotSize.y];
                InvenSlot[,] invenSlots = new InvenSlot[itemButton.Item.slotSize.x, itemButton.Item.slotSize.y];

                int xOffset, yOffset;

                if (itemButton.Item.slotSize.x == 1) xOffset = (auxSlot.GetQuadrantRed().x == 0) ? 0 : -1;
                else
                {
                    xOffset = (itemButton.Item.slotSize.x % 2 == 1 && auxSlot.GetQuadrantRed().x == 0) ? 1 : 0;
                    if (itemButton.Item.slotSize.x > 3) xOffset += itemButton.Item.slotSize.x / 2 - 1;
                }

                if (itemButton.Item.slotSize.y == 1) yOffset = (auxSlot.GetQuadrantRed().y == 0) ? 0 : -1;
                else
                {
                    yOffset = (itemButton.Item.slotSize.y % 2 == 1 && auxSlot.GetQuadrantRed().y == 0) ? 1 : 0;
                    if (itemButton.Item.slotSize.y > 3) yOffset += itemButton.Item.slotSize.y / 2 - 1;
                }

                int i = 0;
                for (int x = auxSlot.Coordinates.x + auxSlot.GetQuadrantRed().x - xOffset; x < auxSlot.Coordinates.x + itemButton.Item.slotSize.x + auxSlot.GetQuadrantRed().x - xOffset; x++)
                {
                    int j = 0;
                    for (int y = auxSlot.Coordinates.y + auxSlot.GetQuadrantRed().y - yOffset; y < auxSlot.Coordinates.y + itemButton.Item.slotSize.y + auxSlot.GetQuadrantRed().y - yOffset; y++)
                    {
                        Debug.Log(new Vector2Int(x, y));
                        try
                        {
                            if (auxGrid.invenGrid[x, y].IsEmpty())
                            {
                                invenSlots[i, j] = auxGrid.invenGrid[x, y];
                                Debug.Log("Adding " + new Vector2Int(x, y) + " in position " + i + " | " + j);
                            }
                            else
                            {
                                ResetPos();
                                ResetIcon();
                                return;
                            }
                        }
                        catch
                        {
                            ResetPos();
                            ResetIcon();
                            return;
                        }
                        j++;
                    }
                    i++;
                }

                //auxSlot.DropBigItem(itemButton, invenSlots);
                auxGrid.AlocateBigItem(itemButton, invenSlots);
                ResetIcon();
            }
        }
        else
        {
            ResetPos();
            ResetIcon();
        }
    }

    void ResetPos()
    {
        //GridManager auxGrid = GameManager.gameManager.inventoryController.ActualInvenSlot.MyGridManager;
        //GameManager.gameManager.inventoryController.ActualGridManager.AlocateBigItem(itemButton, itemButton.OriginSlots);
        try
        {
            itemButton.OriginSlots[0, 0].MyGridManager.AlocateBigItem(itemButton, itemButton.OriginSlots);
        } catch { }

        transform.position = originPos;
    }
    void ResetIcon()
    {
        GameManager.gameManager.inventoryController.ItemDragged = null;
        image.raycastTarget = true;
        image.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PointerDown");
    }

}
