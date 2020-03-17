using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    Text dialogueText;
    GameObject dialoguePopUp;
    Button popUPB;

    public GameObject dialoguePref;

    Dialogue actualDialogue;

    bool activeDialogue = false;
    bool writing = false;

    public void StartDialogue(Dialogue newDialogue, Transform transf/*, INPC npc = null*/)
    {
        activeDialogue = true;
        OpenDialoguePopUp(transf/*, npc*/);
        actualDialogue = newDialogue;
        NextString();
    }
    public void ChangeDialogue(Dialogue newDialogue)
    {
        actualDialogue.ResetDialogue();
        actualDialogue = newDialogue;
        NextString();
    }
    public void NextString()
    {
        //Bug no dialogo combate
        Debug.Log("NextString");
        if (!writing) UpdateText(actualDialogue.NextString());
        else writing = false;

        if (GameManager.gameManager.battleController.ActiveBattle && activeDialogue) StartCoroutine("NextStringCountdown");
    }
    public void EndDialogue()
    {
        activeDialogue = false;
        CloseDialoguePopUp();
    }

    //Quando implementar texto escrito aos poucos, este método tem q retornar o tempo q levou pra ser escrito o texto
    public void UpdateText(string dialogueString)
    {
        if (!dialogueString.Equals("")) StartCoroutine(WriteString(dialogueString));
        //dialogueText.text = dialogueString;
    }

    public void OpenDialoguePopUp(Transform transf/*, INPC npc = null*/)
    {
        try
        {
            dialoguePopUp.transform.position = transf.position;
            dialoguePopUp.SetActive(true);
        }
        catch
        {
            //dialoguePopUp = Instantiate(dialoguePref, GameManager.gameManager.mainHud.transform, false) as GameObject;
            //Pegar no filho do filho, e filho o botao
            //dialogueText = dialoguePopUp.GetComponentInChildren<Text>();
            //popUPB = dialoguePopUp.GetComponent<Button>();
            dialoguePopUp = Instantiate(dialoguePref, transf.position, dialoguePref.transform.rotation) as GameObject;
            //dialoguePopUp.GetComponent<DialoguePopUpFollow>().SetTransform(transf);
            dialogueText = dialoguePopUp.transform.GetChild(0).GetComponentInChildren<Text>();
            popUPB = dialoguePopUp.transform.GetChild(0).GetComponent<Button>();
            Debug.Log("Abrindo Balao de dialogo");
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
        Debug.Log(transf.name);
    }

    //public void TimerForNextString()
    //{
    //    StartCoroutine("NextStringCountdown");
    //}

    IEnumerator NextStringCountdown()
    {
        Debug.Log("Countdown");
        yield return new WaitForSeconds(4.0f);
        NextString();    
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
                break;
            }
            yield return new WaitForSeconds(0.05f);
        }
        writing = false;
    }
}
