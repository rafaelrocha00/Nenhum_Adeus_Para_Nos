using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    DialoguePopUp dialoguePopUp;

    Camera mainCam;

    public GameObject dialoguePref;
    public float answerTime = 4f;
    INPC dialoguingNPC;

    [HideInInspector] Dialogue actualDialogue;
    public Dialogue ActualDialogue { get { return actualDialogue; } }
    Dialogue secondaryDialogue;

    DialogueWithChoice lastDialogueWithChoice;

    [HideInInspector] bool activeMainDialogue = false;
    public bool ActiveMainDialogue { get { return activeMainDialogue; } }

    [HideInInspector] bool waitingForAnswer = false;
    public bool WaitingForAnswer { get { return waitingForAnswer; } }

    float writingTime = 0.0f;

    public List<DialogueOptions>[][][] npcAnswers = new List<DialogueOptions>[4][][];//Array das respostas dos inimigos, Uma lista de opções de respotas pra cada tipo de abordagem para cada personalidade para cada tipo de inimigo
    Dialogue[] failResults = new Dialogue[4];
    public DialogueBattleResult[][][] battleResults = new DialogueBattleResult[4][][]; //Array de resultados de batalha, primeiro Index define o tipo de inimigo, o segundo a personalidade e o terceiro o resultado;
    
    private void Start()
    {    
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && activeMainDialogue && !waitingForAnswer)
        {
            Debug.Log("NextString");
            NextString();
        }
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
    //public Dialogue GetBattleResult(int enType, int personality, DialogueBattle.ApproachType[] comb, INPC npc, Player p)
    //{
    //    Dialogue aux = failResults[personality];

    //    try
    //    {
    //        for (int i = 0; i < battleResults[enType][personality].Length; i++)
    //        {

    //            if (battleResults[enType][personality][i].apCombination.SequenceEqual(comb))
    //            {
    //                battleResults[enType][personality][i].StartEffects(npc, p);
    //                if (battleResults[enType][personality][i].GetResult())
    //                {
    //                    aux = battleResults[enType][personality][i];
    //                }
    //            }
    //        }
    //    }
    //    catch (System.Exception)
    //    {
    //        throw;
    //    }

    //    return aux;
    //}

    public void StartDialogue(Dialogue newDialogue, Transform transf, Sprite sp/*, INPC npc = null*/)
    {
        if (newDialogue is DialogueWithChoice) lastDialogueWithChoice = (DialogueWithChoice)newDialogue;
        activeMainDialogue = true;
        int id = 0;
        //bool isPlayer = (newDialogue is DialogueBattle);        
        //if (isPlayer)
        //{
        //    DialogueBattle db = (DialogueBattle)newDialogue;
        //    id = (int)db.approachType;
        //}

        OpenDialoguePopUp(transf/*, npc*/, sp,/* isPlayer,*/ id);
        actualDialogue = newDialogue;
        NextString();
        StartCoroutine("CheckPlayerDistance");

        CustomEvents.instance.OnDialogueStart(actualDialogue.MyNPC.GetName());
    }
    IEnumerator CheckPlayerDistance()
    {
        while (activeMainDialogue)
        {
            if (actualDialogue.GetPlayerNPCDistance() > 100)
            {
                EndDialogue();
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void ChooseOption(Dialogue d, Sprite sp, int index = -1)
    {
        if (!activeMainDialogue && !(actualDialogue is DialogueWithChoice)) return;

        try
        {
            Debug.Log("ChoosingDialogue");
            Dialogue newD = d;
            if (index > -1)
            {
                DialogueWithChoice dwc = (DialogueWithChoice)actualDialogue;
                if (!dwc.HasThisIndex(index)) return;
                newD = dwc.dialogueChoices[index];
                sp = dwc.MyNPC.GetPortrait();//expressions[1];
            }
            CloseDialogueOptTab();
            waitingForAnswer = false;
            StartDialogue(newD, lastDialogueWithChoice.MyNPC.GetTransform(), sp);
        }
        catch (System.Exception)
        {
            /*DialogueWith Choice dwc = (DialogueWithChoice)actualDialogue;
            if (dwc is DialogueWithChoice)*/ throw;
        }
    }

    public void NextString()
    {
        if (!GameManager.gameManager.MainHud.ActiveOptionsToChoose)
        {
            //Debug.Log("NextString");
            StopCoroutine("NextStringCountdown");
            UpdateText(actualDialogue.NextString());

            if (activeMainDialogue && !waitingForAnswer)
                StartCoroutine("NextStringCountdown");
        }
    }
    public void EndDialogue()
    {
        if (activeMainDialogue)
        {
            //Debug.Log("Ending");
            activeMainDialogue = false;
            actualDialogue.ResetDialogue();
            //writing = false;
            CloseDialoguePopUp();
            waitingForAnswer = false;
            StopAllCoroutines();
            //if (GameManager.gameManager.battleController.ActiveBattle) GameManager.gameManager.MainHud.CloseDialogueTab();

            CustomEvents.instance.OnDialogueEnd(actualDialogue.MyNPC.GetName());
        }
    }

    public void UpdateText(string dialogueString)
    {
        //Debug.Log(dialogueString);
        if (!dialogueString.Equals(""))
        {
            writingTime = dialogueString.Length * 0.1f;
            dialoguePopUp.SetText(dialogueString);
        }
        else EndDialogue();
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

    public void OpenDialoguePopUp(Transform transf/*, INPC npc = null*/,Sprite sp, /*bool isPlayer,*/ int id)
    {
        //Debug.Log("Abrindo");
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
        dialoguePopUp.SetSprite(sp);

    }

    public void CloseDialoguePopUp()
    {
        if (dialoguePopUp != null)
        {
            dialoguePopUp.gameObject.SetActive(false);
        }
    }

    public void ChangePopUpPos(Transform transf, Sprite sp)
    {
        dialoguePopUp.GetComponent<DialoguePopUp>().SetTransform(transf, sp);
    }

    IEnumerator NextStringCountdown()
    {
        //Debug.Log("Countdown");
        yield return new WaitForSeconds(writingTime);
        //Debug.Log("Dps do tempo de " + writingTime);
        writingTime = 0;
        if (activeMainDialogue) NextString();
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
        }
        return enemies.ToArray();
    }
}
