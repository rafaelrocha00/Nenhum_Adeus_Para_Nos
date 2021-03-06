﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Storage : Interactives
{
    [Header("Storage")]
    //public string depositName = "";
    public int page_number = 1;
    public bool main = false;
    public bool questItemsSpawner = false;

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

    public AudioClip clip_openStorage;

    public override void CheckForQuestObjectives(Quest q_)
    {
        base.CheckForQuestObjectives(q_);

        if (!(q_ is DeliveryQuest)) return;

        DeliveryQuest q = (DeliveryQuest)q_;
        if (q.DepositName.Equals(Name))
        {
            SpawnQuestMarker();
            active_quest = q;
            return;
        }
    }

    public override void Interact(Player player)
    {
        if (!generatedMenu)
        {
            GenerateSlots();
        }
        else OpenCloseStorage(true);

        if (clip_openStorage == null) GameManager.gameManager.audioController.PlayEffect(GameManager.gameManager.inventoryController.clip_openStorage);
        else GameManager.gameManager.audioController.PlayEffect(clip_openStorage);
        GameManager.gameManager.MainHud.OpenCloseInventory(true);
        DesactiveBtp();
        CheckQuest();
    }

    public void GenerateSlots(bool close = false)
    {
        if (generatedMenu) return;

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
        
        pages[0] = aux.transform.GetChild(0).gameObject;
        for (int i = 1; i < page_number; i++)
        {
            pages[i] = Instantiate(aux.transform.GetChild(0).gameObject, aux.transform, false) as GameObject;
            myGrids[i] = pages[i].GetComponentInChildren<GridManager>();
            myGrids[i].Generate();
        }
        myGrids[0].Generate();

        prevPageB = aux.transform.GetChild(2).Find("PrevPage").gameObject;
        nextPageB = aux.transform.GetChild(2).Find("NextPage").gameObject;
        GameObject exitB = aux.transform.GetChild(1).Find("Exit").gameObject;
        
        prevPageB.GetComponent<Button>().onClick.AddListener(PreviousPage);
        nextPageB.GetComponent<Button>().onClick.AddListener(NextPage);
        exitB.GetComponent<Button>().onClick.AddListener(CloseStorage);

        aux.transform.GetChild(1).Find("chest_name").GetComponent<Text>().text = Name;

        StartCoroutine(GenItems(close));;
        generatedMenu = true;
        GameManager.gameManager.MainHud.ActualStorage = this;       
    }

    void LoadItems()
    {
        GameManager.gameManager.itemsSaver.SetChestItems(this);
    }

    IEnumerator GenItems(bool close = false)
    {
        yield return new WaitForEndOfFrame();
        if (!GameManager.gameManager.itemsSaver.FindChest(Name))
        {
            for (int i = 0; i < items.Count; i++)
            {
                TryAlocateItem(items[i]);
            }
        }
        else LoadItems();
        if (close) CloseStorage();
        ResetPages();

        if (main)
        {
            //int qSize = GameManager.gameManager.companyController.itemsToAlocate.Count;
            //for (int i = 0; i < qSize; i++)
            //{
            //    TryAlocateItem(GameManager.gameManager.companyController.itemsToAlocate.Dequeue());
            //}
            while (GameManager.gameManager.companyController.itemsToAlocate.Count > 0)
            {
                TryAlocateItem(GameManager.gameManager.companyController.itemsToAlocate.Dequeue());
            }
        }
        else if (questItemsSpawner)
        {
            while (GameManager.gameManager.itemsSaver.itemsToDelivery.Count > 0)
            {
                TryAlocateItem(GameManager.gameManager.itemsSaver.itemsToDelivery.Dequeue());
            }
        }

        //if (close) CloseStorage();
    }

    void TryAlocateItem(Item item)
    {
        ItemButton auxIB = itemGenerator.GenItem(item);
        for (int i = 0; i < myGrids.Length; i++)
        {
            if (myGrids[i].TryAlocateItem(auxIB)) return;
        }

        Destroy(auxIB.gameObject);
    }

    public void TryAlocateGeneratedItem(ItemButton ib)
    {
        for (int i = 0; i < myGrids.Length; i++)
        {
            if (myGrids[i].TryAlocateItem(ib)) return;
        }
    }

    public void AddItemByCoord(Item i, Vector2Int[,] coords, int page)
    {
        ItemButton newItem = itemGenerator.GenItem(i);
        myGrids[page].AlocateByCoord(coords, newItem);
    }

    public ItemButton[][] GetItemsByPage()
    {
        ItemButton[][] allItems = new ItemButton[page_number][];
        for (int i = 0; i < page_number; i++)
        {
            allItems[i] = myGrids[i].itemHolder.GetComponentsInChildren<ItemButton>(true);
        }
        return allItems;
    }

    public Dictionary<Item, int> GetItemsQuants()
    {
        Dictionary<Item, int> allItemsByPage = new Dictionary<Item, int>();

        for (int i = 0; i < page_number; i++)
        {
            Item[] allItems = myGrids[i].GetAllItems();
            for (int j = 0; j < allItems.Length; j++)
            {
                if (allItemsByPage.ContainsKey(allItems[j])) allItemsByPage[allItems[j]]++;
                else  allItemsByPage.Add(allItems[j], 1);
            }
        }

        return allItemsByPage;
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

    public void RemoveItems(string itemName, int quant = 1)
    {
        for (int i = 0; i < quant; i++)
        {
            myGrids[actualPage].RemoveItem(itemName);
        }
    }

    public override void OnExit(Player p)
    {
        base.OnExit(p);
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
                GameManager.gameManager.itemsSaver.SetChestsItemCoords(GetItemsByPage(), Name);
                EndInteraction();
                ResetPages();
            }
            else if (value)
            {
                GameManager.gameManager.MainHud.ActualStorage = this;
            }

            storageMenu.SetActive(value);

            GameManager.gameManager.MainHud.CheckIfWindowOpen();
        }
    }

    public void CloseStorage()
    {
        OpenCloseStorage(false);
    }

    private void OnDisable()
    {
        CustomEvents.instance.onQuestAccepted -= CheckForQuestObjectives;

        if (quest_mark != null) quest_mark.SetActive(false);

        if (buttonPref != null && buttonPref.activeSelf) OnExit(GameManager.gameManager.battleController.MainCharacter);
    }
}
