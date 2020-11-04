using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainHud : MonoBehaviour
{
    [HideInInspector] Player mainCharacter;
    public Player MainCharacter { get { return mainCharacter; } set { mainCharacter = value; } }

    public GameObject pauseMenu;

    public GameObject gameOverScreen;

    public GameObject uiToHide;

    public GameObject inventory;
    public Transform itemStorages;

    public CraftingSection craftingSection;

    public ShopUI shopUI;

    public GameObject quickMenu;
    [HideInInspector] bool isQuickMenuActive;
    public bool IsQuickMenuActive { get { return isQuickMenuActive; } }

    public GameObject destroyItemConfirm;

    public GameObject diaryMenu;

    public GameObject dialogueOptionsTab;
    public GameObject dialogueOptionPref;

    [HideInInspector] bool activeOptionsToChoose;
    public bool ActiveOptionsToChoose { get { return activeOptionsToChoose; } }

    public Image staminaBar;
    public Image lifeBar;
    public Image defenseBar;
    //public int[] battleDialogues = new int[5];
    public GameObject quickItemsTab;
    public QuickDialogueItemIcon[] quickItemSlots = new QuickDialogueItemIcon[3];
    public DashIcon dashIcon;

    public GameObject inBattle_DialogueTab;
    public Image inBattle_edIcon, inBattle_enemyIcon;

    [HideInInspector] Storage actualStorage = null;
    public Storage ActualStorage { set { actualStorage = value; } }

    CompanyPC currentPC;
    bool onPC;

    public Text date;

    public Image fadeScr;

    public GameObject itemDesc_obj;
    public Text itemDesc_name;
    public Text itemDesc_description;

    public Transform popUpsHolder;

    #region Opçoes_de_Dialogo
    //public void OpenDialogueOptTab(DialogueWithChoice dialogue)
    //{
    //    if (GameManager.gameManager.battleController.ActiveBattle)
    //        GameManager.gameManager.timeController.StartSlowdown();

    //    activeOptionsToChoose = true;
    //    dialogueOptionsTab.SetActive(true);
    //    for (int i = 0; i < dialogue.options.Length; i++)
    //    {
    //        GameObject newOptionObj = Instantiate(dialogueOptionPref, dialogueOptionsTab.transform, false) as GameObject;
    //        DialogueOption newOption = newOptionObj.GetComponent<DialogueOption>();

    //        //newOption.SetOption(dialogue.options[i], dialogue.dialogueChoices[i], dialogue.MyNPC, dialogue.MainCharacter);
    //        //Delegar escolhas
    //    }
    //}

    public void CloseDialogueOptTab()
    {
        activeOptionsToChoose = false;
        if (GameManager.gameManager.battleController.ActiveBattle)
            GameManager.gameManager.timeController.EndSlowdown();

        DestroyChilds(dialogueOptionsTab.transform);
        dialogueOptionsTab.SetActive(false);       
    }
    #endregion

    #region Notes
    public GameObject notesMenu;
    public Transform allNotes;
    Text[] notesTxtAreas;    
    public GameObject nextPageB;
    public GameObject prevPageB;
    public int maxCharPerPage = 300;
    int acPage = 0;

    bool gotTexts = false;

    public void OpenCloseNotesMenu(bool value = true)
    {
        if (value) notesMenu.SetActive(!notesMenu.activeSelf);
        else notesMenu.SetActive(false);
        if (!gotTexts && notesMenu.activeSelf)
        {
            notesTxtAreas = allNotes.GetComponentsInChildren<Text>(true);
            gotTexts = true;
        }        
    }

    public void WriteNotes(string[] notes)
    {
        for (int i = 0; i < 10; i++)
        {
            if (notes[i].Equals("")) return;
            notesTxtAreas[i].text = notes[i];
        }
    }

    public void NextPage()
    {
        Debug.Log("Prox");
        acPage++;
        if (acPage == 9) nextPageB.SetActive(false);
        else if (acPage > 0) prevPageB.SetActive(true);
        UpdatePage();
    }
    public void PrevPage()
    {
        acPage--;
        if (acPage == 0) prevPageB.SetActive(false);
        else if (acPage < 9) nextPageB.SetActive(true);
        UpdatePage();
    }
    void UpdatePage()
    {
        for (int i = 0; i < notesTxtAreas.Length; i++)
        {
            Debug.Log(i);
            notesTxtAreas[i].gameObject.SetActive(false);
        }
        Debug.Log(acPage);
        notesTxtAreas[acPage].gameObject.SetActive(true);
    }
    #endregion

    private void Start()
    {
        GameManager.gameManager.MainHud = this;

        GameManager.gameManager.calendarController.UpdateHudH();

        if (!GameManager.gameManager.NewGame) uiToHide.SetActive(true);

        //if (GameManager.gameManager.NewGame)
        //{
        Invoke("DelayOpen", 0.02f);
        //}
    }
    void DelayOpen()
    {
        GameManager.gameManager.inventoryController.Inventory.myGrid.Generate();
        //OpenCloseInventory(true);
        //Invoke("DelayClose", 0.01f);
    }
    void DelayClose()
    {
        //OpenCloseInventory(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            //if (!GameManager.gameManager.battleController.ActiveBattle)
            //OpenCloseQuickMenu();
            //OpenCloesNotesMenu(!notesMenu.activeSelf);

            //AddNote("ASDSDSADASDASDSADSADSADSA DSAD ASD SA DSA DAS DAS.");
        }
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    OpenCloseDiaryMenu();
        //}
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenCloseInventory(!inventory.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (notesMenu.activeSelf) OpenCloseNotesMenu(false);
            else if (onPC) currentPC.Exit();
            else if (actualStorage != null && actualStorage.storageMenu.activeSelf) actualStorage.OpenCloseStorage(false);
            else if (shopUI.gameObject.activeSelf) shopUI.Exit();
            else if (inventory.activeSelf) OpenCloseInventory(false);
            else if (pauseMenu.activeSelf) OpenClosePauseMenu(false);
            else OpenClosePauseMenu(true);
        }

    }

    //public void WaitingForAnswer(bool value)
    //{
    //    equippedDialogueB.GetComponent<Animator>().SetBool("WaitingAnswer", value);
    //}

    public void ShowHiddenUI(bool v)
    {
        uiToHide.SetActive(v);
    }

    public void UpdateStamina(float staminaValue)
    {
        staminaBar.rectTransform.localScale = new Vector3(staminaValue, 1, 1);
    }
    public void UpdateLife(float lifeValue)
    {
        Debug.Log(lifeValue);
        lifeBar.rectTransform.localScale = new Vector3(lifeValue, 1, 1);
    }

    public void UpdateDefense(float defenseValue)
    {
        defenseBar.rectTransform.localScale = new Vector3(defenseValue, 1, 1);
    }
    public void ShowHideDefenseBar()
    {
        defenseBar.transform.parent.gameObject.SetActive(!defenseBar.transform.parent.gameObject.activeSelf);
    }

    public void ShowHideQuickItemSlot(bool value)
    {
        //quickItemSlot.transform.parent.gameObject.SetActive(value);
        quickItemsTab.SetActive(value);
    }

    public void OpenCloseDestroyItem(bool value)
    {
        destroyItemConfirm.SetActive(value);
    }

    public void OpenCloseQuickMenu()
    {
        isQuickMenuActive = !isQuickMenuActive;
        quickMenu.SetActive(isQuickMenuActive);

        if (GameManager.gameManager.battleController.ActiveBattle)
        {
            if (IsQuickMenuActive) GameManager.gameManager.timeController.StartSlowdown();
            else GameManager.gameManager.timeController.EndSlowdown();
        }
        //else GameManager.gameManager.MainHud.OpenCloseQuickDialogueTab();
    }

    public void OpenCloseDiaryMenu()
    {
        diaryMenu.SetActive(!diaryMenu.activeSelf);
    }

    public void OpenCloseInventory(bool value)
    {
        if (!value) CloseCraftSection();
        OpenClosePauseMenu(false);
        inventory.SetActive(value);
        ShowHideQuickItemSlot(!value);       
    }
    public void OpenCraftSection(BrokenObject bo)
    {
        craftingSection.gameObject.SetActive(true);
        craftingSection.SetBrokenObject(bo);
    }
    public void CloseCraftSection()
    {
        if (craftingSection.gameObject.activeSelf)
        {
            craftingSection.ReturnItems();
            craftingSection.gameObject.SetActive(false);           
        }
    }

    public void OpenShopUI(Shop shop)
    {
        OpenCloseInventory(true);
        shopUI.gameObject.SetActive(true);
        shopUI.Shop = shop;
    }
    public void CloseShopUI()
    {
        shopUI.gameObject.SetActive(false);
        OpenCloseInventory(false);
    }

    public void OpenClosePauseMenu(bool value)
    {
        pauseMenu.SetActive(value);
        GameManager.gameManager.timeController.PauseResume(value);
    }

    public void UpdateDate(string day, int hour, int min)
    {
        date.text = day + " | " + hour.ToString("00") + ":" + min.ToString("00");
    }

    public void OpenDialogueTab(Sprite eSprite)
    {
        inBattle_DialogueTab.SetActive(true);
        inBattle_enemyIcon.sprite = eSprite;
    }
    public void CloseDialogueTab()
    {
        inBattle_DialogueTab.SetActive(false);
    }

    public void SetQuickItemSprite(int id, Sprite sp)
    {
        quickItemSlots[id].SetSprite(sp);
    }
    public void SetQuickItemColor(int id, Color color)
    {
        quickItemSlots[id].SetColor(color);
    }
    public void QuickItemCooldown(int id, float cd)
    {
        quickItemSlots[id].Cooldown(cd);
    }
    public void ChangeSelectedItem(int id)
    {
        for (int i = 0; i < 3; i++)
        {
            quickItemSlots[i].transform.parent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 15);
            quickItemSlots[i].transform.parent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 15);
        }
        quickItemSlots[id].transform.parent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30);
        quickItemSlots[id].transform.parent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
    }

    public void EnterPC(bool v, CompanyPC pc = null)
    {
        EnablePopUps(!v);
        onPC = v;
        currentPC = pc;
    }

    public void FadeInOut()
    {
        StartCoroutine("FadeIn");
    }
    IEnumerator FadeIn()
    {
        fadeScr.gameObject.SetActive(true);

        float timer = 0.0f;
        while (timer <= 1)
        {
            fadeScr.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, timer);
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
        }
        fadeScr.color = Color.black;
        StartCoroutine(FadeOut(2));
    }
    IEnumerator FadeOut(float t)
    {
        yield return new WaitForSeconds(t);
        float timer = 0.0f;
        while (timer <= 1)
        {
            fadeScr.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), timer);
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
        }

        fadeScr.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
    }

    public void BackToMenu()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        GameManager.gameManager.timeController.PauseResume(false);
        Application.Quit();
    }

    public void ShowItemDesc(Item i, Vector3 pos)
    {
        itemDesc_obj.SetActive(true);
        itemDesc_obj.transform.position = pos;
        itemDesc_name.text = i.itemName;
        itemDesc_description.text = i.description;
    }
    public void HideItemDesc()
    {
        itemDesc_obj.SetActive(false);
    }

    void SetObjSize(GameObject obj, float ammount)
    {
        try
        {
            //Debug.Log("Alow");
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.sizeDelta.x + ammount);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.sizeDelta.y + ammount);
        }
        catch { Debug.Log("Não tem RectTransform"); }
    }

    public void EnablePopUps(bool v)
    {
        popUpsHolder.gameObject.SetActive(v);
    }

    void DestroyChilds(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
