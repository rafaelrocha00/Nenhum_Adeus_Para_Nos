using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainHud : MonoBehaviour
{
    [HideInInspector] Player mainCharacter;
    public Player MainCharacter { get { return mainCharacter; } set { mainCharacter = value; } }

    public GameObject quickMenu;
    [HideInInspector] bool isQuickMenuActive;
    public bool IsQuickMenuActive { get { return isQuickMenuActive; } }

    public GameObject dialogueOptionsTab;
    public GameObject dialogueOptionPref;

    [HideInInspector] bool activeOptionsToChoose;
    public bool ActiveOptionsToChoose { get { return activeOptionsToChoose; } }

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

    void TryChangeBattleDialogue(int idx)
    {
        BattleDialogueB aux = null;
        for (int i = 0; i < equipablesBattleDialogues.Length; i++)
        {
            if (equipablesBattleDialogues[i].IsMouseOverMe)
            {
                aux = equipablesBattleDialogues[i];
                break;
            }
        }
        if (aux != null)
        {
            ChangeBattleDialogue(idx, aux.DialogueB, aux.GetComponent<Image>().color);
        }
    }

    //Mudar para sprite
    public void ChangeBattleDialogue(int idx, DialogueBattle newDialogue, Color newIcon)
    {
        int aux = (idx == 0)? 1 : 0;
        if (battleDialoguesSlots[aux].DialogueB != newDialogue)
        {
            battleDialoguesSlots[idx].SetDialogue(newDialogue, newIcon);
        }
    }

    public DialogueBattle GetDialogueFromSlot(int idx)
    {
        return battleDialoguesSlots[idx].UseMyDialogue();
    }

    public void UseDialogue(int idx)
    {
        mainCharacter.UseDialogue(idx);
    }

    #endregion

    private void Awake()
    {
        GameManager.gameManager.MainHud = this;
    }
    private void Start()
    {
        equipablesBattleDialogues = equipableDialoguesTab.GetComponentsInChildren<BattleDialogueB>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            OpenCloseQuiCkMenu();
        }

        if (isQuickMenuActive)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                TryChangeBattleDialogue(0);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                TryChangeBattleDialogue(1);
            }
        }
    }

    public void OpenCloseQuiCkMenu()
    {
        isQuickMenuActive = !isQuickMenuActive;
        quickMenu.SetActive(isQuickMenuActive);

        if (GameManager.gameManager.battleController.ActiveBattle)
        {
            if (IsQuickMenuActive) GameManager.gameManager.timeController.StartSlowdown();
            else GameManager.gameManager.timeController.EndSlowdown();
        }
    }

    void DestroyChilds(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
