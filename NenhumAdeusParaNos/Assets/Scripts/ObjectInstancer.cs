using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInstancer : MonoBehaviour
{
    public Transform enemyLocationObj;
    //public Transform[] enemyLocations;
    public Transform chestLocations;

    ObjectSpawnPoint[] enemieSpawnPoints;
    ObjectSpawnPoint chestSpawnPoint;

    private void Start()
    {
        GameManager.gameManager.questController.InstantiateQuestObjs(this);
    }

    public void SpawnEnemyGroup(string locName, GameObject[] enems)
    {
        bool canspawn = false;
        for (int i = 0; i < enemyLocationObj.childCount; i++)
        {
            if (locName.Equals(enemyLocationObj.GetChild(i).name)) canspawn = true; 
        }
        if (!canspawn) return;

        enemieSpawnPoints = enemyLocationObj.Find(locName).GetComponentsInChildren<ObjectSpawnPoint>();

        try
        {
            for (int i = 0; i < enems.Length; i++)
            {
                enems[i].GetComponent<INPC>().AreaName = locName;
                for (int j = 0; j < enemieSpawnPoints.Length; j++)
                {
                    if (!enemieSpawnPoints[j].Used)
                    {
                        enemieSpawnPoints[j].SpawnObject(enems[i]);
                        break;
                    }
                }
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public void SpawnSingleEnemy(string locName, string tarName, GameObject enem)
    {
        bool canspawn = false;
        for (int i = 0; i < enemyLocationObj.childCount; i++)
        {
            if (locName.Equals(enemyLocationObj.GetChild(i).name)) canspawn = true;
        }
        if (!canspawn) return;

        enemieSpawnPoints = enemyLocationObj.Find(locName).GetComponentsInChildren<ObjectSpawnPoint>();

        try
        {
            enem.GetComponent<INPC>().AreaName = locName;
            enem.GetComponent<INPC>().Name = tarName;
            for (int i = 0; i < enemieSpawnPoints.Length; i++)
            {
                if (!enemieSpawnPoints[i].Used)
                {
                    enemieSpawnPoints[i].SpawnObject(enem);
                    return;
                }
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public void SpawnChest(string locName, GameObject chest)
    {
        chestSpawnPoint = chestLocations.Find(locName).GetComponent<ObjectSpawnPoint>();

        try
        {
            chest.GetComponent<Storage>().Name = locName;
            if (!chestSpawnPoint.Used) chestSpawnPoint.SpawnObject(chest);
        }
        catch (System.Exception)
        {

            throw;
        }
    }
}
