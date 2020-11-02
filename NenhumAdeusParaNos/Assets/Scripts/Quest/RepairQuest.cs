using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Repair Quest")]
public class RepairQuest : Quest
{
    [Header("Repair Quest")]
    [SerializeField] string objectToRepair = "";
    public string ObjectToRepair { get { return objectToRepair; } set { objectToRepair = value; } }

    public GameObject toInstantiate;

    public override void CheckComplete<T>(T thing)
    {
        try
        {
            RepairableObject rep = thing as RepairableObject;
            if (rep.Name.Equals(objectToRepair) && !rep.broken) { TryComplete(); rep.CheckQuestMarker(); }
        }
        catch { Debug.Log("Not a repairable"); }
    }

    public override void InstantiateObjs(ObjectInstancer oi)
    {
        //throw new System.NotImplementedException();
    }
}
