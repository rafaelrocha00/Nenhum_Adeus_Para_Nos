﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    [HideInInspector] public DialogueController dialogueController;
    [HideInInspector] public BattleController battleController;
    [HideInInspector] public TimeController timeController;

    [HideInInspector] MainHud mainHud;
    public MainHud MainHud { get { return mainHud; } set { mainHud = value; } }

    //[HideInInspector] Player mainCharacter;
    //public Player MainCharacter { get { return mainCharacter; } set { mainCharacter = value; } }

    public float[] Dandere = new float[5];
    public float[] Kuudere = new float[5];
    public float[] Tsundere = new float[5];
    public float[] Yandere = new float[5];

    public float[][] personalities = new float[4][];

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

        //mainHud = GameObject.Find("/MainHud").GetComponent<MainHud>();
        dialogueController = GetComponent<DialogueController>();
        battleController = GetComponent<BattleController>();
        timeController = GetComponent<TimeController>();

        personalities[0] = Dandere;
        personalities[1] = Kuudere;
        personalities[2] = Tsundere;
        personalities[3] = Yandere;
    }
}