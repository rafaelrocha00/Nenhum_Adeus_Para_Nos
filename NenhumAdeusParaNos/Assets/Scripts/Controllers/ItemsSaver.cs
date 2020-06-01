using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Item_Coord
{
    public Item item;
    public Vector2Int[,] coords;
}

public struct Chest_Item
{
    public string chestName;
    public Item_Coord[][] itemCoords;
}

public class ItemsSaver : MonoBehaviour
{
    Item_Coord[] invenItems;
    List<Chest_Item> chestItems = new List<Chest_Item>();

    public bool chestGeneratedItems = false;

    public bool SavedInven()
    {
        if (invenItems == null) return false;
        else return true;
    }

    public void BlockChestGen()
    {
        chestGeneratedItems = true;
    }

    public void SetInventoryItemCoords(ItemButton[] itemArr)
    {
        invenItems = new Item_Coord[itemArr.Length];

        for (int i = 0; i < invenItems.Length; i++)
        {
            invenItems[i].item = itemArr[i].Item;
            invenItems[i].coords = (Vector2Int[,]) itemArr[i].myCoords.Clone();
        }
    }

    public void SetInventoryItems()
    {
        if (invenItems != null)
        {
            for (int i = 0; i < invenItems.Length; i++)
            {
                GameManager.gameManager.inventoryController.Inventory.AddItemByCoord(invenItems[i].item, invenItems[i].coords);
            }
        }
    }

    public void SetChestsItemCoords(ItemButton[][] allItemsByPage, string chestName)
    {
        int id = chestItems.FindIndex(x => x.chestName.Equals(chestName));
        Chest_Item chest = new Chest_Item();

        chest.itemCoords = new Item_Coord[allItemsByPage.Length][];
        chest.chestName = chestName;
        for (int i = 0; i < allItemsByPage.Length; i++)
        {
            chest.itemCoords[i] = new Item_Coord[allItemsByPage[i].Length];
            for (int j = 0; j < allItemsByPage[i].Length; j++)
            {                
                chest.itemCoords[i][j].item = allItemsByPage[i][j].Item;
                chest.itemCoords[i][j].coords = (Vector2Int[,])allItemsByPage[i][j].myCoords.Clone();
            }
        }

        if (id < 0)
        {            
            chestItems.Add(chest);
        }
        else
        {
            chestItems[id] = new Chest_Item();
            chestItems[id] = chest;
        }
    }

    public void SetChestItems(Storage sto)
    {
        for (int i = 0; i < chestItems.Count; i++)
        {
            if (chestItems[i].chestName.Equals(sto.depositName))
            {
                for (int j = 0; j < chestItems[i].itemCoords.Length; j++)
                {
                    for (int k = 0; k < chestItems[i].itemCoords[j].Length; k++)
                    {
                        sto.AddItemByCoord(chestItems[i].itemCoords[j][k].item, chestItems[i].itemCoords[j][k].coords, j);
                    }
                }
            }
        }
    }

    public bool FindChest(string chestName)
    {
        int id = chestItems.FindIndex(x => x.chestName.Equals(chestName));

        if (id < 0) return false;
        else return true;
    }
}
