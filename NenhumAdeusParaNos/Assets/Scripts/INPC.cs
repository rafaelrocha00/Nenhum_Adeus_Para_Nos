using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INPC : Interactives, BattleUnit
{
    [HideInInspector] CharacterStats charStats;
    public CharacterStats CharStats { get { return charStats; } }

    public Dialogue myDialogue;
    Dialogue initialDialogue;

    public bool hostile = false;

    bool interacting = false;

    bool inBattle = false;

    private void Start()
    {
        charStats = new CharacterStats(this);

        myDialogue.MyNPC = this;
        initialDialogue = myDialogue;
    }

    public override void Interact(Player player)
    {
        myDialogue.MainCharacter = player;
        interacting = true;
        DesactiveBtp();
        GameManager.gameManager.dialogueUIMain.OpenDialoguePopUp(transform.position, this);
        NextString();
    }

    public void NextString()
    {
        GameManager.gameManager.dialogueUIMain.UpdateText(myDialogue.NextString());
    }

    public void EndDialogue()
    {        
        GameManager.gameManager.dialogueUIMain.CloseDialoguePopUp();
        myDialogue.ResetDialogue();
        myDialogue = initialDialogue;
        interacting = false;
    }

    public override void OnExit()
    {
        EndDialogue();
    }

    public void ChangeDialogue(Dialogue newDialogue)
    {
        myDialogue.ResetDialogue();
        myDialogue = newDialogue;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && interacting) NextString();
    }

    private void FixedUpdate()
    {
        if (inBattle)
        {

        }
    }

    public void StartBattle()
    {
        if (hostile && !inBattle)
        {
            inBattle = true;
            GetComponent<SphereCollider>().enabled = false;
            GameManager.gameManager.battleController.AddFighter(this);
            Debug.Log("NPC entrou na batalha");
        }
    }

    public void EndBattle()
    {
        inBattle = false;
        //GetComponent<SphereCollider>().enabled = true;
    }

    public bool CanFight()
    {
        return charStats.CanFight;
    }
}
