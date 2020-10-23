using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public ItemGenerator iGen;

    public Image progressionBar;
    public Color lockedColor;
    public Color unlockedColor;

    [HideInInspector] Shop shop;
    public Shop Shop { get { return shop; } set { shop = value; } }

    public Transform choosentemsTab;
    TradeSlot[] choosenItems;

    public Transform playerItemsTab;
    TradeSlot[] playerItems;

    public GridManager shopItems;

    bool generatedGrid;

    private void Start()
    {
        choosenItems = choosentemsTab.GetComponentsInChildren<TradeSlot>();

        playerItems = playerItemsTab.GetComponentsInChildren<TradeSlot>();

        for (int i = 0; i < playerItems.Length; i++)
        {
            playerItems[i].SetAcceptableItems(shop.acceptableItems);
        }

        if (!generatedGrid)
        {
            generatedGrid = true;
            shopItems.Generate();
        }
    }

    private void OnEnable()
    {
        Invoke("GenItems", 0.04f);
    }

    private void OnDisable()
    {
        shopItems.DeleteAllItems();
    }

    void GenItems()
    {
        for (int i = 0; i < shop.sellingItems.Length; i++)
        {
            ItemButton newItem = iGen.GenItem(shop.sellingItems[i]);

            shopItems.TryAlocateItem(newItem);
        }
    }

    public void UpdateBar(float rate)
    {
        Debug.Log("Updating Bar: " + rate);
        //progressionBar.transform.localScale = new Vector3(rate, 1, 1);
        progressionBar.fillAmount = rate;

        if (rate == 1) progressionBar.color = unlockedColor;
        else progressionBar.color = lockedColor;
    }

    public void ConfirmTrade()
    {
        shop.ConfirmTrade(choosenItems, playerItems);
    }

    public void Exit()
    {
        for (int i = 0; i < playerItems.Length; i++)
        {
            if (playerItems[i].ThisItemB != null)
            {
                GameManager.gameManager.inventoryController.Inventory.myGrid.TryAlocateItem(playerItems[i].ThisItemB);
                playerItems[i].Clear();
            }
        }
        for (int i = 0; i < choosenItems.Length; i++)
        {
            if (choosenItems[i].ThisItemB != null)
            {
                shopItems.TryAlocateItem(choosenItems[i].ThisItemB);
                choosenItems[i].Clear();
            }
        }

        shop.CancelTrade();
    }
}
