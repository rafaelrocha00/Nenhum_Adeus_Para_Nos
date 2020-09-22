using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Dialogue Quest")]
public class DialogueQuest : Quest
{
    public string[] npcsToTalk;
    public bool[] talked;

    public override void CheckComplete<T>(T thing)
    {
        try
        {
            Dialogue d = thing as Dialogue;
            for (int i = 0; i < npcsToTalk.Length; i++)
            {
                if (d.MyNPC.Name.Equals(npcsToTalk[i])) talked[i] = true;
            }
            CheckTalked();
        }
        catch { Debug.Log("Not a dialogue"); }
    }

    public void CheckTalked()
    {
        for (int i = 0; i < talked.Length; i++)
        {
            if (!talked[i]) return;
        }
        TryComplete();
    }

    public override void InstantiateObjs(ObjectInstancer oi)
    {
        //
    }

    public override void Reset()
    {
        base.Reset();
        for (int i = 0; i < talked.Length; i++)
        {
            talked[i] = false;
        }
    }
}
