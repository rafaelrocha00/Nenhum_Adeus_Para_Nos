using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNPC : Interactives, IDialogueable
{
    [Header("Simple NPC")]
    public Sprite portrait;
    public DialogueOptions myDialogues;
    //public bool sign = false;

    NavMeshAgent navmesh;

    Animator anim;

    private void Start()
    {
        navmesh = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        CustomEvents.instance.onDialogueStart += EnterDialogueAnim;
        CustomEvents.instance.onDialogueEnd += ExitDialogueAnim;
    }

    private void OnDestroy()
    {
        CustomEvents.instance.onDialogueStart -= EnterDialogueAnim;
        CustomEvents.instance.onDialogueEnd -= ExitDialogueAnim;
    }

    public override void Interact(Player player)
    {
        DesactiveBtp();
        StartDialogue(player);
    }

    void StartDialogue(Player p)
    {
        Dialogue aux = myDialogues.GetRandomDialogue();
        aux.MyNPC = this;
        aux.MainCharacter = p;

        GameManager.gameManager.dialogueController.StartDialogue(aux, transform, portrait);
    }

    public void EndDialogue()
    {
        GameManager.gameManager.dialogueController.EndDialogue();
    }

    public string GetName()
    {
        return Name;
    }

    public Sprite GetPortrait()
    {
        return portrait;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void MoveNavMesh(Vector3 point)
    {
        navmesh.isStopped = false;
        navmesh.destination = point;
    }

    public void ReceiveItem()
    {
        if (anim != null) anim.SetTrigger("ReceivedItem");
    }

    public void EnterDialogueAnim(string npc_name)
    {
        //if (!sign) 

        if (!Name.Equals(npc_name) || anim == null) return;

        anim.SetBool("Talking", true);
    }

    public void ExitDialogueAnim(string npc_name)
    {
        if (!Name.Equals(npc_name) || anim == null) return;

        anim.SetBool("Talking", false);
    }
    
    //IEnumerator RotateToPlayer()
    //{
    //    //Quaternion newRot = 
    //    while (true)
    //    {            
    //    }
    //}
}
