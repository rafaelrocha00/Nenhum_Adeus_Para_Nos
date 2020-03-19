using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOption : MonoBehaviour
{
    Text optionText;

    Dialogue myDialogue;
    INPC actualNPC;


    private void Start()
    {
        optionText = GetComponent<Text>();
    }

    public void SetOption(string text, Dialogue dialogue, INPC theNPC, Player player)
    {
        optionText = GetComponent<Text>();

        optionText.text = text;
        myDialogue = dialogue;
        actualNPC = theNPC;

        dialogue.MyNPC = theNPC;
        dialogue.MainCharacter = player;
    }

    public void Choose()
    {
        //actualNPC.ChangeDialogue(myDialogue);
        //actualNPC.NextString();
        GameManager.gameManager.MainHud.CloseDialogueOptTab();
        GameManager.gameManager.dialogueController.ChangeDialogue(myDialogue);        
    }
}
