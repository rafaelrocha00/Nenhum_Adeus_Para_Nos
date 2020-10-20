using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeSlot : DropSlot
{
    public ShopUI shopUI;
    public Image itemIcon;
    public Sprite none_sprite;

    [HideInInspector] Item[] acceptableItems;
    public Item[] AcceptableItems { get { return acceptableItems; } set { acceptableItems = value; } }

    public bool shop;

    bool empty = true;

    public override bool OnDrop(ItemButton itemButton)
    {
        if (!empty) return false;

        if (!shop && (itemButton.OriginSlots == null || itemButton.CompareTag("inventory")))
        {
            for (int i = 0; i < acceptableItems.Length; i++)
            {
                if (itemButton.Item.itemName.Equals(acceptableItems[i].itemName))
                {
                    //thisItemB = itemButton;
                    //thisItemB.OriginDropSlot = this;
                    //itemButton.ClearOrigin();
                    //itemButton.transform.position = transform.position;
                    //itemButton.transform.SetParent(transform);
                    ////Adiciona valor na barrinha
                    //empty = false;
                    //shopUI.Shop.UpdateValue(itemButton.Item.Value, shop);
                    SetItem(itemButton);

                    return true;
                }
            }

            return false;
        }
        else if (shop /*&& itemButton.OriginSlots != null*/ && itemButton.CompareTag("shop"))
        {
            //thisItemB = itemButton;
            //thisItemB.OriginDropSlot = this;
            //itemButton.ClearOrigin();
            //itemButton.transform.position = transform.position;
            //itemButton.transform.SetParent(transform);
            ////Adiciona valor na barrinha
            //empty = false;
            //shopUI.Shop.UpdateValue(itemButton.Item.Value, shop);
            SetItem(itemButton);

            return true;
        }
        else return false;

        //AdicionaValor
    }

    void SetItem(ItemButton itemButton)
    {
        thisItemB = itemButton;
        thisItemB.OriginDropSlot = this;
        itemButton.ClearOrigin();
        itemButton.transform.position = transform.position;
        itemButton.transform.SetParent(transform);
        //Adiciona valor na barrinha
        empty = false;
        shopUI.Shop.UpdateValue(itemButton.Item.Value, shop);
    }

    public override void OnRemove()
    {
        base.OnRemove();
        shopUI.Shop.UpdateValue(-thisItemB.Item.Value, shop);
        Clear();
        //thisItemB.OriginDropSlot = null;
    }

    public void Clear()
    {
        thisItemB = null;
        empty = true;
    }

    public void SetAcceptableItems(Item[] items)
    {
        acceptableItems = new Item[items.Length];

        items.CopyTo(acceptableItems, 0);
    }
}
