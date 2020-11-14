using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickItemSlot : DropSlot
{
    public int id = 0;

    [HideInInspector] QuickUseItem quickItem = null;
    public QuickUseItem QuickItem { get { return quickItem; } }
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
            if (!itemButton.OriginSlots[0, 0].MyGridManager.CompareTag("inventory")) aux = GameManager.gameManager.inventoryController.Inventory.myGrid.TryAlocateItem(itemButton);

            if (aux)
            {
                Debug.Log("OnDropNoSlot de item");
                image.color = Color.white;
                quickItem = (QuickUseItem)itemButton.Item;
                thisItemB = itemButton;
                image.sprite = itemButton.GetComponent<Image>().sprite;
                GameManager.gameManager.MainHud.SetQuickItemSprite(id, itemButton.GetComponent<Image>().sprite);//quickItemSlots[id].SetSprite(itemButton.GetComponent<Image>().sprite);//////////////////////////////////
                hasInInventory = true;
                if (itemButton.Item is Notes) GameManager.gameManager.questController.mainNotes = (Notes)itemButton.Item;
                Invoke("CheckItemInventory", 0.01f);
                if (!itemButton.OriginSlots[0, 0].MyGridManager.CompareTag("inventory")) return true;
                else return false;
            }
        }
        //}
        //catch
        //{
        //}
        return false;
    }

    public void SetItem(Item i)
    {
        Debug.Log("OnDropNoSlot de item");
        if (image == null) image = transform.GetChild(0).GetComponent<Image>();
        image.color = Color.white;
        quickItem = (QuickUseItem)i;
        //thisItemB = itemButton;
        image.sprite = i.itemSprite;
        GameManager.gameManager.MainHud.SetQuickItemSprite(id, i.itemSprite);//quickItemSlots[id].SetSprite(itemButton.GetComponent<Image>().sprite);//////////////////////////////////
        hasInInventory = true;
        if (i is Notes) GameManager.gameManager.questController.mainNotes = (Notes)i;
        //Invoke("CheckItemInventory", 0.01f);
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
            //GameManager.gameManager.MainHud.quickItemSlots[0].SetColor(auxColor);
            GameManager.gameManager.MainHud.SetQuickItemColor(id, auxColor);
        }
    }
    public bool CheckItemIventoryToSave()
    {
        InvenSlot aux = null;
        if (quickItem != null) aux = GameManager.gameManager.inventoryController.Inventory.FindItem(quickItem.itemName);

        return (aux != null);
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
                //GameManager.gameManager.MainHud.quickItemSlots[0].Cooldown(quickItem.AnimTime);
                GameManager.gameManager.MainHud.QuickItemCooldown(id, quickItem.AnimTime);
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
