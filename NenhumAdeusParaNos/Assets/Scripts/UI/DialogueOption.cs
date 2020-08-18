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

    public void SetOption(string text, int id, Dialogue dialogue, INPC theNPC, Player player)
    {
        optionText = GetComponent<Text>();

        if (!text.Equals(""))
        {
            optionText.text = (id + 1).ToString() + ". " + text;
            GetComponent<Button>().enabled = true;
        }
        else GetComponent<Button>().enabled = false;
        myDialogue = dialogue;
        actualNPC = theNPC;

        dialogue.MyNPC = theNPC;
        dialogue.MainCharacter = player;
    }

    public void CleanText()
    {
        optionText.text = "";
    }

    public void Choose()
    {
        //actualNPC.ChangeDialogue(myDialogue);
        //actualNPC.NextString();
        GameManager.gameManager.dialogueController.ChooseOption(myDialogue, myDialogue.MyNPC.expressions[1]);          
    }
}
