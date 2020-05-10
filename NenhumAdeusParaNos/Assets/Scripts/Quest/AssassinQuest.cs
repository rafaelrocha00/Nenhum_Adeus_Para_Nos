using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Assassin Quest")]
public class AssassinQuest : Quest
{
    [SerializeField] string targetName = "";
    public string TargetName { get { return targetName; } }

    public override void CheckComplete<T>(T thing)
    {
        try
        {
            INPC npc = thing as INPC;
            if (npc.Name.Equals(targetName)) TryComplete();
        }
        catch { Debug.Log("Not a npc"); }
    }
}
