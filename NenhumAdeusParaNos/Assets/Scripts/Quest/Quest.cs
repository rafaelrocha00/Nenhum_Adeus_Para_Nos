using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest : ScriptableObject
{
    //contador no player ou controlador q indica quantas quest o jogador deixou de cumprir - diminui recompensa   

    [SerializeField] string qName = "quest_name";
    [SerializeField] string description = "quest_description";
    [SerializeField] int moneyReward = 0;    
    [SerializeField] int resourceReward = 0;
    [SerializeField] string contractor = "contractor_";
    [SerializeField] protected bool mustReturn = false;
    [SerializeField] int limitDay = -1;
    public string Name { get { return qName; } }
    public string Description { get { return description; } }
    public int MoneyReward { get { return moneyReward; } }
    public CompanyController.ResourceType resourceT = CompanyController.ResourceType.None;
    public int ResourceReward { get { return resourceReward; } }
    public string Contractor { get { return contractor; } }
    public bool MustReturn { get { return mustReturn; } }
    public DialogueQuestTrigger completingQuestDialogue;
    public int LimitDay { get { return limitDay; } }

    public Item[] quest_itemRewards;

    [SerializeField] protected bool completed = false;
    [SerializeField] protected bool accepted = false;
    [SerializeField] protected bool waitingReturnToNPC = false;
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

    private void OnDisable()
    {
        accepted = false;
        completed = false;
        waitingReturnToNPC = false;
    }

    private void OnEnable()
    {
        accepted = false;
        completed = false;
        waitingReturnToNPC = false;
    }
}
