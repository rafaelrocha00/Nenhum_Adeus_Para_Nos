using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Kill Quest")]
public class KillQuest : Quest
{
    public INPC.EnemyType enemyType;
    public INPC.Faction faction;

    [SerializeField] string areaName = "";
    [SerializeField] int quantToKill = 1;
    public string AreaName { get { return areaName; } }
    public int QuantToKill { get { return quantToKill; } }

    int quantKilled = 0;

    public override void CheckComplete<T>(T thing)
    {
        try
        {
            INPC npc = thing as INPC;
            if (npc.AreaName.Equals(areaName) && (npc.enemyType == enemyType || enemyType == INPC.EnemyType.None) && (npc.faction == faction || faction == INPC.Faction.None))
            {
                quantKilled++;
                if (quantKilled == quantToKill) TryComplete();
            }

        }
        catch { Debug.Log("Not a npc"); }
    }

    private void OnDisable()
    {
        quantKilled = 0;
    }
}
