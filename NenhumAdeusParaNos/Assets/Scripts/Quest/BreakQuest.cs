using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Break Quest", fileName = "New Quest")]
public class BreakQuest : Quest
{
    [Header("Break Quest")]
    public string objectToBreak;

    public override void CheckComplete<T>(T thing)
    {
        try
        {
            RepairableObject rep = thing as RepairableObject;
            if (rep.Name.Equals(objectToBreak) && rep.broken) { TryComplete(); rep.CheckQuestMarker(); }
        }
        catch { Debug.Log("Not a repairable"); }
    }

    public override void InstantiateObjs(ObjectInstancer oi)
    {
       //throw new System.NotImplementedException();
    }
}

