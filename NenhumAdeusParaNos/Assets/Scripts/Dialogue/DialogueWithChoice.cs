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
            //if (actualID == allStrings.Length - 2) lastString = true;
            return allStrings[actualID];
        }
        else if (actualID < allStrings.Length - 1)
        {
            //myNPC.EndDialogue();
            actualID++;
            CheckMCStrings();
            GameManager.gameManager.dialogueController.OpenDialogueOptTab(this);
            lastString = true;
            int i = actualID;
            ResetDialogue();
            return allStrings[i];
            //return "";
        }
        else return "";
    }

    public bool HasThisIndex(int id)
    {
        if (options[id].Equals("")) return false;
        else return true;
    }

    public DialogueQuestTrigger SearchQuestDialogue()
    {
        for (int i = 0; i < dialogueChoices.Length; i++)
        {
            if (dialogueChoices[i] is DialogueQuestTrigger)
            {
                return (DialogueQuestTrigger)dialogueChoices[i];
            }
        }
        return null;
    }
}
