using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Interactives
{
    [Header("Bed")]
    Player p;
    public int timeTopass = 480;

    public AudioClip clip_interaction;

    public Item itemToGive;

    public override void Interact(Player player)
    {
        GameManager.gameManager.MainHud.FadeInOut();
        p = player;

        DesactiveBtp();
        p.enabled = false;
        Invoke("PassTime", 1);
        Invoke("ActivePlayer", 3);
        CheckQuest();
        canInteract = false;
        if (itemToGive != null) GameManager.gameManager.inventoryController.Inventory.AddItem(itemToGive);
    }

    void PassTime()
    {
        //GameManager.gameManager.calendarController.PassTime(0, 8, 0);
        GameManager.gameManager.audioController.PlayEffect(clip_interaction);
        GameManager.gameManager.calendarController.PassTime(timeTopass);
    }

    void ActivePlayer()
    {
        p.enabled = true;
        OnExit(p);
        //DesactiveBtp();
        EndInteraction();
    }
}
