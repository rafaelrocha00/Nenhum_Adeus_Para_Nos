using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public Item[] sellingItems;

    public Item[] acceptableItems;

    [HideInInspector] int currentRequiredValue = 0;
    [HideInInspector] int currentValue = 0;

    public int CurrentRequiredValue { get { return currentRequiredValue; } set { currentRequiredValue = value; } }
    public int CurrentValue { get { return currentValue; } set { currentValue = value; } }

    bool unlocked = false;

    public void OpenShop()
    {
        GameManager.gameManager.MainHud.OpenShopUI(this);
    }

    public void UpdateValue(int value, bool shop)
    {
        if (shop) currentRequiredValue += value;
        else currentValue += value;

        float rate = Mathf.Clamp((float)currentValue / currentRequiredValue, 0, 1);
        if (currentRequiredValue > 0) GameManager.gameManager.MainHud.shopUI.UpdateBar(rate);

        if (rate == 1) unlocked = true;
        else unlocked = false;
    }

    public void ConfirmTrade(TradeSlot[] choosenItems, TradeSlot[] playerItems)
    {
        if (!unlocked) return;

        for (int i = 0; i < playerItems.Length; i++)
        {
            if (playerItems[i].ThisItemB != null)
            {
                playerItems[i].ThisItemB.RemoveAndDestroy();
                playerItems[i].Clear();
            }
        }

        for (int i = 0; i < choosenItems.Length; i++)
        {
            if (choosenItems[i].ThisItemB != null)
            {
                GameManager.gameManager.inventoryController.Inventory.myGrid.TryAlocateItem(choosenItems[i].ThisItemB);
                choosenItems[i].Clear();
            }
        }

        ResetValue();
    }

    public void CancelTrade()
    {
        ResetValue();

        GameManager.gameManager.MainHud.CloseShopUI();
        GameManager.gameManager.MainHud.OpenCloseInventory(false);
    }

    void ResetValue()
    {
        currentValue = 0;
        currentRequiredValue = 0;
        GameManager.gameManager.MainHud.shopUI.UpdateBar(0);
    }
}
