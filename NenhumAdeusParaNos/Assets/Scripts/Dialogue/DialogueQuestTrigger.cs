using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue quest trigger", menuName = "Dialogue/Dialogue quest trigger")]
public class DialogueQuestTrigger : Dialogue
{
    public Quest quest;
    public bool accepting = true;

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

            if (accepting) quest.AcceptQuest();
            else quest.Complete();
            //myNPC.MyQuestAccepted = true;
            ResetDialogue();
            CheckPostDialogueActions();
            return "";
        }
    }
}
