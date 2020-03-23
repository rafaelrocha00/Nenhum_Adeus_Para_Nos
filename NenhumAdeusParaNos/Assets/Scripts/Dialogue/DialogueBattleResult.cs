using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogueBattleResult : Dialogue
{

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
            ResetDialogue();
            BattleResult();
            return "";
        }
    }

    public abstract void BattleResult();
}
