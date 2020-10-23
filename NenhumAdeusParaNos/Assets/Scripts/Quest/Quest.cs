using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest : ScriptableObject
{
    //contador no player ou controlador q indica quantas quest o jogador deixou de cumprir - diminui recompensa   

    [SerializeField] string qName = "quest_name";
    [SerializeField] int id = 0;
    [SerializeField] string description = "quest_description";
    [SerializeField] int moneyReward = 0;    
    [SerializeField] int resourceReward = 0;
    [SerializeField] string contractor = "contractor_";
    [SerializeField] protected bool mustReturn = false;
    [SerializeField] int limitDay = -1;
    public string Name { get { return qName; } set { qName = value; } }
    public int ID { get { return id; } set { id = value; } }
    public bool generated = true;
    public string Description { get { return description; } set { description = value; } }
    public int MoneyReward { get { return moneyReward; } set { moneyReward = value; } }
    public CompanyController.ResourceType resourceT = CompanyController.ResourceType.None;
    public int ResourceReward { get { return resourceReward; } set { resourceReward = value; } }
    public string Contractor { get { return contractor; } set { contractor = value; } }
    public bool MustReturn { get { return mustReturn; } set { mustReturn = value; } }
    public DialogueQuestTrigger completingQuestDialogue;
    public int LimitDay { get { return limitDay; } set { limitDay = value; } }

    public Item[] quest_itemRewards;
    public int[] quest_itemRQuants;

    [SerializeField] protected bool completed = false;
    [SerializeField] protected bool accepted = false;
    [SerializeField] protected bool cancelled = false;
    [SerializeField] protected bool waitingReturnToNPC = false;
    public bool Completed { get { return completed; } }
    public bool Accepted { get { return accepted; } }
    public bool Cancelled { get { return cancelled; } }
    public bool WaitingReturnToNPC { get { return waitingReturnToNPC; } }

    [TextArea] public string acceptText;
    [TextArea] public string completeText;

    [Header("Unlockable Dialogues")]
    public DialogueOptions[] unlockableDialogues;

    [Header("Chain Quest")]
    public Quest questToAccept;

    public abstract void CheckComplete<T>(T thing);

    [Header("Changing Scene")]
    public SceneStateConditions sceneStateChange;

    [Header("Companions")]
    public GameObject[] compToAdd;

    public void AcceptQuest()
    {
        cancelled = false;
        accepted = true;
        GameManager.gameManager.questController.AcceptQuest(this);
        if (!generated) GameManager.gameManager.questController.AddNote(acceptText);//mainNotes.AddNote(acceptText);

        CustomEvents.instance.OnQuestAccepted(qName);
    }

    public void Complete()
    {
        completed = true;
        GameManager.gameManager.questController.CompleteQuest(this);
        GameManager.gameManager.companyController.AddResource(resourceT, resourceReward);
        GameManager.gameManager.companyController.Money += moneyReward;

        if (!generated)
        {
            OnComplete();
        }

        CustomEvents.instance.OnQuestComplete(qName);

        Debug.Log("QUEST COMPLETA: " + qName);
    }

    void OnComplete()
    {
        GameManager.gameManager.questController.AddNote(completeText);//mainNotes.AddNote(completeText);
        for (int i = 0; i < quest_itemRewards.Length; i++)
        {
            for (int j = 0; j < quest_itemRQuants[i]; j++)
            {
                GameManager.gameManager.inventoryController.Inventory.AddItem(quest_itemRewards[i]);
            }
        }

        if (unlockableDialogues != null)
        {
            for (int i = 0; i < unlockableDialogues.Length; i++)
            {
                unlockableDialogues[i].UnlockDialogue();
            }
        }

        if (questToAccept != null) questToAccept.AcceptQuest();

        if (sceneStateChange != null) GameManager.gameManager.ChangeCurrentSceneState(sceneStateChange);

        if (compToAdd != null)
        {
            for (int i = 0; i < compToAdd.Length; i++)
            {
                GameManager.gameManager.PlayerCompanionsPref.Add(compToAdd[i]);
            }
        }
    }

    public void Cancel()
    {
        Reset();
        GameManager.gameManager.questController.CancelQuest(this);
        cancelled = true;
    }

    public void TryComplete()
    {
        if (!mustReturn) Complete();
        else waitingReturnToNPC = true;
    }

    public abstract void InstantiateObjs(ObjectInstancer oi);

    //private void OnDisable()
    //{
    //    accepted = false;
    //    completed = false;
    //    waitingReturnToNPC = false;
    //}

    private void OnEnable()
    {
        Reset();
    }

    public virtual void Reset()
    {
        accepted = false;
        completed = false;
        waitingReturnToNPC = false;
    }
}
