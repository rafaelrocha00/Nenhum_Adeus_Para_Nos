using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Image image;
    Vector3 originPos;
    [HideInInspector] ItemButton itemButton;
    public ItemButton ThisItemB { get { return itemButton; } }

    private void Start()
    {
        image = GetComponent<Image>();
        itemButton = GetComponent<ItemButton>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.GetKey(KeyCode.LeftShift)) return;
        Debug.Log("BeginDrag");
        GameManager.gameManager.inventoryController.Dragging = true;
        GameManager.gameManager.inventoryController.ItemDragged = this;
        transform.SetParent(GameManager.gameManager.MainHud.transform);
        //itemButton.InvenSlot = null;
        //itemButton.ClearSlots();
        //if (itemButton.OriginDropSlot != null) itemButton.OriginDropSlot.OnRemove();
        //originPos = transform.position;
        RemoveFromSlot();
        image.raycastTarget = false;
        image.color = new Color(1, 1, 1, 0.5f);

        GameManager.gameManager.ChangeCursor(1);
    }

    public void RemoveFromSlot()
    {
        itemButton.ClearSlots();
        if (itemButton.OriginDropSlot != null) itemButton.OriginDropSlot.OnRemove();
        originPos = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Dragging");
        if (!GameManager.gameManager.inventoryController.Dragging) return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameManager.gameManager.ChangeCursor(0);

        if (!GameManager.gameManager.inventoryController.Dragging) return;
        Debug.Log("Drop");
        GameManager.gameManager.inventoryController.Dragging = false;
        //InvenSlot auxSlot = GameManager.gameManager.inventoryController.ActualInvenSlot;

        if (GameManager.gameManager.inventoryController.ActualDropSlot != null)
        {
            if (!GameManager.gameManager.inventoryController.ActualDropSlot.OnDrop(itemButton))
            {
                ResetPos();
                ResetIcon();
            }
            else
            {
                //itemButton.OriginDropSlot = null;
                ResetIcon();
            }
        }
        else
        {
            ResetPos();
            ResetIcon();
        }        
    }

    public void ResetPos()
    {
        //GridManager auxGrid = GameManager.gameManager.inventoryController.ActualInvenSlot.MyGridManager;
        //GameManager.gameManager.inventoryController.ActualGridManager.AlocateBigItem(itemButton, itemButton.OriginSlots);

        if (itemButton.OriginSlots != null)
        {
            itemButton.OriginSlots[0, 0].MyGridManager.AlocateBigItem(itemButton, itemButton.OriginSlots);
        }
        else if (itemButton.OriginDropSlot != null)
        {
            itemButton.OriginDropSlot.OnDrop(itemButton);
        }
        else if (itemButton.WasInMaterialSlot)
        {
            GameManager.gameManager.MainHud.craftingSection.DropOnLastSlot(itemButton);
            return;
        }
        else itemButton.WasInMaterialSlot = false;

        transform.position = originPos;
    }
    public void ResetIcon()
    {
        GameManager.gameManager.inventoryController.ItemDragged = null;
        image.raycastTarget = true;
        image.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetKey(KeyCode.LeftShift) && GameManager.gameManager.MainHud.ActualStorage.storageMenu.activeSelf)
        {
            if (itemButton.myInvenSlots[0, 0].CompareTag("inventory"))
            {
                RemoveFromSlot();
                GameManager.gameManager.MainHud.ActualStorage.TryAlocateGeneratedItem(itemButton);
            }
            else
            {
                RemoveFromSlot();
                GameManager.gameManager.inventoryController.Inventory.myGrid.TryAlocateItem(itemButton);
            }
        }
    }

}
