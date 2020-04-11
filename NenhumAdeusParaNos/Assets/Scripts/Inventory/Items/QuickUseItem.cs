using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuickUseItem : Item
{
    [HideInInspector] Player mainCharacter;
    public Player MainCharacter { get { if (mainCharacter == null) mainCharacter = GameManager.gameManager.battleController.MainCharacter; return mainCharacter; } set { mainCharacter = value; } }

    [SerializeField] float animTime;
    public float AnimTime { get { return animTime; } set { animTime = value; } }

    public bool instantUse = false;

    //void SetPlayer()
    //{
    //    if (mainCharacter == null) mainCharacter = GameManager.gameManager.battleController.MainCharacter;
    //}

    public abstract bool Effect();
}
