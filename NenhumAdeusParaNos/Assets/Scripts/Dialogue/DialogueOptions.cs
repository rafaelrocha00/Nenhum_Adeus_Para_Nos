﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Options", menuName = "Dialogue/Dialogue Options")]
public class DialogueOptions : ScriptableObject
{
    public Dialogue[] dialogueOp;
    int lastID = -1;

    private void OnDisable()
    {
        lastID = -1;
    }

    public Dialogue GetRandomDialogue()
    {
        if (dialogueOp != null && dialogueOp.Length > 0)
        {
            int random = Random.Range(0, dialogueOp.Length);
            if (dialogueOp.Length > 1 && lastID >= 0)
            {
                while (random == lastID)
                {
                    random = Random.Range(0, dialogueOp.Length);
                }
                lastID = random;
            }
            
            return dialogueOp[random];
        }
        else return null;
    }
}
