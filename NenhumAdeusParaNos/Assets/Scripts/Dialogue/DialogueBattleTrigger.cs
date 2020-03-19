﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue battle trigger", menuName = "Dialogue/Dialogue battle trigger")]
public class DialogueBattleTrigger : Dialogue
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

            GameManager.gameManager.battleController.StartBattle();
            myNPC.StartBattle();
            mainCharacter.StartBattle();
            ResetDialogue();
            return "";
        }
    }
}