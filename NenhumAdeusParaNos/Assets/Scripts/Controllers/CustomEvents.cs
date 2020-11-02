using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEvents : MonoBehaviour
{
    public static CustomEvents instance;

    private void Awake()
    {
        instance = this;
    }

    public event Action<Quest> onQuestComplete;
    public void OnQuestComplete(Quest quest)
    {
        if (onQuestComplete != null)
        {
            onQuestComplete(quest);
        }
    }

    public event Action<Quest> onQuestAccepted;
    public void OnQuestAccepted(Quest quest)
    {
        if (onQuestAccepted != null)
        {
            onQuestAccepted(quest);
        }
    }

    public event Action<string> onDialogueStart;
    public void OnDialogueStart(string npc_name)
    {
        if (onDialogueStart != null)
        {
            onDialogueStart(npc_name);
        }
    }

    public event Action<string> onDialogueEnd;
    public void OnDialogueEnd(string npc_name)
    {
        if (onDialogueEnd != null)
        {
            onDialogueEnd(npc_name);
        }
    }

    public event Action onExitTrajectory;
    public void OnExitTrajectory()
    {
        if (onExitTrajectory != null)
        {
            onExitTrajectory();
        }
    }
}
