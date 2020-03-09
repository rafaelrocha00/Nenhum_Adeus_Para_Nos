using System.Collections;
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
        actualID = -1;
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
            myNPC.EndDialogue();
            return "";
        }
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
        if (mcSpeak) GameManager.gameManager.dialogueUIMain.ChangePopUpPos(mainCharacter.transform.position);
        else GameManager.gameManager.dialogueUIMain.ChangePopUpPos(myNPC.transform.position);
    }
}
