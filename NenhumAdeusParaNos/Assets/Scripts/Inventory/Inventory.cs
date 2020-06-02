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

    public ItemGenerator iGen;

    private void Start()
    {
        GameManager.gameManager.inventoryController.Inventory = this;     
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
        myGrid.AlocateByCoord(coords, newItem);
    }

    public void RemoveItem(string itemName)
    {
        myGrid.RemoveItem(itemName);
    }

    public void SaveItems()
    {
        ItemButton[] allItems = myGrid.itemHolder.GetComponentsInChildren<ItemButton>();

        GameManager.gameManager.itemsSaver.SetInventoryItemCoords(allItems);
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
