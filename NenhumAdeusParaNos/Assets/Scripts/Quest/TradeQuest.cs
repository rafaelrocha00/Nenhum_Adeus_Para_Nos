using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Trade Quest")]
public class TradeQuest : Quest
{
    public Item itemToGet;

    public override void CheckComplete<T>(T thing)
    {
        try
        {
            Item i = thing as Item;
            if (i.itemName.Equals(itemToGet.itemName)) TryComplete();
        }
        catch { Debug.Log("Not a repairable"); }
    }

    public override void InstantiateObjs(ObjectInstancer oi) {  }
}
