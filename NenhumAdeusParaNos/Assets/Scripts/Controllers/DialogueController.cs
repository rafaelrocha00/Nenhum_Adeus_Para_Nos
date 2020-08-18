using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    //Text dialogueText;
    //GameObject dialoguePopUp;
    DialoguePopUp dialoguePopUp;
    //DialoguePopUp dialoguePopUpSecond;
    //Button popUPB;

    Camera mainCam;

    public GameObject dialoguePref;
    public float answerTime = 4f;
    bool waitingForAnswerBattle;
    INPC dialoguingNPC;

    [HideInInspector] Dialogue actualDialogue;
    public Dialogue ActualDialogue { get { return actualDialogue; } }
    Dialogue secondaryDialogue;

    DialogueWithChoice lastDialogueWithChoice;

    [HideInInspector] bool activeMainDialogue = false;
    public bool ActiveMainDialogue { get { return activeMainDialogue; } }
    //bool writing = false;

    [HideInInspector] bool waitingForAnswer = false;
    public bool WaitingForAnswer { get { return waitingForAnswer; } }

    //public float writingSpeed = 0.05f;
    float writingTime = 0.0f;

    //public List<DialogueBattle>[][] approaches = new List<DialogueBattle>[4][];//Array de Approaches o primeirod Index define o tipo de inimigo, o segundo o tipo de abordagem, e dentro da lista deles estão os diálogos para serem sorteados.
    public DialogueBattle[] playerApproaches = new DialogueBattle[3];

    public List<DialogueOptions>[][][] npcAnswers = new List<DialogueOptions>[4][][];//Array das respostas dos inimigos, Uma lista de opções de respotas pra cada tipo de abordagem para cada personalidade para cada tipo de inimigo
    Dialogue[] failResults = new Dialogue[4];
    public DialogueBattleResult[][][] battleResults = new DialogueBattleResult[4][][]; //Array de resultados de batalha, primeiro Index define o tipo de inimigo, o segundo a personalidade e o terceiro o resultado;
    //Sprite dps
    //public Color[] dialogueColors = new Color[5];
    //public GameObject[] dialogueExpressions = new GameObject[3];
    
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
            npcAnswers[i] = new List<DialogueOptions>[2][];
            for (int j = 0; j < npcAnswers[i].Length; j++)
            {
                npcAnswers[i][j] = new List<DialogueOptions>[3];
                for (int k = 0; k < npcAnswers[i][j].Length; k++)
                {
                    npcAnswers[i][j][k] = new List<DialogueOptions>();
                    for (int l = 0; l < 3; l++)
                    {
                        DialogueOptions dialogueAux = Resources.Load<DialogueOptions>("NPCAnswers/EnemyT" + (i + 1) + "/Personality" + (j + 1) + "/ApproachT" + (k + 1) + "/Dialogues" + (l + 1));
                        if (dialogueAux != null)
                        {
                            npcAnswers[i][j][k].Add(dialogueAux);
                        }
                    }
                }
            }
        }
        for (int i = 0; i < battleResults.Length; i++)
        {
            battleResults[i] = new DialogueBattleResult[2][];
            for (int j = 0; j < battleResults[i].Length; j++)
            {
                battleResults[i][j] = Resources.LoadAll<DialogueBattleResult>("DialogueBattleResults/EnemyT" + (i + 1) + "/Personality" + (j + 1));
            }
        }
        //failResults[0] = Resources.Load<Dialogue>("DialogueBattleResults/FailCombination1");
        //failResults[1] = Resources.Load<Dialogue>("DialogueBattleResults/FailCombination2");
        //failResults[2] = Resources.Load<Dialogue>("DialogueBattleResults/FailCombination3");
        //failResults[3] = Resources.Load<Dialogue>("DialogueBattleResults/FailCombination4");
        for (int i = 0; i < failResults.Length; i++)
        {
            failResults[i] = Resources.Load<Dialogue>("DialogueBattleResults/FailCombination" + (i + 1));
        }

        SetCam();
    }
    public void SetCam()
    {
        try { mainCam = GameManager.gameManager.MainCamera.GetComponent<Camera>(); }
        catch { }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.E) && activeMainDialogue && !(actualDialogue is DialogueBattle) && !waitingForAnswerBattle && !waitingForAnswer)
    //    {
    //        Debug.Log("NextString");
    //        NextString();
    //    }
    //}

    public DialogueBattle GetDialogueBattle(/*int enType,*/ int apType/*, int idx*/)
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
        return npcAnswers[enType][personality][apType][idx].GetRandomDialogue();
    }
    public Dialogue GetBattleResult(int enType, int personality, DialogueBattle.ApproachType[] comb, INPC npc, Player p)
    {
        Dialogue aux = failResults[personality];
        //enType = 0;

        try
        {
            for (int i = 0; i < battleResults[enType][personality].Length; i++)
            {

                if (battleResults[enType][personality][i].apCombination.SequenceEqual(comb))
                {
                    battleResults[enType][personality][i].StartEffects(npc, p);
                    if (battleResults[enType][personality][i].GetResult())
                    {
                        aux = battleResults[enType][personality][i];
                        //print("Achou um resultado aí");
                        //print(battleResults[enType][personality][i].allStrings[0]);
                    }
                }
                //Debug.Log(comb[0] + " | " + comb[1] + " | " + comb[2]);
                //Debug.Log(battleResults[enType][personality][i].apCombination[0] + " | " + battleResults[enType][personality][i].apCombination[1] + " | " + battleResults[enType][personality][i].apCombination[2]);
            }
        }
        catch (System.Exception)
        {
            throw;
        }

        return aux;
    }

    public void StartDialogue(Dialogue newDialogue, Transform transf, Sprite sp/*, INPC npc = null*/)
    {
        if (newDialogue is DialogueWithChoice) lastDialogueWithChoice = (DialogueWithChoice)newDialogue;
        activeMainDialogue = true;
        int id = 0;
        bool isPlayer = (newDialogue is DialogueBattle);        
        if (isPlayer)
        {
            DialogueBattle db = (DialogueBattle)newDialogue;
            id = (int)db.approachType;
        }

        OpenDialoguePopUp(transf/*, npc*/, sp, isPlayer, id);
        actualDialogue = newDialogue;
        NextString();
        StartCoroutine("CheckPlayerDistance");
    }
    IEnumerator CheckPlayerDistance()
    {
        while (activeMainDialogue)
        {
            if (actualDialogue.GetPlayerNPCDistance() > 100)
            {
                //StopCoroutine("NextStringCountdown");
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
    public void ChooseOption(Dialogue d, Sprite sp, int index = -1)
    {
        try
        {
            Debug.Log("ChoosingDialogue");
            Dialogue newD = d;
            if (index > -1)
            {                
                DialogueWithChoice dwc = (DialogueWithChoice)actualDialogue;
                if (!dwc.HasThisIndex(index)) return;
                newD = dwc.dialogueChoices[index];
                sp = dwc.MyNPC.expressions[1];
            }
            CloseDialogueOptTab();
            //Dialogue newDialogue = lastDialogueWithChoice.dialogueChoices[index];
            //newDialogue.MyNPC = lastDialogueWithChoice.MyNPC;
            //newDialogue.MainCharacter = lastDialogueWithChoice.MainCharacter;
            //StopCoroutine("NextStringCountdown");
            waitingForAnswer = false;
            StartDialogue(/*newDialogue*/newD, lastDialogueWithChoice.MyNPC.transform, sp);            
            //GameManager.gameManager.MainHud.WaitingForAnswer(false);
        }
        //catch { Debug.Log("Não é de opção"); }
        //try
        //{

        //}
        catch (System.Exception)
        {

            Debug.Log("Não pode escolher");
        }
    }

    public void NextString()
    {
        if (!GameManager.gameManager.MainHud.ActiveOptionsToChoose)
        {
            //Bug no dialogo combate
            Debug.Log("NextString");
            //Debug.Log(writing);
            //if (!writing) UpdateText(actualDialogue.NextString());
            //else
            //{
            //    CheckForDialogueOptions();
            //    writing = false;
            //}
            //CheckForDialogueOptions();
            StopCoroutine("NextStringCountdown");
            UpdateText(actualDialogue.NextString());
            //CheckForDialogueOptions();        
            //Debug.Log("Deu nextstring");
            if (activeMainDialogue && !waitingForAnswer && ((actualDialogue is DialogueBattle) || !waitingForAnswerBattle))
                StartCoroutine("NextStringCountdown");
        }
    }
    public void EndDialogue()
    {
        if (activeMainDialogue)
        {
            Debug.Log("Ending");
            activeMainDialogue = false;
            actualDialogue.ResetDialogue();
            //writing = false;
            CloseDialoguePopUp();
            waitingForAnswer = false;
            //Debug.Log("Encerrando dialogo");            
            //StopCoroutine("NextStringCountdown");
            //StopCoroutine("ResetCooldown");
            StopAllCoroutines();
        }
    }

    //Quando implementar texto escrito aos poucos, este método tem q retornar o tempo q levou pra ser escrito o texto
    public void UpdateText(string dialogueString)
    {
        Debug.Log(dialogueString);
        if (!dialogueString.Equals(""))
        {
            writingTime = dialogueString.Length * 0.1f;
            //StartCoroutine(WriteString(dialogueString));
            dialoguePopUp.SetText(dialogueString);
        }
        else EndDialogue();
        //dialogueText.text = dialogueString;
    }

    public void OpenDialogueOptTab(DialogueWithChoice d)
    {
        dialoguePopUp.OpenChoicesTab(d);
        waitingForAnswer = true;
    }
    public void CloseDialogueOptTab()
    {
        dialoguePopUp.CloseChoicesTab();
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
                dialoguePopUp.StartTimer(12);
                waitingForAnswer = true;
                //GameManager.gameManager.MainHud.WaitingForAnswer(true);
            }
        }        
    }

    public void OpenDialoguePopUp(Transform transf/*, INPC npc = null*/,Sprite sp, bool isPlayer, int id)
    {
        //if (!secondary)
        //{
            Debug.Log("Abrindo");
            try
            {
                dialoguePopUp.transform.position = mainCam.WorldToScreenPoint(transf.position);
                dialoguePopUp.gameObject.SetActive(true);
            }
            catch
            {
                GameObject aux = Instantiate(dialoguePref, GameManager.gameManager.MainHud.transform, false) as GameObject;
                dialoguePopUp = aux.GetComponent<DialoguePopUp>();
                if (mainCam == null) SetCam();
                dialoguePopUp.transform.position = mainCam.WorldToScreenPoint(transf.position);
            }
            dialoguePopUp.InitialSet(sp, isPlayer, id);
        //}
        //else
        //{
        //    try
        //    {
        //        dialoguePopUpSecond.transform.position = mainCam.WorldToScreenPoint(transf.position);
        //        dialoguePopUpSecond.gameObject.SetActive(true);
        //    }
        //    catch
        //    {
        //        GameObject aux = Instantiate(dialoguePref, GameManager.gameManager.MainHud.transform, false) as GameObject;
        //        dialoguePopUpSecond = aux.GetComponent<DialoguePopUp>();
        //        dialoguePopUpSecond.transform.position = mainCam.WorldToScreenPoint(transf.position);
        //    }
        //    dialoguePopUpSecond.InitialSet(sp);
        //}
        //Mudar posição;
        //if (npc != null) popUPB.onClick.AddListener(npc.NextString);
        //popUPB.onClick.AddListener(NextString);        
    }

    public void CloseDialoguePopUp()
    {
        if (dialoguePopUp != null)
        {
            dialoguePopUp.gameObject.SetActive(false);
            //dialoguePopUp.RemoveOnClick();
        }
        //if (dialoguePopUpSecond)
        //{
        //    dialoguePopUpSecond.gameObject.SetActive(false);
        //}
        //if (popUPB != null) popUPB.onClick.RemoveAllListeners();
    }

    public void ChangePopUpPos(/*Vector3 newPos, */Transform transf)
    {
        //dialoguePopUp.transform.position = newPos;
        dialoguePopUp.GetComponent<DialoguePopUp>().SetTransform(transf);
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
        Debug.Log("Dps do tempo de " + writingTime);
        writingTime = 0;
        if (activeMainDialogue) NextString();
    }

    //IEnumerator WriteString(string dialogueString)
    //{
    //    writing = true;
    //    string allText = "";

    //    for (int i = 0; i < dialogueString.Length; i++)
    //    {
    //        allText += dialogueString[i].ToString();
    //        //dialogueText.text = allText;
    //        dialoguePopUp.SetText(allText);
    //        if (!writing)
    //        {
    //            //dialogueText.text = dialogueString;
    //            dialoguePopUp.SetText(dialogueString);
    //            yield break;
    //        }
    //        yield return new WaitForSeconds(writingSpeed);
    //    }
    //    CheckForDialogueOptions();
    //    writing = false;
    //}

    public bool PlayerCanAnswer()
    {
        return waitingForAnswerBattle;
    }
    public void StartPlayerAnswerCountown(INPC npc)
    {
        dialoguingNPC = npc;
        waitingForAnswerBattle = true;
        //Chamar uma barrinha de tempo na hud
        StartCoroutine("ResetCooldown");
    }
    public void ContinueBattleDialogue()
    {
        waitingForAnswerBattle = true;
        StopCoroutine("ResetCooldown");
        StartCoroutine("ResetCooldown");        
    }
    public void CancelBattleDialogue()
    {
        StopCoroutine("ResetCooldown");
        waitingForAnswerBattle = false;        
        dialoguingNPC.CancelBattleDialogue();
    }
    IEnumerator ResetCooldown()
    {
        float aux = 0;
        while (!dialoguePopUp.gameObject.activeSelf)
        {
            aux += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        dialoguePopUp.StartTimer(answerTime - aux);
        yield return new WaitForSeconds(answerTime - aux);
        waitingForAnswerBattle = false;
        EndDialogue();
        dialoguingNPC.CancelBattleDialogue();
    }

    public INPC[] GetNearbyNPCs(Vector3 center, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(center, radius, LayerMask.GetMask("Interactives"));
        List<INPC> enemies = new List<INPC>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].isTrigger)
            {
                INPC npc = colliders[i].GetComponent<INPC>();
                if (npc != null) enemies.Add(npc);
            }
            //try
            //{
            //    if (!colliders[i].isTrigger)
            //    {
            //        enemies.Add(colliders[i].GetComponent<INPC>());
            //    }
            //}
            //catch { Debug.Log("Não é NPC"); }
        }
        return enemies.ToArray();
    }
}
