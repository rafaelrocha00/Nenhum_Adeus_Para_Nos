using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue of battle", menuName = "Dialogue/Dialogue of battle")]
public class DialogueBattle : Dialogue
{
    public enum ApproachType { Seduzir, Ameaçar, Discordar }
    public ApproachType approachType;

    [HideInInspector] INPC targetedNPC;
    public INPC TagetedNPC { get { return targetedNPC; } set { targetedNPC = value; } }

    public override string NextString()
    {
        if (actualID < allStrings.Length - 1)
        {
            GameManager.gameManager.dialogueController.ChangePopUpPos(mainCharacter.transform);
            actualID++;
            return allStrings[actualID];
        }
        else
        {
            GameManager.gameManager.dialogueController.EndDialogue();
            //Mandar para o inimigo/alvo atual o dialogo para ser respondido            
            //mainCharacter.BattleDialoguing = false;
            ResetDialogue();
            targetedNPC.ReceiveBattleDialogue(this);
            return "";
        }
    }
}
