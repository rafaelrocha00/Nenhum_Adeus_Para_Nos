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

            //myNPC.StartBattle();
            //mainCharacter.StartBattle();
            //List<BattleUnit> aux = new List<BattleUnit>();
            //aux.Add(myNPC);
            //aux.Add(mainCharacter);
            //GameManager.gameManager.battleController.StartBattle(aux);
            GameObject aux = Instantiate(compPref, myNPC.transform.position, myNPC.transform.rotation) as GameObject;
            mainCharacter.MyCompanion = aux.GetComponent<Companion>();
            myNPC.gameObject.SetActive(false);
            ResetDialogue();
            return "";
        }
    }
}
