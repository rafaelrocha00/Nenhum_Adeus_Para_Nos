using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Storage : Interactives
{
    public string depositName = "";
    public int page_number = 1;

    public GameObject storageMenu;
    public GameObject[] pages;
    int actualPage = 0;
    GameObject prevPageB;
    GameObject nextPageB;
   
    public GameObject storagePref;
    GridManager[] myGrids;
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
        myGrids = new GridManager[page_number];
        pages = new GameObject[page_number];

        GameObject aux = Instantiate(storagePref, GameManager.gameManager.MainHud.itemStorages, false) as GameObject;
        storageMenu = aux;
        storageMenu.transform.position = new Vector3(storageMenu.transform.position.x, storageMenu.transform.position.y - storageYSize * 20);
        itemGenerator = aux.GetComponent<ItemGenerator>();
        RectTransform auxRect = aux.GetComponent<RectTransform>();
        myGrids[0] = aux.GetComponentInChildren<GridManager>();
        myGrids[0].xSize = storageXSize;
        myGrids[0].ySize = storageYSize;
        for (int i = -1; i < 2; i++)
        {
            if (i > -1) auxRect = aux.transform.GetChild(0).GetChild(i).GetComponent<RectTransform>();

            auxRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, auxRect.sizeDelta.x * storageXSize);
            auxRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, auxRect.sizeDelta.y * storageYSize);
        }

        //myGrids[0].Generate();
        
        pages[0] = aux.transform.GetChild(0).gameObject;
        for (int i = 1; i < page_number; i++)
        {
            pages[i] = Instantiate(aux.transform.GetChild(0).gameObject, aux.transform, false) as GameObject;
            myGrids[i] = pages[i].GetComponentInChildren<GridManager>();
            myGrids[i].Generate();
            //pages[i].SetActive(false);
        }
        myGrids[0].Generate();

        prevPageB = aux.transform.Find("PrevPage").gameObject;
        nextPageB = aux.transform.Find("NextPage").gameObject;
        prevPageB.GetComponent<Button>().onClick.AddListener(PreviousPage);
        nextPageB.GetComponent<Button>().onClick.AddListener(NextPage);

        //ResetPages();

        StartCoroutine("GenItems");
        generatedMenu = true;
        GameManager.gameManager.MainHud.ActualStorage = this;
    }

    IEnumerator GenItems()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < items.Count; i++)
        {
            TryAlocateItem(items[i]);
        }
        ResetPages();
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
        for (int i = 0; i < myGrids.Length; i++)
        {
            //if (!myGrids[i].TryAlocateItem(auxIB))
            //{
            //    Destroy(auxIB.gameObject);
            //}
            if (myGrids[i].TryAlocateItem(auxIB)) return;
        }

        Destroy(auxIB.gameObject);
    }

    IEnumerator Mark()
    {
        //bool itemWasPlaced = false;
        for (int i = 0; i < myGrids[actualPage].invenGrid.GetLength(0); i++)
        {
            for (int j = 0; j < myGrids[actualPage].invenGrid.GetLength(1); j++)
            {
                myGrids[actualPage].invenGrid[i, j].Select();
                yield return new WaitForSeconds(1);
            }
        }
    }

    public int CheckQuestItems(string itemName)
    {
        return myGrids[actualPage].CheckItemQuant(itemName);
    }

    public override void OnExit()
    {
        base.OnExit();
        OpenCloseStorage(false);
    }

    public void NextPage()
    {
        actualPage++;
        if (page_number - 1 == actualPage) nextPageB.SetActive(false);
        prevPageB.SetActive(true);

        pages[actualPage - 1].SetActive(false);
        pages[actualPage].SetActive(true);
    }
    public void PreviousPage()
    {
        actualPage--;
        if (actualPage == 0) prevPageB.SetActive(false);
        nextPageB.SetActive(true);

        pages[actualPage + 1].SetActive(false);
        pages[actualPage].SetActive(true);
    }
    void ResetPages()
    {
        actualPage = 0;
        prevPageB.SetActive(false);
        if (page_number < 2) nextPageB.SetActive(false);

        for (int i = 0; i < page_number; i++)
        {
            pages[i].SetActive(false);
        }
        pages[0].SetActive(true);
    }

    public void OpenCloseStorage(bool value)
    {
        if (storageMenu != null)
        {
            if (!value && storageMenu.activeSelf)
            {
                GameManager.gameManager.questController.CheckQuests(this);
                ResetPages();
            }
            else if (value)
            {
                GameManager.gameManager.MainHud.ActualStorage = this;
            }

            storageMenu.SetActive(value);
        }
    }
}
