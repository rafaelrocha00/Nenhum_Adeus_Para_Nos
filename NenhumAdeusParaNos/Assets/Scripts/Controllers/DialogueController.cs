﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    Text dialogueText;
    GameObject dialoguePopUp;
    Button popUPB;

    Camera mainCam;

    public GameObject dialoguePref;

    Dialogue actualDialogue;

    DialogueWithChoice lastDialogueWithChoice;

    [HideInInspector] bool activeDialogue = false;
    public bool ActiveDialogue { get { return activeDialogue; } }
    bool writing = false;

    [HideInInspector] bool waitingForAnswer = false;
    public bool WaitingForAnswer { get { return waitingForAnswer; } }

    public float writingSpeed = 0.05f;
    float writingTime = 0.0f;

    //public List<DialogueBattle>[][] approaches = new List<DialogueBattle>[4][];//Array de Approaches o primeirod Index define o tipo de inimigo, o segundo o tipo de abordagem, e dentro da lista deles estão os diálogos para serem sorteados.
    public DialogueBattle[] playerApproaches = new DialogueBattle[4];

    public List<Dialogue>[][][] npcAnswers = new List<Dialogue>[4][][];//Array das respostas dos inimigos, Uma lista de opções de respotas pra cada tipo de abordagem para cada personalidade para cada tipo de inimigo
    //Sprite dps
    public Color[] dialogueColors = new Color[5];
    
    private void Start()
    {
        //for (int i = 0; i < approaches.Length; i++)
        //{
        //    approaches[i] = new List<DialogueBattle>[5];
        //    for (int j = 0; j < approaches[i].Length; j++)
        //    {
        //        approaches[i][j] = new List<DialogueBattle>();
        //        for (int k = 0; k < 2; k++)
        //        {
        //            DialogueBattle dialogueAux = Resources.Load<DialogueBattle>("Approaches/EnemyT" + (i + 1) + "/ApproachT" + (j + 1) + "/Dialogue" + (k + 1));
        //            if (dialogueAux != null)
        //            {
        //                approaches[i][j].Add(dialogueAux);
        //            }
        //        }
        //    }
        //}

        for (int i = 0; i < npcAnswers.Length; i++)
        {
            npcAnswers[i] = new List<Dialogue>[4][];
            for (int j = 0; j < npcAnswers[i].Length; j++)
            {
                npcAnswers[i][j] = new List<Dialogue>[4];
                for (int k = 0; k < npcAnswers[i][j].Length; k++)
                {
                    npcAnswers[i][j][k] = new List<Dialogue>();
                    for (int l = 0; l < 3; l++)
                    {
                        Dialogue dialogueAux = Resources.Load<Dialogue>("NPCAnswers/EnemyT" + (i + 1) + "/Personality" + (j + 1) + "/ApproachT" + (k + 1) + "/Dialogue" + (l + 1));
                        if (dialogueAux != null)
                        {
                            npcAnswers[i][j][k].Add(dialogueAux);
                        }
                    }
                }
            }
        }
        mainCam = GameManager.gameManager.MainCamera.GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && activeDialogue && !(actualDialogue is DialogueBattle))
        {
            Debug.Log("NextString");
            NextString();
        }
    }

    public DialogueBattle GetDialogueBattle(int enType, int apType, int idx)
    {
        //return approaches[enType][apType][idx];
        return playerApproaches[apType];
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="enType"></param>
    /// <param name="personality"></param>
    /// <param name="apType"></param>
    /// <param name="idx">0 e 1 para Respostas que deram errado, 2 para a Resposta que deu certo</param>
    /// <returns></returns>
    public Dialogue GetAnswer(int enType, int personality, int apType, int idx)
    {
        return npcAnswers[enType][personality][apType][idx];
    }

    public void StartDialogue(Dialogue newDialogue, Transform transf/*, INPC npc = null*/)
    {
        if (newDialogue is DialogueWithChoice) lastDialogueWithChoice = (DialogueWithChoice)newDialogue;
        activeDialogue = true;
        OpenDialoguePopUp(transf/*, npc*/);
        actualDialogue = newDialogue;
        NextString();
        StartCoroutine("CheckPlayerDistance");
    }
    IEnumerator CheckPlayerDistance()
    {
        while (activeDialogue)
        {
            if (actualDialogue.GetPlayerNPCDistance() > 100)
            {
                EndDialogue();
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void ChangeDialogue(Dialogue newDialogue)
    {
        actualDialogue.ResetDialogue();
        actualDialogue = newDialogue;
        NextString();
        //Debug.Log("Mudando dialogo");
    }
    public void ChooseOption(int index)
    {
        try
        {
            Dialogue newDialogue = lastDialogueWithChoice.dialogueChoices[index];
            newDialogue.MyNPC = lastDialogueWithChoice.MyNPC;
            newDialogue.MainCharacter = lastDialogueWithChoice.MainCharacter;
            StopCoroutine("NextStringCountdown");
            StartDialogue(newDialogue, lastDialogueWithChoice.MyNPC.transform);
            waitingForAnswer = false;
            //GameManager.gameManager.MainHud.WaitingForAnswer(false);
        }
        catch { Debug.Log("Não é de opção"); }
    }

    public void NextString()
    {
        if (!GameManager.gameManager.MainHud.ActiveOptionsToChoose)
        {
            //Bug no dialogo combate
            //Debug.Log("NextString");
            //Debug.Log(writing);
            if (!writing) UpdateText(actualDialogue.NextString());
            else
            {
                CheckForDialogueOptions();
                writing = false;
            }

            //Debug.Log("Deu nextstring");
            if (activeDialogue && (GameManager.gameManager.battleController.ActiveBattle || (actualDialogue is DialogueBattle))) StartCoroutine("NextStringCountdown");
        }
    }
    public void EndDialogue()
    {
        if (activeDialogue)
        {
            activeDialogue = false;
            actualDialogue.ResetDialogue();
            CloseDialoguePopUp();
            //Debug.Log("Encerrando dialogo");
            StopCoroutine("NextStringCountdown");
        }
    }

    //Quando implementar texto escrito aos poucos, este método tem q retornar o tempo q levou pra ser escrito o texto
    public void UpdateText(string dialogueString)
    {       
        if (!dialogueString.Equals(""))
        {
            writingTime = dialogueString.Length * writingSpeed + 1.5f;
            StartCoroutine(WriteString(dialogueString));
        }
        //dialogueText.text = dialogueString;
    }

    void CheckForDialogueOptions()
    {
        if (actualDialogue is DialogueWithChoice)
        {
            DialogueWithChoice aux = (DialogueWithChoice)actualDialogue;
            if (aux.LastString)
            {
                //GameManager.gameManager.MainHud.OpenDialogueOptTab(aux);
                aux.MyNPC.SetWaitingForAnswer();
                waitingForAnswer = true;
                GameManager.gameManager.MainHud.WaitingForAnswer(true);
            }
        }        
    }

    public void OpenDialoguePopUp(Transform transf/*, INPC npc = null*/)
    {
        try
        {
            dialoguePopUp.transform.position = mainCam.WorldToScreenPoint(transf.position);
            dialoguePopUp.SetActive(true);
        }
        catch
        {
            //dialoguePopUp = Instantiate(dialoguePref, GameManager.gameManager.mainHud.transform, false) as GameObject;
            //Pegar no filho do filho, e filho o botao
            //dialogueText = dialoguePopUp.GetComponentInChildren<Text>();
            //popUPB = dialoguePopUp.GetComponent<Button>();
            //dialoguePopUp = Instantiate(dialoguePref, transf.position, dialoguePref.transform.rotation) as GameObject;
            dialoguePopUp = Instantiate(dialoguePref, GameManager.gameManager.MainHud.transform, false) as GameObject;
            dialoguePopUp.transform.position = mainCam.WorldToScreenPoint(transf.position);
            //dialoguePopUp.GetComponent<DialoguePopUpFollow>().SetTransform(transf);
            dialogueText = dialoguePopUp.transform.GetChild(0).GetComponentInChildren<Text>();
            popUPB = dialoguePopUp.transform.GetChild(0).GetComponent<Button>();
            //Debug.Log("Abrindo Balao de dialogo");
        }
        //Mudar posição;
        //if (npc != null) popUPB.onClick.AddListener(npc.NextString);
        popUPB.onClick.AddListener(NextString);
    }

    public void CloseDialoguePopUp()
    {
        if (dialoguePopUp != null) dialoguePopUp.SetActive(false);
        if (popUPB != null) popUPB.onClick.RemoveAllListeners();
    }

    public void ChangePopUpPos(/*Vector3 newPos, */Transform transf)
    {
        //dialoguePopUp.transform.position = newPos;
        dialoguePopUp.GetComponent<DialoguePopUpFollow>().SetTransform(transf);
        //Debug.Log(transf.name);
    }

    //public void TimerForNextString()
    //{
    //    StartCoroutine("NextStringCountdown");
    //}

    IEnumerator NextStringCountdown()
    {
        Debug.Log("Countdown");
        yield return new WaitForSeconds(writingTime);
        writingTime = 0;
        if (activeDialogue) NextString();
    }

    IEnumerator WriteString(string dialogueString)
    {
        writing = true;
        string allText = "";

        for (int i = 0; i < dialogueString.Length; i++)
        {
            allText += dialogueString[i].ToString();
            dialogueText.text = allText;
            if (!writing)
            {
                dialogueText.text = dialogueString;
                yield break;
            }
            yield return new WaitForSeconds(writingSpeed);
        }
        CheckForDialogueOptions();
        writing = false;
    }

    public INPC[] GetNearbyNPCs(Vector3 center, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(center, radius, LayerMask.GetMask("Interactives"));
        List<INPC> enemies = new List<INPC>();
        for (int i = 0; i < colliders.Length; i++)
        {
            try
            {
                if (!colliders[i].isTrigger)
                {
                    enemies.Add(colliders[i].GetComponent<INPC>());
                }
            }
            catch { Debug.Log("Não é NPC"); }
        }
        return enemies.ToArray();
    }
}
