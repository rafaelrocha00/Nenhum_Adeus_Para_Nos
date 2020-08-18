using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactives : MonoBehaviour
{
    public GameObject buttonToPressPref;
    protected GameObject buttonPref;

    public float popUPHigh;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("player") && !GameManager.gameManager.battleController.ActiveBattle)
        {
            Player player = other.GetComponent<Player>();
            if (buttonPref == null) buttonPref = Instantiate(buttonToPressPref, transform.position + Vector3.up * popUPHigh, Quaternion.identity);
            else buttonPref.SetActive(true);
            player.InteractingObjs.Add(this);
            player.CanInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("ué");
        if (other.GetComponent<Player>() != null)
        {
            Player player = other.GetComponent<Player>();
            player.Interacting = false;
            player.CanInteract = false;
            Debug.Log(player.Interacting);
            OnExit(player);
        }
    }

    protected void DesactiveBtp()
    {
        if (buttonPref != null)
        {
            buttonPref.transform.position = transform.position + Vector3.up * popUPHigh;
            buttonPref.SetActive(false);
        }
    }

    public abstract void Interact(Player player);

    public virtual void OnExit(Player p)
    {
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
}
