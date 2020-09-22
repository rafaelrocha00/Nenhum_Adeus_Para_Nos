using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Interact Quest")]
public class InteractQuest : Quest
{
    public string objectToInteract;

    public override void CheckComplete<T>(T thing)
    {
        try
        {
            Interactives interactive = thing as Interactives;
            if (interactive.Name.Equals(objectToInteract)) TryComplete();
        }
        catch { Debug.Log("Not a interactive"); }
    }

    public override void InstantiateObjs(ObjectInstancer oi)
    {
        //throw new System.NotImplementedException();
    }
}
