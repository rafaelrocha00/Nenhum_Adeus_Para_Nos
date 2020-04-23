using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Storage : Interactives
{
    GameObject storageMenu;

    public GameObject storagePref;
    GridManager myGrid;
    ItemGenerator itemGenerator;
    public int storageXSize;
    public int storageYSize;

    public List<Item> items = new List<Item>();
    bool generatedMenu;

    public override void Interact(Player player)
    {
        if (!generatedMenu)
        {
            GenerateSlots();
        }
        else OpenCloseStorage(true);

        GameManager.gameManager.MainHud.OpenCloseInventory(true);
        DesactiveBtp();
    }

    void GenerateSlots()
    {
        GameObject aux = Instantiate(storagePref, GameManager.gameManager.MainHud.itemStorages, false) as GameObject;
        storageMenu = aux;
        itemGenerator = aux.GetComponent<ItemGenerator>();
        RectTransform auxRect = aux.GetComponent<RectTransform>();
        myGrid = aux.GetComponentInChildren<GridManager>();
        myGrid.xSize = storageXSize;
        myGrid.ySize = storageYSize;
        for (int i = -1; i < 2; i++)
        {
            if (i > -1) auxRect = aux.transform.GetChild(i).GetComponent<RectTransform>();

            auxRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, auxRect.sizeDelta.x * storageXSize);
            auxRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, auxRect.sizeDelta.y * storageYSize);
        }

        myGrid.Generate();
        //StartCoroutine("Mark");
        StartCoroutine("GenItems");

        generatedMenu = true;
    }

    IEnumerator GenItems()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < items.Count; i++)
        {
            TryAlocateItem(items[i]);
        }
    }

    void TryAlocateItem(Item item)
    {
        //bool itemWasPlaced = false;
        //for (int i = 0; i < myGrid.invenGrid.GetLength(0); i++)
        //{
        //    for (int j = 0; j < myGrid.invenGrid.GetLength(1); j++)
        //    {
        //        Debug.Log(myGrid.invenGrid[i, j].transform.position);
        //        if (myGrid.invenGrid[i, j].IsEmpty())
        //        {
        //            if (item.slotSize == Vector2.one)
        //            {
        //                ItemButton auxIB = itemGenerator.GenItem(item);
        //                myGrid.invenGrid[i, j].DropItem(auxIB);
        //                itemWasPlaced = true;
        //            }
        //            else
        //            {
        //                int xSize = item.slotSize.x;
        //                int ySize = item.slotSize.y;
        //                InvenSlot[,] itemSlots = new InvenSlot[xSize, ySize];
        //                bool canAlocateItem = true;
        //                for (int x = 0; x < xSize; x++)
        //                {
        //                    for (int y = 0; y < ySize; y++)
        //                    {
        //                        try
        //                        {
        //                            if (myGrid.invenGrid[i + x, j + y].IsEmpty()) itemSlots[x, y] = myGrid.invenGrid[i + x, j + y];
        //                            else canAlocateItem = false;
        //                        }
        //                        catch { canAlocateItem = false; }
        //                        if (!canAlocateItem) break;    
        //                    }
        //                    if (!canAlocateItem) break;
        //                }
        //                if (canAlocateItem)
        //                {
        //                    ItemButton auxIB = itemGenerator.GenItem(item);
        //                    myGrid.AlocateBigItem(auxIB, itemSlots);
        //                    itemWasPlaced = true;
        //                }
        //            }
        //            if (itemWasPlaced) break;
        //        }
        //    }
        //    if (itemWasPlaced) break;
        //}
        ItemButton auxIB = itemGenerator.GenItem(item);
        if (!myGrid.TryAlocateItem(auxIB)) Destroy(auxIB.gameObject);
    }

    IEnumerator Mark()
    {
        //bool itemWasPlaced = false;
        for (int i = 0; i < myGrid.invenGrid.GetLength(0); i++)
        {
            for (int j = 0; j < myGrid.invenGrid.GetLength(1); j++)
            {
                myGrid.invenGrid[i, j].Select();
                yield return new WaitForSeconds(1);
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        OpenCloseStorage( false);
    }

    public void OpenCloseStorage(bool value)
    {
        if (storageMenu != null) storageMenu.SetActive(value);
    }
}
