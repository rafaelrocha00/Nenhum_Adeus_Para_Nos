﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGenerator : MonoBehaviour
{
    int actualID = 0;
    public int[] jobsChances = new int[3];
    //bool exeLimit = false;

    bool firstGen = true;

    public List<string> contractorNames = new List<string>();
    #region KillQuests
    public List<string> locationNames = new List<string>();
    public List<string> possibleTargetNames = new List<string>();
    List<GameObject> enemy_communist = new List<GameObject>();
    List<GameObject> enemy_capitalist = new List<GameObject>();
    List<GameObject> enemy_lustrous = new List<GameObject>();
    List<GameObject> enemy_normal = new List<GameObject>();
    public GameObject[] enemy_all;
    #endregion

    #region DeliveryQuest
    public List<Item> itemsToDel = new List<Item>();
    public List<string> storageNames = new List<string>();
    public GameObject[] chests;
    #endregion

    #region RepairQuest
    public List<string> objectsToRepair = new List<string>();
    #endregion

    public Queue<Quest> quest_queue = new Queue<Quest>();
    //public List<Quest> quest_nonAccepteds = new List<Quest>();

    public void Start()
    {
        jobsChances[0] = 34;
        jobsChances[1] = 33;
        jobsChances[2] = 33;

        for (int i = 0; i < enemy_all.Length; i++)
        {
            INPC.EnemyType et = enemy_all[i].GetComponent<INPC>().enemyType;
            INPC.Faction fac = enemy_all[i].GetComponent<INPC>().faction;
            if (et == INPC.EnemyType.Lustro) enemy_lustrous.Add(enemy_all[i]);
            else if (et == INPC.EnemyType.Normal) enemy_normal.Add(enemy_all[i]);
            else if (fac == INPC.Faction.Communist) enemy_communist.Add(enemy_all[i]);
            else enemy_capitalist.Add(enemy_all[i]);
            Debug.Log(et + " | " + fac);
        }

        if (firstGen)
        {
            firstGen = false;
            GenKilQuest();
            GenDelQuest();
            GenRepQuest();
        }
    }

    public void IncreaseChance(int idx)
    {
        if (jobsChances[idx] < 80)
        {
            int leftover = 0;
            jobsChances[idx] += 4;
            leftover = Mathf.Clamp(jobsChances[idx] - 80, 0, 3);
            jobsChances[idx] = Mathf.Clamp(jobsChances[idx], 10, 80);
            int othersDec = 4 - leftover;

            while (othersDec > 0)
            {
                int aux = othersDec;
                leftover = othersDec % 2;
                for (int i = 0; i < jobsChances.Length; i++)
                {
                    if (i != idx && jobsChances[i] > 10)
                    {
                        jobsChances[i] -= aux / 2 + leftover;
                        othersDec -= aux / 2 + leftover;
                        if (leftover > 0) leftover -= leftover;

                        othersDec += Mathf.Clamp(10 - jobsChances[i], 0, 3);
                        jobsChances[i] = Mathf.Clamp(jobsChances[i], 10, 80);
                    }
                }
            }
        }

        Debug.Log(jobsChances[0] + " | " + jobsChances[1] + " | " + jobsChances[2]);
    }
    public void DecreaseChance(int idx)
    {
        if (jobsChances[idx] > 10)
        {
            int leftover = 0;
            jobsChances[idx] -= 2;
            leftover = Mathf.Clamp(10 - jobsChances[idx], 0, 3);
            jobsChances[idx] = Mathf.Clamp(jobsChances[idx], 10, 80);
            int othersInc = 2 - leftover;

            while (othersInc > 0)
            {
                int aux = othersInc;
                leftover = othersInc % 2;
                for (int i = 0; i < jobsChances.Length; i++)
                {
                    if (i != idx && jobsChances[i] < 80)
                    {
                        jobsChances[i] += aux / 2 + leftover;
                        othersInc -= aux / 2 + leftover;
                        if (leftover > 0) leftover -= leftover;

                        othersInc += Mathf.Clamp(10 - jobsChances[i], 0, 3);
                        jobsChances[i] = Mathf.Clamp(jobsChances[i], 10, 80);
                    }
                }
            }
        }

        Debug.Log(jobsChances[0] + " | " + jobsChances[1] + " | " + jobsChances[2]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            IncreaseChance(Random.Range(0, 3));
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            try { Debug.Log(quest_queue.Peek().Name); }
            catch { }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            GenQuest();
        }
    }

    public void GenQuest()
    {
        if (quest_queue.Count < 9)
        {
            float aux = Random.Range(0, 100);

            if (aux < jobsChances[0])
            {
                GenKilQuest();
                return;
            }
            aux = Random.Range(0, 100 - jobsChances[0]);
            if (aux < jobsChances[1])
            {
                GenDelQuest();
                return;
            }
            GenRepQuest();
        }
    }

    public void GenKilQuest()
    {
        Debug.Log("Gerando KILL quest");
        int rand = Random.Range(0, 2);
        Quest newQuest;
        if (rand == 0)
        {
            newQuest = ScriptableObject.CreateInstance("KillQuest") as Quest;
            KillQuest kq = (KillQuest)newQuest;

            kq.AreaName = locationNames[Random.Range(0, locationNames.Count)];
            rand = Random.Range(0, 2);
            string etype = "";
            List<GameObject> toInst = new List<GameObject>();
            if (rand == 0)
            {
                rand = Random.Range(1, 11);                

                kq.faction = (INPC.Faction)Random.Range(0, 2);
                if (kq.faction == INPC.Faction.Communist)
                {
                    etype = "Comunistas";
                    for (int i = 0; i < rand; i++)
                        toInst.Add(enemy_communist[Random.Range(0, enemy_communist.Count)]);
                }
                else
                {
                    etype = "Capitalistas";
                    for (int i = 0; i < rand; i++)
                        toInst.Add(enemy_capitalist[Random.Range(0, enemy_capitalist.Count)]);
                }
            }
            else
            {
                rand = Random.Range(1, 11);

                kq.enemyType = 0;//(INPC.EnemyType)Random.Range(0, 2);
                if (kq.enemyType == INPC.EnemyType.Lustro)
                {
                    etype = "Lustros";
                    for (int i = 0; i < rand; i++)
                        toInst.Add(enemy_lustrous[Random.Range(0, enemy_lustrous.Count)]);
                }
                else
                {
                    etype = "Cidadãos";
                    for (int i = 0; i < rand; i++)
                        toInst.Add(enemy_normal[Random.Range(0, enemy_normal.Count)]);
                }
            }
            kq.MoneyReward = 100 * rand;
            kq.ResourceReward = 10 * rand;
            kq.QuantToKill = rand;
            kq.toInstantiate = toInst.ToArray();

            kq.Name = "Elimine um grupo de " + etype;
            kq.Description = "Eliminar um grupo de " + etype + " em " + kq.AreaName;
        }
        else
        {
            newQuest = ScriptableObject.CreateInstance("AssassinQuest") as Quest;
            AssassinQuest aq = (AssassinQuest)newQuest;
            aq.TargetName = possibleTargetNames[Random.Range(0, possibleTargetNames.Count)];
            aq.AreaName = locationNames[Random.Range(0, locationNames.Count)];

            aq.Name = "Elimine um alvo";
            aq.Description = "Eliminar " + aq.TargetName + ", Paradeiro: " + aq.AreaName;
            aq.MoneyReward = 500;
            aq.ResourceReward = 50;
            aq.toInstantiate = enemy_all[Random.Range(0, enemy_all.Length)];
        }

        DefaultSet(newQuest);
    }
    public void GenDelQuest()
    {
        Debug.Log("Gerando DELIVERY quest");
        DeliveryQuest newQuest = ScriptableObject.CreateInstance("DeliveryQuest") as DeliveryQuest;
        int rand = Random.Range(1, 3);
        newQuest.itemsToDelivery = new Item[rand];
        newQuest.itemsQuant = new int[rand];
        string itemsTD = "";
        int reward = 0;
        for (int i = 0; i < rand; i++)
        {
            if (rand > 1 && i == rand - 1) itemsTD += "& ";
            newQuest.itemsToDelivery[i] = itemsToDel[Random.Range(0, itemsToDel.Count)];
            newQuest.itemsQuant[i] = Random.Range(1, 11);
            itemsTD += newQuest.itemsToDelivery[i].itemName + " " + newQuest.itemsQuant[i] + "x ";

            reward += newQuest.itemsToDelivery[i].slotSize.x * newQuest.itemsToDelivery[i].slotSize.y * newQuest.itemsQuant[i];
        }
        newQuest.DepositName = storageNames[Random.Range(0, storageNames.Count)];        

        newQuest.Name = "Entregue umas coisas";
        newQuest.Description = "Entregar: " + itemsTD + ", Local: " + newQuest.DepositName;
        newQuest.MoneyReward = 100 * reward;
        newQuest.ResourceReward = 20 * reward;
        newQuest.toInstantiate = chests[Random.Range(0, chests.Length)];

        DefaultSet(newQuest);
    }
    public void GenRepQuest()
    {
        Debug.Log("Gerando REPAIR quest");
        RepairQuest newQuest = ScriptableObject.CreateInstance("RepairQuest") as RepairQuest;

        newQuest.ObjectToRepair = objectsToRepair[Random.Range(0, objectsToRepair.Count)];

        newQuest.Name = "Conserte um negócio aí";
        newQuest.Description = "Consertar: " + newQuest.ObjectToRepair;
        newQuest.MoneyReward = 80;
        newQuest.ResourceReward = 8;

        DefaultSet(newQuest);
    }

    void DefaultSet(Quest _q)
    {
        _q.ID = actualID;
        _q.resourceT = (CompanyController.ResourceType)Random.Range(0, 4);
        _q.Contractor = contractorNames[Random.Range(0, contractorNames.Count)];
        _q.LimitDay = Random.Range(0, 7);

        actualID++;

        quest_queue.Enqueue(_q);
        //quest_nonAccepteds.Add(_q);
    }
}
