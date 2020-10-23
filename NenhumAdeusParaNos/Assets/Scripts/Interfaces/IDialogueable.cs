using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogueable
{
    void EndDialogue();

    void MoveNavMesh(Vector3 point);

    void OnExit(Player p);

    void ReceiveItem();

    string GetName();

    Sprite GetPortrait();

    Transform GetTransform();
}
