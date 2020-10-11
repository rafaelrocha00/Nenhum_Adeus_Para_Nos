using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scene State Contition", fileName = "Scene State Condition")]
public class SceneStateConditions : ScriptableObject
{
    [SerializeField] int changingState = -1;

    [SerializeField] Quest questToNotBeAccepted = null;
    [SerializeField] Quest questToBeCompleted = null;

    [SerializeField] bool movePlayer = false;
    [SerializeField] bool fadeInOut = false;

    public int ChangingState { get { return changingState; } }

    public Quest QuestToNotBeAccepted { get { return questToNotBeAccepted; } }
    public Quest QuestToBeCompleted { get { return questToBeCompleted; } }

    public bool MovePlayer { get { return movePlayer; } }
    public bool FadeInOut { get { return fadeInOut; } }

    public bool CanChangeState()
    {
        try { return questToBeCompleted.Completed && !questToNotBeAccepted.Accepted; }
        catch {  }

        if (questToBeCompleted != null && questToBeCompleted.Completed) return true;

        if (questToNotBeAccepted != null && questToNotBeAccepted.Accepted) return true;

        return false;
    }
}
