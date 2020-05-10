using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue quest trigger", menuName = "Dialogue/Dialogue quest trigger")]
public class DialogueQuestTrigger : Dialogue
{
    public Quest quest;

    public override string NextString()
    {
        if (actualID < allStrings.Length - 1)
        {
            actualID++;
            CheckMCStrings();
            return allStrings[actualID];
        }
        else
        {
            GameManager.gameManager.dialogueController.EndDialogue();
            myNPC.EndDialogue();

            quest.AcceptQuest();
            myNPC.MyQuestAccepted = true;
            ResetDialogue();
            return "";
        }
    }
}
