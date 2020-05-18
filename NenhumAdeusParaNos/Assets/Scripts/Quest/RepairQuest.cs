using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Repair Quest")]
public class RepairQuest : Quest
{
    [SerializeField] string objectToRepair = "";
    public string ObjectToRepair { get { return objectToRepair; } set { objectToRepair = value; } }

    public GameObject toInstantiate;

    public override void CheckComplete<T>(T thing)
    {
        //throw new System.NotImplementedException();
    }

    public override void InstantiateObjs(ObjectInstancer oi)
    {
        //throw new System.NotImplementedException();
    }
}
