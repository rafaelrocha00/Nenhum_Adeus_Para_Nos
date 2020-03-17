using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogueB : MonoBehaviour
{
    [HideInInspector] DialogueBattle thisDialogueBattle;
    public DialogueBattle DialogueB { get { return thisDialogueBattle; } set { thisDialogueBattle = value; } }

    Button thisButton;

    private void Start()
    {
        thisButton = GetComponent<Button>();
    }


}
