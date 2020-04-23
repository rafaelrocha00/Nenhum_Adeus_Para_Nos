using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue end battle", menuName = "Dialogue/Dialogue end battle")]
public class DialogueEndBattle : DialogueBattleResult
{

    //public override string NextString()
    //{
    //    if (actualID < allStrings.Length - 1)
    //    {
    //        actualID++;
    //        CheckMCStrings();
    //        return allStrings[actualID];
    //    }
    //    else
    //    {
    //        GameManager.gameManager.dialogueController.EndDialogue();
    //        myNPC.EndDialogue();
    //        ResetDialogue();
    //        GameManager.gameManager.battleController.EndAllFightersBattle();
    //        return "";
    //    }
    //}

    //public override void BattleResult()
    //{
    //    GameManager.gameManager.battleController.EndAllFightersBattle();
    //}
}
