using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [HideInInspector] List<INPC> allEnemyFighters = new List<INPC>();
    public List<INPC> AllEnemyFighters { get { return allEnemyFighters; } }

    [HideInInspector] Player mainCharacter;
    public Player MainCharacter { get { return mainCharacter; } set { mainCharacter = value; } }

    [HideInInspector] bool activeBattle = false;
    public bool ActiveBattle { get { return activeBattle; } set { activeBattle = value; } }

    public void StartBattle()
    {
        activeBattle = true;
        for (int i = 0; i < allEnemyFighters.Count; i++)
        {
            allEnemyFighters[i].MCharacter = mainCharacter;
        }
    }

    public void AddFighter(INPC battleNPC)
    {
        allEnemyFighters.Add(battleNPC);
    }

    public void CheckForBattleEnd()
    {
        if (!mainCharacter.CanFight())
        {
            EndAllFightersBattle();
            return;
        }

        int aliveCount = 0;
        for (int i = 0; i < allEnemyFighters.Count; i++)
        {
            if (allEnemyFighters[i].CanFight()) aliveCount++;
        }
        if (aliveCount == 0) EndAllFightersBattle();
    }

    public void EndAllFightersBattle()
    {
        activeBattle = false;
        Debug.Log("BattleEnd");
        mainCharacter.EndBattle();
        for (int i = 0; i < allEnemyFighters.Count; i++)
        {
            allEnemyFighters[i].EndBattle();
        }

        allEnemyFighters.Clear();
    }
}
