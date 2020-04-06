using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector] InvenSlot invenSlot;
    public InvenSlot ActualInvenSlot { get { return invenSlot; } set { invenSlot = value; } }

    [HideInInspector] GridManager actualGridMannager;
    public GridManager ActualGridManager { get { return actualGridMannager; } set { actualGridMannager = value; } }

    [HideInInspector] DragDrop itemDragged;
    public DragDrop ItemDragged { get { return itemDragged; } set { itemDragged = value; } }

    [HideInInspector] bool dragging = false;
    public bool Dragging { get { return dragging; } set { dragging = value; } }
}
