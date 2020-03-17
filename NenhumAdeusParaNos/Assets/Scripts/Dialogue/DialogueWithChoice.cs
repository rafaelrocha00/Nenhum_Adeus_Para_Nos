using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue with choice", menuName = "Dialogue/Dialogue with choice")]
public class DialogueWithChoice : Dialogue
{
    public Dialogue[] dialogueChoices;
    public string[] options;

    public override string NextString()
    {
        if (actualID < allStrings.Length - 2)
        {
            actualID++;
            CheckMCStrings();
            return allStrings[actualID];
        }
        else if (actualID < allStrings.Length - 1)
        {
            //myNPC.EndDialogue();
            actualID++;
            CheckMCStrings();
            GameManager.gameManager.mainHud.OpenDialogueOptTab(this);
            return allStrings[actualID];
        }
        else return "";
    }
}
