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

    //Vector3 battleCenter = Vector3.zero;

    public float triggerDistance = 8.0f;
    public float playerMaxDistance;

    public void StartBattle()
    {
        Debug.Log("Starting Battle");
        activeBattle = true;
        for (int i = 0; i < allEnemyFighters.Count; i++)
        {
            allEnemyFighters[i].MCharacter = mainCharacter;
        }
        GameManager.gameManager.MainHud.OpenCloseQuickDialogueTab();
        StartCoroutine("CheckPlayerPos");
    }

    IEnumerator CheckPlayerPos()
    {
        while (activeBattle)
        {
            yield return new WaitForSeconds(1);
            for (int i = 0; i < allEnemyFighters.Count; i++)
            {
                if ((mainCharacter.transform.position - allEnemyFighters[i].transform.position).sqrMagnitude >= playerMaxDistance * playerMaxDistance)
                    EndAllFightersBattle();
            }
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
        GameManager.gameManager.MainHud.OpenCloseQuickDialogueTab();        
        Debug.Log("BattleEnd");
        mainCharacter.EndBattle();
        for (int i = 0; i < allEnemyFighters.Count; i++)
        {
            allEnemyFighters[i].EndBattle();
        }

        allEnemyFighters.Clear();
    }

    public void TriggerHostileNPCs(Vector3 initialPoint, float radius = 0)
    {
        if (radius == 0) radius = triggerDistance;
        Collider[] collidersWithin = Physics.OverlapSphere(initialPoint, radius);
        for (int i = 0; i < collidersWithin.Length; i++)
        {
            try
            {
                Debug.Log(collidersWithin[i].name);
                collidersWithin[i].GetComponent<BattleUnit>().StartBattle(false);
            }
            catch
            {
                Debug.Log("Não é uma battleUnit");
            }
        }
        if (GameManager.gameManager.battleController.EnoughNPCs())
        {
            GameManager.gameManager.battleController.StartBattle();
        }
    }

    public bool EnoughNPCs()
    {
        if (allEnemyFighters.Count > 0)
        {
            if (mainCharacter == null) mainCharacter = GameObject.Find("Player").GetComponent<Player>();
            mainCharacter.DelayStartBattle();
            return true;
        }
        else return false;
    }
}
