using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [HideInInspector] List<INPC> allEnemyFighters = new List<INPC>();
    public List<INPC> AllEnemyFighters { get { return allEnemyFighters; } }

    [HideInInspector] Player mainCharacter;
    public Player MainCharacter { get { if (mainCharacter == null) FindPlayer(); return mainCharacter; } set { mainCharacter = value; } }

    [HideInInspector] bool activeBattle = false;
    public bool ActiveBattle { get { return activeBattle; } set { activeBattle = value; } }

    [HideInInspector] bool triggeringBattle = false;
    public bool TriggeringBattle { get { return triggeringBattle; } set { triggeringBattle = value; } }

    [SerializeField] float waitTime = 0.0f;
    public float WaitTime { get { return waitTime; } set { waitTime = value; } }
    //Vector3 battleCenter = Vector3.zero;

    public float triggerDistance = 8.0f;
    public float playerMaxDistance;

    [HideInInspector] INPC closestEnemy;
    public INPC ClosetEnemy { get { return closestEnemy; } }

    bool byDialogue = true;

    public GameObject startBattleEffectPrefab;
    ParticleSystem startBattleEffect;

    //Vector3 closestEnemyPos;

    public void StartBattle(List<BattleUnit> fighters)
    {       

        StartCoroutine(DelayedStartBattle(fighters));
    }
    IEnumerator DelayedStartBattle(List<BattleUnit> fighters)
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Starting Battle");
        activeBattle = true;
        allEnemyFighters.Clear();
        for (int i = 0; i < fighters.Count; i++)
        {
            fighters[i].StartBattle(byDialogue);
        }
        //if (allEnemyFighters.Count == 0)
        //{
        //    activeBattle = false;
        //    byDialogue = true;
        //    triggeringBattle = false;
        //    yield break;
        //}

        for (int i = 0; i < allEnemyFighters.Count; i++)
        {
            allEnemyFighters[i].MCharacter = mainCharacter;
        }
        //GameManager.gameManager.MainHud.OpenCloseQuickDialogueTab();
        if (startBattleEffect == null)
        {
            GameObject aux = Instantiate(startBattleEffectPrefab, transform.position, startBattleEffectPrefab.transform.rotation) as GameObject;
            startBattleEffect = aux.GetComponent<ParticleSystem>();
        }
        if (allEnemyFighters.Count != 0)
        {
            StartBattleCamera(fighters[0].GetPos());
            startBattleEffect.transform.position = mainCharacter.transform.position;
            startBattleEffect.Play();
        }
        StartCoroutine("CheckPlayerPos");
        byDialogue = true;
        triggeringBattle = false;
    }

    IEnumerator CheckPlayerPos()
    {
        while (activeBattle)
        {            
            closestEnemy = null;
            for (int i = 0; i < allEnemyFighters.Count; i++)
            {
                if (i == 0) closestEnemy = allEnemyFighters[i];
                else if (mainCharacter != null && (closestEnemy.transform.position - mainCharacter.transform.position).sqrMagnitude > (mainCharacter.transform.position - allEnemyFighters[i].transform.position).sqrMagnitude)
                {
                    closestEnemy = allEnemyFighters[i];
                    //closestEnemyPos = closestEnemy.transform.position;
                }                    
            }
            Vector3 toEnemyVector = Vector3.zero;

            if (closestEnemy != null) toEnemyVector = closestEnemy.transform.position - mainCharacter.transform.position;

            if (closestEnemy == null || toEnemyVector.sqrMagnitude >= playerMaxDistance * playerMaxDistance)
                EndAllFightersBattle();
            else GameManager.gameManager.MainCamera.SetTarget(toEnemyVector * 0.5f + mainCharacter.transform.position);
            yield return new WaitForEndOfFrame();
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
        //GameManager.gameManager.MainHud.OpenCloseQuickDialogueTab();        
        Debug.Log("BattleEnd");
        mainCharacter.EndBattle();
        for (int i = 0; i < allEnemyFighters.Count; i++)
        {
            allEnemyFighters[i].EndBattle();
        }

        allEnemyFighters.Clear();

        GameManager.gameManager.dialogueController.EndDialogue();
        GameManager.gameManager.MainCamera.EndBattle();
    }

    public void TriggerHostileNPCs(Vector3 initialPoint, float radius = 0)
    {
        if (radius == 0) radius = triggerDistance;        
        Collider[] collidersWithin = Physics.OverlapSphere(initialPoint, radius);
        List<BattleUnit> fighters = new List<BattleUnit>();
        for (int i = 0; i < collidersWithin.Length; i++)
        {
            //try
            //{
            //    Debug.Log(collidersWithin[i].name);
            //    BattleUnit aux = collidersWithin[i].GetComponent<BattleUnit>();
            //    fighters.Add(aux);//.StartBattle(false);
            //}
            //catch
            //{
            //    Debug.Log("Não é uma battleUnit");
            //}
            Debug.Log(collidersWithin[i].name);
            BattleUnit aux = collidersWithin[i].GetComponent<BattleUnit>();
            if (aux != null) fighters.Add(aux);//.StartBattle(false);
            else Debug.Log("Não é uma battleunit");
        }
        if (EnoughNPCs(fighters))
        {
            Debug.Log("tem");
            triggeringBattle = true;
            byDialogue = false;
            StartBattle(fighters);
        }
    }

    public bool EnoughNPCs(List<BattleUnit> fighters)
    {
        if (fighters.Count > 0)
        {
            //if (mainCharacter == null) mainCharacter = GameObject.Find("Player").GetComponent<Player>();
            FindPlayer();
            mainCharacter.DelayStartBattle();
            return true;
        }
        else return false;
    }

    public void FindAndRemove(string fighterName)
    {
        if (activeBattle)
        {
            for (int i = 0; i < allEnemyFighters.Count; i++)
            {
                if (fighterName.Equals(allEnemyFighters[i].name)) allEnemyFighters.RemoveAt(i);
            }
        }
    }

    public void StartBattleCamera(Vector3 reference)
    {
        FindPlayer();
        Vector3 toEnemyVector = reference - mainCharacter.transform.position;
        Vector3 cameraTarget = toEnemyVector.normalized * toEnemyVector.magnitude * 0.25f + mainCharacter.transform.position;
        GameManager.gameManager.MainCamera.StartBattle(cameraTarget);
    }

    void FindPlayer()
    {
       if (mainCharacter == null) mainCharacter = GameObject.Find("Player").GetComponent<Player>();
    }
}
