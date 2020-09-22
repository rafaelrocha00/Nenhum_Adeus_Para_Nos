using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Interactives
{
    Player p;
    public int timeTopass = 480;

    public override void Interact(Player player)
    {
        GameManager.gameManager.MainHud.FadeInOut();
        p = player;

        p.enabled = false;
        Invoke("PassTime", 1);
        Invoke("ActivePlayer", 3);
        CheckQuest();
        canInteract = false;
    }

    void PassTime()
    {
        //GameManager.gameManager.calendarController.PassTime(0, 8, 0);
        GameManager.gameManager.calendarController.PassTime(timeTopass);
    }

    void ActivePlayer()
    {
        p.enabled = true;
        DesactiveBtp();
    }
}
