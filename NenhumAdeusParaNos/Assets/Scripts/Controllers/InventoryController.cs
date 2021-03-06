﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector] Inventory inventory;
    public Inventory Inventory { get { return inventory; } set { inventory = value; } }

    [HideInInspector] DropSlot dropSlot;
    public DropSlot ActualDropSlot { get { return dropSlot; } set { dropSlot = value; } }

    [HideInInspector] GridManager actualGridMannager;
    public GridManager ActualGridManager { get { return actualGridMannager; } set { actualGridMannager = value; } }

    [HideInInspector] DragDrop itemDragged;
    public DragDrop ItemDragged { get { return itemDragged; } set { itemDragged = value; } }

    [HideInInspector] bool dragging = false;
    public bool Dragging { get { return dragging; } set { dragging = value; } }

    public AudioClip clip_openStorage;

    Item[] equippedWeapons = new Item[2];
    Item[] equippedIQuicktems = new Item[3];

    public void SaveEquips(Item[] ew, Item[] eq)
    {
        equippedWeapons = new Item[ew.Length];
        equippedIQuicktems = new Item[eq.Length];

        ew.CopyTo(equippedWeapons, 0);
        eq.CopyTo(equippedIQuicktems, 0);
    }

    public void LoadEquips()
    {
        Debug.Log("Loading equip items");

        inventory.SetEquippedWeapons(equippedWeapons);

        inventory.SetEquippedItems(equippedIQuicktems);
    }
}
