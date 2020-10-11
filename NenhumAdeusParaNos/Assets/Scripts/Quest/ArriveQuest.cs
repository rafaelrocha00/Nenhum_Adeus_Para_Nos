using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Arrive Quest")]
public class ArriveQuest : Quest
{
    [Header("Arrive Quest")]
    public string placeToGoName;

    public override void CheckComplete<T>(T thing)
    {
        try
        {
            PlaceToGo ptg = thing as PlaceToGo;
            if (ptg.PlaceName.Equals(placeToGoName)) TryComplete();
        }
        catch { Debug.Log("Not a palce"); }
    }

    public override void InstantiateObjs(ObjectInstancer oi)
    {
        Debug.Log("Nada");
    }
}
