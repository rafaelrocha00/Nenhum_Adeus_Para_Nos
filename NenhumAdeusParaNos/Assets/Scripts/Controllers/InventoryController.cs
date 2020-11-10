using System.Collections;
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
}
