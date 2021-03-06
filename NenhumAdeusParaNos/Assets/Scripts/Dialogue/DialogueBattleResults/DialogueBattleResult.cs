﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Battle Result", menuName = "Dialogue/Dialogue Battle Result")]
public class DialogueBattleResult : Dialogue
{
    //public DialogueBattle.ApproachType[] apCombination = new DialogueBattle.ApproachType[3];
    public Battle_Effect.BattleEffect[] posEffects;
    public float[] chancePercentages;//Considerando as maiores porcentages no final e as menores no inicio
    public bool life_staminaCond;
    public float life_staminaRequiredPerc;

    [HideInInspector] float percCond = 100;
    public float PercCond { set { percCond = value; } }

    bool startedEffects;

    Battle_Effect.BattleEffect chosenEffect;

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
            //ResetDialogue();
            CheckPostDialogueActions();
            BattleResult();
            return "";
        }
    }

    public void StartEffects(INPC npc, Player p)
    {
        if (!startedEffects)
        {
            for (int i = 0; i < posEffects.Length; i++)
            {
                posEffects[i].Inialize(npc, p);
            }

            startedEffects = true;
        }
    }
    private void OnDisable()
    {
        startedEffects = false;
    }

    public bool GetResult()
    {
        Debug.Log("Efeito");
        bool aux = true;
        if (life_staminaCond) if (percCond <= life_staminaRequiredPerc) aux = false;

        if (aux)
        {
            float random = Random.Range(0.0f, 100.0f);
            for (int i = 0; i < posEffects.Length; i++)
            {
                if (posEffects[i] == null) return false;

                if (random < chancePercentages[i])
                {
                    chosenEffect = posEffects[i];
                    return true;
                }
            }
        }

        return false;
    }

    void BattleResult()
    {
        chosenEffect.Effect();
    }
}
