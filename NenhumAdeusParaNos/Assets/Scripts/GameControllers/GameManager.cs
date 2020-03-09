using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    [HideInInspector] public DialogueUI dialogueUIMain;
    [HideInInspector] public BattleController battleController;
    [HideInInspector] public MainHud mainHud;

    private void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            Destroy(gameObject);
        }
        else
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject);
        }

        mainHud = GameObject.Find("/MainHud").GetComponent<MainHud>();
        dialogueUIMain = GetComponent<DialogueUI>();
        battleController = GetComponent<BattleController>();
    }
}
