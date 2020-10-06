using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableDoor : Interactives
{
    public Animator anim;
    //public Collider gateCol;

    public Quest questAcceptedCondition;

    private void Start()
    {
        if (questAcceptedCondition == null) return;

        if (questAcceptedCondition.Accepted) canInteract = true;
        else canInteract = false;
    }

    public override void Interact(Player player)
    {
        DesactiveBtp();
        anim.SetBool("Open", true);
        //gateCol.enabled = false;

        OnExit(player);
        canInteract = false;
        this.enabled = false;
    }
}
