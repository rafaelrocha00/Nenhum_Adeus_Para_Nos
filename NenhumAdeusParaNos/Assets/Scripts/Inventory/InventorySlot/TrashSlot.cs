using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrashSlot : DropSlot
{
    ItemButton itemToDelete;

    public void DeleteItem()
    {
        //GameManager.gameManager.inventoryController.ItemDragged.ThisItemB.RemoveAndDestroy();
        itemToDelete.RemoveAndDestroy();
        GameManager.gameManager.MainHud.ChangeCursor(0);
    }
    public void CancelDelete()
    {
        //GameManager.gameManager.inventoryController.DeletingItem = false;
        itemToDelete = null;
        GameManager.gameManager.MainHud.ChangeCursor(0);
    }

    private void OnDisable()
    {
        CancelDelete();
    }

    public override bool OnDrop(ItemButton itemButton)
    {
        if (itemButton.Item.indestructible) return false;

        //itemButton.RemoveAndDestroy();
        //Se for confirmar, abrir janela de confirmação
        GameManager.gameManager.MainHud.OpenCloseDestroyItem(true);
        itemToDelete = itemButton;
        return false;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Debug.Log("PointerEnter");
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        Debug.Log("PointerExit");
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    GameManager.gameManager.inventoryController.DeletingItem = true;
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    GameManager.gameManager.inventoryController.DeletingItem = false;
    //}
}
