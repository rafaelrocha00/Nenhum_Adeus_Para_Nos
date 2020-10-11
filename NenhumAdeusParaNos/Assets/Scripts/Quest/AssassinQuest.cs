using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Assassin Quest")]
public class AssassinQuest : Quest
{
    [Header("Assassin Quest")]
    [SerializeField] string areaName = "";
    [SerializeField] string targetName = "";
    public string AreaName { get { return areaName; } set { areaName = value; } }
    public string TargetName { get { return targetName; } set { targetName = value; } }

    public GameObject toInstantiate;

    public override void CheckComplete<T>(T thing)
    {
        try
        {
            INPC npc = thing as INPC;
            if (npc.Name.Equals(targetName)) TryComplete();
        }
        catch { Debug.Log("Not a npc"); }
    }

    public override void InstantiateObjs(ObjectInstancer oi)
    {
        oi.SpawnSingleEnemy(areaName, targetName, toInstantiate);
    }
}
