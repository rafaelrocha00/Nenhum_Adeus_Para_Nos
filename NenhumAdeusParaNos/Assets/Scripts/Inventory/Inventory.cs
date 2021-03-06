﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GridManager myGrid;

    int selectedItemID = 0;
    public WeaponSlot[] weaponSlots = new WeaponSlot[2];
    public QuickItemSlot selectedItemSlot;
    public QuickItemSlot[] quickItems = new QuickItemSlot[3];

    public List<string> names = new List<string>();

    [HideInInspector] float money = 0.00f;
    public float Money { get { return money; } set { money = value; } }

    public ItemGenerator iGen;

    bool loadedEquippedItems = false;

    private void Start()
    {
        GameManager.gameManager.inventoryController.Inventory = this;
        selectedItemSlot = quickItems[selectedItemID];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //string itemName = names[Random.Range(0, names.Count)];
            //Debug.Log("Trying remove item: " + itemName);
            //InvenSlot itemSlot = myGrid.FindItem(itemName);
            //myGrid.RemoveItem(itemName);
            //if (itemSlot != null)
            //{
            //    Debug.Log("Item found and destroyed");
            //}
            //else Debug.Log("Item not found");
            //Debug.Log(myGrid.CheckItemQuant("Shield"));
        }
    }

    public InvenSlot FindItem(string itemName)
    {
        return myGrid.FindItem(itemName);
    }

    public Item[] GetItemInstances(string itemName)
    {
        return myGrid.GetItemInstances(itemName);
    }

    public void AddItem(Item i)
    {        
        ItemButton newItem = iGen.GenItem(i);
        if (!myGrid.TryAlocateItem(newItem))
        {
            Destroy(newItem.gameObject);
            GameManager.gameManager.companyController.itemsToAlocate.Enqueue(i);
        }
    }
    public void AddItemByCoord(Item i, Vector2Int[,] coords)
    {
        ItemButton newItem = iGen.GenItem(i);
        newItem.tag = myGrid.tag;
        myGrid.AlocateByCoord(coords, newItem);
    }

    public void RemoveItem(string itemName)
    {
        myGrid.RemoveItem(itemName);
    }

    public void SaveItems()
    {
        ItemButton[] allItems = myGrid.itemHolder.GetComponentsInChildren<ItemButton>();

        if (allItems.Length == 0) return;

        GameManager.gameManager.itemsSaver.SetInventoryItemCoords(allItems);
    }

    public void ChangeQuickItem(bool next)
    {
        if (next) NextItemID();
        else  PreviousItemID();

        Debug.Log("ID: " + selectedItemID);
        selectedItemSlot = quickItems[selectedItemID];
        GameManager.gameManager.MainHud.ChangeSelectedItem(selectedItemID);
    }
    void NextItemID()
    {
        if (selectedItemID == 2)
        {
            selectedItemID = 0;
            return;
        }
        selectedItemID++;
    }
    void PreviousItemID()
    {       
        if (selectedItemID == 0)
        {
            selectedItemID = 2;
            return;
        }
        selectedItemID--;
    }

    public void ItemRemovedOrAdded()
    {
        StartCoroutine("CheckItemInInventory");
    }
    IEnumerator CheckItemInInventory()
    {
        yield return new WaitForEndOfFrame();
        //quickItemSlot.CheckItemInventory();
        for (int i = 0; i < 3; i++)
        {
            quickItems[i].CheckItemInventory();
        }
    }

    public ItemButton GetEquippedWeapon(int id)
    {
        return weaponSlots[id].ThisItemB;
    }

    public void LoadEquippedItems()
    {
        if (loadedEquippedItems) return;

        Debug.Log("Loading equip items in inventory");

        GameManager.gameManager.inventoryController.LoadEquips();

        loadedEquippedItems = true;
    }

    public void SetEquippedWeapons(Item[] i_weapons)
    {
        Debug.Log("Loading equip weapons");

        for (int i = 0; i < 2; i++)
        {
            if (i_weapons[i] != null)
            {
                ItemButton aux = iGen.GenItem(i_weapons[i]);
                weaponSlots[i].OnDrop(aux);
            }
        }
    }

    public void SetEquippedItems(Item[] i_quick)
    {
        Debug.Log("Loading equip quick");

        for (int i = 0; i < i_quick.Length; i++)
        {
            if (i_quick[i] != null)
            {
                quickItems[i].SetItem(i_quick[i]);
            }
        }
    }

    private void OnDisable()
    {
        List<Item> eq_weapons = new List<Item>();
        List<Item> eq_quickItems = new List<Item>();

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].ThisItemB == null) eq_weapons.Add(null);
            else eq_weapons.Add(weaponSlots[i].ThisItemB.Item);
        }

        for (int i = 0; i < quickItems.Length; i++)
        {
            if (quickItems[i].CheckItemIventoryToSave()) eq_quickItems.Add(quickItems[i].QuickItem);
            else eq_quickItems.Add(null);
        }

        GameManager.gameManager.inventoryController.SaveEquips(eq_weapons.ToArray(), eq_quickItems.ToArray());
    }
}
