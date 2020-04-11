using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GridManager myGrid;

    public QuickItemSlot quickItemSlot;

    public List<string> names = new List<string>();

    [HideInInspector] float money = 0.00f;
    public float Money { get { return money; } set { money = value; } }

    private void Start()
    {
        GameManager.gameManager.inventoryController.Inventory = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string itemName = names[Random.Range(0, names.Count)];
            Debug.Log("Trying remove item: " + itemName);
            InvenSlot itemSlot = myGrid.FindItem(itemName);
            myGrid.RemoveItem(itemName);
            if (itemSlot != null)
            {
                Debug.Log("Item found and destroyed");
            }
            else Debug.Log("Item not found");
        }
    }

    public InvenSlot FindItem(string itemName)
    {
        return myGrid.FindItem(itemName);
    }

    public void RemoveItem(string itemName)
    {
        myGrid.RemoveItem(itemName);
    }

    public void ItemRemovedOrAdded()
    {
        StartCoroutine("CheckItemInInventory");
    }
    IEnumerator CheckItemInInventory()
    {
        yield return new WaitForEndOfFrame();
        quickItemSlot.CheckItemInventory();
    }
}
