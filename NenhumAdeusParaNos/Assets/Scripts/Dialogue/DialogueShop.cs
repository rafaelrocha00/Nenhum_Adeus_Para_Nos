﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue shop", menuName = "Dialogue/Dialogue shop")]
public class DialogueShop : Dialogue
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

            try
            {
                NPC aux = (NPC)myNPC;
                aux.GetComponent<Shop>().OpenShop();
            }
            catch { }

            ResetDialogue();
            CheckPostDialogueActions();
            return "";
        }
    }
}