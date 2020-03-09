using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    List<INPC> allFighters = new List<INPC>();

    [HideInInspector] Player mainCharacter;
    public Player MainCharacter { get { return mainCharacter; } set { mainCharacter = value; } }

    public void AddFighter(INPC battleNPC)
    {
        allFighters.Add(battleNPC);
    }

    public void CheckForBattleEnd()
    {
        if (!mainCharacter.CanFight())
        {
            EndAllFightersBattle();
            return;
        }

        int aliveCount = 0;
        for (int i = 0; i < allFighters.Count; i++)
        {
            if (allFighters[i].CanFight()) aliveCount++;
        }
        if (aliveCount == 0) EndAllFightersBattle();
    }

    public void EndAllFightersBattle()
    {
        Debug.Log("BattleEnd");
        mainCharacter.EndBattle();
        for (int i = 0; i < allFighters.Count; i++)
        {
            allFighters[i].EndBattle();
        }

        allFighters.Clear();
    }
}
