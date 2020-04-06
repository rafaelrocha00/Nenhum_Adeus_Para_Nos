using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainHud : MonoBehaviour
{
    [HideInInspector] Player mainCharacter;
    public Player MainCharacter { get { return mainCharacter; } set { mainCharacter = value; } }

    public GameObject gameOverScreen;

    public GameObject quickMenu;
    [HideInInspector] bool isQuickMenuActive;
    public bool IsQuickMenuActive { get { return isQuickMenuActive; } }

    public GameObject diaryMenu;

    public GameObject dialogueOptionsTab;
    public GameObject dialogueOptionPref;

    [HideInInspector] bool activeOptionsToChoose;
    public bool ActiveOptionsToChoose { get { return activeOptionsToChoose; } }

    public Image staminaBar;
    public Image lifeBar;
    public Image defenseBar;
    //public int[] battleDialogues = new int[5];

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
    public GameObject equipableDialoguesTab;

    public BattleDialogueB[] battleDialoguesSlots = new BattleDialogueB[2];
    BattleDialogueB[] equipablesBattleDialogues;

    public BattleDialogueB equippedDialogueB;

    public GameObject quickDialogueTab;

    public void OpenCloseQuickDialogueTab()
    {
        quickDialogueTab.SetActive(!quickDialogueTab.activeSelf);
    }

    public void EquipDialogue(int idx)
    {
        //Muda somente a cor/sprite do ícone do diálogo.
        if (GameManager.gameManager.battleController.ActiveBattle) equippedDialogueB.SetIcon(GameManager.gameManager.dialogueController.dialogueColors[idx]);
    }

    public void IconCooldown(float value)
    {
        equippedDialogueB.Cooldown(value);
    }

    public void UseDialogue(/*int idx*/)
    {
        mainCharacter.UseDialogue(/*idx*/);
    }

    #endregion

    private void Awake()
    {
        GameManager.gameManager.MainHud = this;
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

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
    }

    void DestroyChilds(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
