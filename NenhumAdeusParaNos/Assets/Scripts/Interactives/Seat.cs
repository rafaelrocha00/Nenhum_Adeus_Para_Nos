using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : Interactives
{
    [Header("Seat")]
    public Transform edPos;

    private void Start()
    {
        if (GameManager.gameManager.NewGame) canInteract = false;
    }

    public override void Interact(Player player)
    {
        DesactiveBtp();

        player.Sit();
        player.CanMove = false;

        player.transform.position = edPos.transform.position;
        player.transform.rotation = edPos.transform.rotation;

        OnExit(player);
        EndInteraction();
    }

    private void OnDisable()
    {
        CustomEvents.instance.onQuestAccepted -= CheckForQuestObjectives;

        if (quest_mark != null) quest_mark.SetActive(false);

        if (buttonPref != null && buttonPref.activeSelf) OnExit(GameManager.gameManager.battleController.MainCharacter);
    }
}
