﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    [TextArea] public string[] allStrings;
    public int[] mcStrings; //Um array de Ints que representa quais os Index do allStrings deste dialogo em que o player vai ser quem fala;

    protected int actualID = -1;

    [HideInInspector] protected INPC myNPC;
    public INPC MyNPC { get { return myNPC; } set { myNPC = value; } }
    [HideInInspector] protected Player mainCharacter;
    public Player MainCharacter { get { return mainCharacter; } set { mainCharacter = value; } }

    protected bool mcSpeak = false;

    private void OnDisable()
    {
        ResetDialogue();
    }

    public void ResetDialogue()
    {
        actualID = -1;
    }

    public virtual string NextString()
    {
        if (actualID < allStrings.Length - 1)
        {
            actualID++;
            CheckMCStrings();
            return allStrings[actualID];
        }
        else
        {
            //Debug.Log("Ending Dialogue");
            GameManager.gameManager.dialogueController.EndDialogue();
            myNPC.EndDialogue();
            ResetDialogue();
            return "";
        }
    }

    public string GetActualString()
    {
        if (actualID >= 0)
            return allStrings[actualID];
        else
            return allStrings[0];
    }

    protected void CheckMCStrings()
    {
        for (int i = 0; i < mcStrings.Length; i++)
        {
            if (actualID == mcStrings[i])
            {
                mcSpeak = true;
                break;
            }
            else mcSpeak = false;
        }
        if (mcSpeak) GameManager.gameManager.dialogueController.ChangePopUpPos(mainCharacter.transform);
        else GameManager.gameManager.dialogueController.ChangePopUpPos(myNPC.transform);
    }
}
