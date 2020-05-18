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

    public GameObject inventory;
    public Transform itemStorages;

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

    public QuickDialogueItemIcon quickItemSlot;

    [HideInInspector] Storage actualStorage = null;
    public Storage ActualStorage { set { actualStorage = value; } }

    #region Opçoes_de_Dialogo
    public void OpenDialogueOptTab(DialogueWithChoice dialogue)
    {
        if (GameManager.gameManager.battleController.ActiveBattle)
            GameManager.gameManager.timeController.StartSlowdown();

        activeOptionsToChoose = true;
        dialogueOptionsTab.SetActive(true);
        for (int i = 0; i < dialogue.options.Length; i++)
        {
            GameObject newOptionObj = Instantiate(dialogueOptionPref, dialogueOptionsTab.transform, false) as GameObject;
            DialogueOption newOption = newOptionObj.GetComponent<DialogueOption>();

            newOption.SetOption(dialogue.options[i], dialogue.dialogueChoices[i], dialogue.MyNPC, dialogue.MainCharacter);
            //Delegar escolhas
        }
    }

    public void CloseDialogueOptTab()
    {
        activeOptionsToChoose = false;
        if (GameManager.gameManager.battleController.ActiveBattle)
            GameManager.gameManager.timeController.EndSlowdown();

        DestroyChilds(dialogueOptionsTab.transform);
        dialogueOptionsTab.SetActive(false);       
    }
    #endregion

    #region Batalha   
    //public GameObject equipableDialoguesTab;

    //public QuickDialogueItemIcon[] battleDialoguesSlots = new QuickDialogueItemIcon[2];
    QuickDialogueItemIcon[] equipableBattleDialogues = new QuickDialogueItemIcon[3];
    QuickDialogueItemIcon equippedDialogueB;

    public GameObject quickDialogueTab;

    public void OpenCloseQuickDialogueTab()
    {
        quickDialogueTab.SetActive(!quickDialogueTab.activeSelf);
    }

    public void EquipDialogue(int idx)
    {
        if (equippedDialogueB != null)
        {
            equippedDialogueB.StopSpriteAnim();
            SetObjSize(equippedDialogueB.gameObject, -15);
        }
        equippedDialogueB = equipableBattleDialogues[idx];
        equippedDialogueB.PlaySpriteAnim();
        SetObjSize(equippedDialogueB.gameObject, 15);
        //Muda somente a cor/sprite do ícone do diálogo.
        /*if (GameManager.gameManager.battleController.ActiveBattle)*/
    }

    public void IconCooldown(float value)
    {
        //equippedDialogueB.Cooldown(value);

        foreach (QuickDialogueItemIcon qd in equipableBattleDialogues)
        {
            qd.Cooldown(value);
        }
    }

    public void UseDialogue(/*int idx*/)
    {
        mainCharacter.UseDialogue(/*idx*/);
    }

    #endregion

    private void Start()
    {
        GameManager.gameManager.MainHud = this;

        equipableBattleDialogues = quickDialogueTab.GetComponentsInChildren<QuickDialogueItemIcon>();
        EquipDialogue(0);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    if (!GameManager.gameManager.battleController.ActiveBattle)
        //        OpenCloseQuickMenu();
        //}
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
            if (actualStorage != null && actualStorage.storageMenu.activeSelf) actualStorage.OpenCloseStorage(false);
            else if (inventory.activeSelf) OpenCloseInventory(false);
            else if (pauseMenu.activeSelf) OpenClosePauseMenu(false);
            else OpenClosePauseMenu(true);
        }

    }

    //public void WaitingForAnswer(bool value)
    //{
    //    equippedDialogueB.GetComponent<Animator>().SetBool("WaitingAnswer", value);
    //}

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
        quickItemSlot.transform.parent.gameObject.SetActive(value);
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
        OpenClosePauseMenu(false);
        inventory.SetActive(value);
        ShowHideQuickItemSlot(!value);
    }

    public void OpenClosePauseMenu(bool value)
    {
        pauseMenu.SetActive(value);
        GameManager.gameManager.timeController.PauseResume(value);
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        GameManager.gameManager.timeController.PauseResume(false);
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

    void DestroyChilds(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
