using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactives : MonoBehaviour
{
    [SerializeField] string myName = "";
    public string Name { get { return myName; } set { myName = value; } }
    public GameObject buttonToPressPref;
    protected GameObject buttonPref;

    public float popUPHigh;

    public bool oneInteraction = false;
    public bool canInteract = true;

    [Header("Quest Triggerable")]
    public bool onlyAcceptedQuest;
    public bool onlyCompletedQuest;
    public Quest triggerQuest;
    // public RepairableObject triggerRepairable;

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
                buttonPref = Instantiate(buttonToPressPref, GameManager.gameManager.MainHud.pressEPops, false) as GameObject;
                buttonPref.GetComponent<ButtonToPress>().SetTransf(transform, popUPHigh);
            }
            else buttonPref.SetActive(true);
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
