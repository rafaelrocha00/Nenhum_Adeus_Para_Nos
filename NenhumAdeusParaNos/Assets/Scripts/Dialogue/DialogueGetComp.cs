using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue get companion", menuName = "Dialogue/Dialogue get companion")]
public class DialogueGetComp : Dialogue
{
    public GameObject compPref;

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
            myNPC.OnExit(mainCharacter);

            GameObject aux = Instantiate(compPref, myNPC.GetTransform().position, myNPC.GetTransform().rotation) as GameObject;
            GameManager.gameManager.PlayerCompanionsPref.Add(compPref);
            //mainCharacter.MyCompanion = aux.GetComponent<Companion>();

            mainCharacter.MyCompanions.Add(aux.GetComponent<Companion>());
            myNPC.GetTransform().gameObject.SetActive(false);
            ResetDialogue();
            CheckPostDialogueActions();
            return "";
        }
    }
}
