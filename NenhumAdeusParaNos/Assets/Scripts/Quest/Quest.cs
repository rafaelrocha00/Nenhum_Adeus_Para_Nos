using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest : ScriptableObject
{
    [SerializeField] string qName = "quest_name";
    [SerializeField] string description = "quest_description";
    [SerializeField] int moneyReward = 0;
    [SerializeField] protected bool mustReturn = false;
    public string Name { get { return qName; } }
    public string Description { get { return description; } }
    public int MoneyReward { get { return moneyReward; } }
    public bool MustReturn { get { return mustReturn; } }

    public Item[] quest_itemRewards;

    [HideInInspector] protected bool completed = false;
    [HideInInspector] protected bool accepted = false;
    [HideInInspector] protected bool waitingReturnToNPC = false;
    public bool Completed { get { return completed; } }
    public bool Accepted { get { return accepted; } }
    public bool WaitingReturnToNPC { get { return waitingReturnToNPC; } }

    public abstract void CheckComplete<T>(T thing);

    public void AcceptQuest()
    {
        accepted = true;
        GameManager.gameManager.questController.AcceptQuest(this);
    }

    public void Complete()
    {
        completed = true;
        GameManager.gameManager.questController.CompleteQuest(this);
        //Dar recompensas?
    }

    public void TryComplete()
    {
        if (!mustReturn) Complete();
        else waitingReturnToNPC = true;
    }
}
