using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactives : MonoBehaviour
{
    [SerializeField] string myName = "";
    public string Name { get { return myName; } set { myName = value; } }
    public GameObject buttonToPressPref;
    protected GameObject buttonPref;
    protected GameObject quest_mark;
    protected Quest active_quest;

    public float popUPHigh;

    public bool oneInteraction = false;
    public bool canInteract = true;

    //protected List<Quest> quests_targeted_me = new List<Quest>();

    [Header("Quest Triggerable")]
    public bool onlyAcceptedQuest;
    public bool onlyCompletedQuest;
    public Quest triggerQuest;
    // public RepairableObject triggerRepairable;

    //protected bool firstEnable = true;

    private void Start()
    {
        if (triggerQuest != null)
        {
            if (onlyAcceptedQuest)
            {
                canInteract = triggerQuest.Accepted;
                return;
            }
            if (onlyCompletedQuest)
            {
                canInteract = triggerQuest.Completed;
                return;
            }

            //if (triggerQuest.Accepted && !triggerQuest.Completed) canInteract = true;
            canInteract = triggerQuest.Accepted && !triggerQuest.Completed;
        }

        //Acionar evento
        //firstEnable = false;
    }

    private void OnEnable()
    {
        /*if (!firstEnable)*/
        Invoke("AddQuestEvent", 0.005f);
    }
    void AddQuestEvent()
    {
        //Debug.Log("adicionando quest event /" + Name);
        CustomEvents.instance.onQuestAccepted += CheckForQuestObjectives;
    }

    private void OnDisable()
    {
        CustomEvents.instance.onQuestAccepted -= CheckForQuestObjectives;

        if (quest_mark != null) quest_mark.SetActive(false);
    }

    public virtual void CheckForQuestObjectives(Quest q_)
    {
        //Debug.Log("Checando quest no interagiveis");

        if (q_ is InteractQuest)
        {
            InteractQuest q = (InteractQuest)q_;
            if (q.objectToInteract.Equals(Name))
            {
                SpawnQuestMarker();
                active_quest = q;
                //if (quests_targeted_me.Contains(q_)) quests_targeted_me.Add(q_);
                return;
            }
        }
    }

    protected void SpawnQuestMarker()
    {
        if (quest_mark != null)
        {
            quest_mark.SetActive(true);
            return;
        }

        quest_mark = Instantiate(GameManager.gameManager.questController.quest_marker_pref, GameManager.gameManager.MainHud.popUpsHolder, false) as GameObject;
        quest_mark.GetComponent<ButtonToPress>().SetTransf(transform, popUPHigh);
    }
    public void CheckQuestMarker()
    {
        if (quest_mark != null && quest_mark.activeSelf) quest_mark.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (oneInteraction && !canInteract) return;
        if (!canInteract) return;

        if (other.tag.Equals("player") && !GameManager.gameManager.battleController.ActiveBattle)
        {
            Player player = other.GetComponent<Player>();
            if (buttonPref == null)
            {
                //buttonPref = Instantiate(buttonToPressPref, transform.position + Vector3.up * popUPHigh, Quaternion.identity);
                buttonPref = Instantiate(buttonToPressPref, GameManager.gameManager.MainHud.popUpsHolder, false) as GameObject;
                buttonPref.GetComponent<ButtonToPress>().SetTransf(transform, popUPHigh);
            }
            else
            {
                buttonPref.GetComponent<ButtonToPress>().SetTransf(transform, popUPHigh);
                buttonPref.SetActive(true);
            }
            player.InteractingObjs.Add(this);
            player.CanInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            Player player = other.GetComponent<Player>();
            player.Interacting = false;
            if (player.InteractingObjs.Count <= 1) player.CanInteract = false;
            Debug.Log(player.Interacting);
            OnExit(player);
        }
    }

    public void ActiveBtp()
    {
        if (buttonPref != null) buttonPref.SetActive(true);
    }

    public void DesactiveBtp()
    {
        if (buttonPref != null)
        {
            //buttonPref.transform.position = transform.position + Vector3.up * popUPHigh;
            buttonPref.SetActive(false);
        }
    }

    public abstract void Interact(Player player);

    public void CheckQuest()
    {
        GameManager.gameManager.questController.CheckQuests(this);
    }

    public void EndInteraction()
    {
        Player p = GameManager.gameManager.battleController.MainCharacter;

        p.EnableInteraction();
    }

    public virtual void OnExit(Player p)
    {
        //if (p.InteractingObjs == null) return;
        p.Interacting = false;
        for (int i = 0; i < p.InteractingObjs.Count; i++)
        {
            Debug.Log(p.InteractingObjs[i].name);
        }
        int aux = p.InteractingObjs.FindIndex(x => x.name.Equals(this.name));
        Debug.Log("Index: " + aux);
        if (aux >= 0) p.InteractingObjs.RemoveAt(aux);
        Debug.Log("Removendo " + this.name);
        DesactiveBtp();
    }

    public void EnableCollider(bool v)
    {
        GetComponent<Collider>().enabled = v;
    }
}
