using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickItemSlot : DropSlot
{
    QuickUseItem quickItem;
    Image image;

    bool hasInInventory = false;

    bool inCooldown;

    private void Start()
    {
        image = transform.GetChild(0).GetComponent<Image>();
    }

    public override bool OnDrop(ItemButton itemButton)
    {
        //try
        //{
        if (itemButton.Item is QuickUseItem/* && itemButton.OriginSlots[0, 0].MyGridManager.generateOnStart*/)
        {
            bool aux = true;
            if (!itemButton.OriginSlots[0, 0].MyGridManager.generateOnStart) aux = GameManager.gameManager.inventoryController.Inventory.myGrid.TryAlocateItem(itemButton);

            if (aux)
            {
                Debug.Log("OnDropNoSlot de item");
                image.color = Color.white;
                quickItem = (QuickUseItem)itemButton.Item;
                thisItemB = itemButton;
                image.sprite = itemButton.GetComponent<Image>().sprite;
                GameManager.gameManager.MainHud.quickItemSlot.SetSprite(itemButton.GetComponent<Image>().sprite);
                hasInInventory = true;
                Invoke("CheckItemInventory", 0.01f);
                if (!itemButton.OriginSlots[0, 0].MyGridManager.generateOnStart) return true;
                else return false;
            }
        }
        //}
        //catch
        //{
        //}
        return false;
    }

    public void CheckItemInventory()
    {
        if (quickItem != null)
        {
            InvenSlot aux = GameManager.gameManager.inventoryController.Inventory.FindItem(quickItem.itemName);
            Color auxColor;

            if (aux == null)
            {
                auxColor = new Color(0.75f, 0.75f, 0.75f, 0.75f);
                image.color = auxColor;
                hasInInventory = false;
            }
            else
            {
                auxColor = Color.white;
                image.color = auxColor;
                hasInInventory = true;
            }
            GameManager.gameManager.MainHud.quickItemSlot.SetColor(auxColor);
        }
    }

    public void UseItemEffect()
    {        
        if (hasInInventory && !inCooldown)
        {
            //if (quickItem.Effect()) GameManager.gameManager.inventoryController.Inventory.RemoveItem(quickItem.itemName);            
            if (!quickItem.instantUse)
            {
                inCooldown = true;
                Invoke("ResetCooldown", quickItem.AnimTime);
                Invoke("ApplyItemEffect", quickItem.AnimTime - 0.1f);
                GameManager.gameManager.MainHud.quickItemSlot.Cooldown(quickItem.AnimTime);
                GameManager.gameManager.battleController.MainCharacter.HealAnim();
            }
            else
            {
                quickItem.Effect();
            }

            //CheckItemInventory();
        }
    }
    void ApplyItemEffect()
    {
        if (quickItem.Effect()) GameManager.gameManager.inventoryController.Inventory.RemoveItem(quickItem.itemName);
        CheckItemInventory();
    }

    public void ConfirmUse()
    {
        GameManager.gameManager.inventoryController.Inventory.RemoveItem(quickItem.itemName);
        CheckItemInventory();
    }
    public ThrowableItem GetThrowable()
    {
        //ThrowableItem throwableItem = (ThrowableItem)quickItem;
        if (quickItem is ThrowableItem) return (ThrowableItem)quickItem;
        else return null;
    }

    void ResetCooldown()
    {
        inCooldown = false;
    }
}
