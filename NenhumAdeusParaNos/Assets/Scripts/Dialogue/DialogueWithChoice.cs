using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue with choice", menuName = "Dialogue/Dialogue with choice")]
public class DialogueWithChoice : Dialogue
{
    public Dialogue[] dialogueChoices = new Dialogue[3];
    public string[] options;

    [HideInInspector] bool lastString = false;
    public bool LastString { get { return lastString; } }

    public override void ResetDialogue()
    {
        base.ResetDialogue();
        lastString = false;
    }

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
            //GameManager.gameManager.MainHud.OpenDialogueOptTab(this);
            lastString = true;
            return allStrings[actualID];
            //return "";
        }
        else return "";
    }
}
