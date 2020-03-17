using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactives : MonoBehaviour
{
    public GameObject buttonToPressPref;
    GameObject buttonPref;

    public float popUPHigh;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null && !other.GetComponent<Player>().IsInBattle())
        {
            Player player = other.GetComponent<Player>();
            if (buttonPref == null) buttonPref = Instantiate(buttonToPressPref, transform.position + Vector3.up * popUPHigh, Quaternion.identity);
            else buttonPref.SetActive(true);
            player.InteractingObj = this;
            player.CanInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            DesactiveBtp();
            OnExit();
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

    public virtual void OnExit()
    {
        Debug.Log("Do on exit");
    }
}
