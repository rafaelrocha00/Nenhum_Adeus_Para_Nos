using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    Text dialogueText;
    GameObject dialoguePopUp;
    Button popUPB;

    public GameObject dialoguePref;

    public void UpdateText(string dialogueString)
    {
        dialogueText.text = dialogueString;
    }

    public void OpenDialoguePopUp(Vector3 worldPos, INPC npc)
    {
        try
        {
            dialoguePopUp.SetActive(true);            
        }
        catch
        {
            //dialoguePopUp = Instantiate(dialoguePref, GameManager.gameManager.mainHud.transform, false) as GameObject;
            //Pegar no filho do filho, e filho o botao
            //dialogueText = dialoguePopUp.GetComponentInChildren<Text>();
            //popUPB = dialoguePopUp.GetComponent<Button>();
            dialoguePopUp = Instantiate(dialoguePref, worldPos, dialoguePref.transform.rotation) as GameObject;
            dialogueText = dialoguePopUp.transform.GetChild(0).GetComponentInChildren<Text>();
            popUPB = dialoguePopUp.transform.GetChild(0).GetComponent<Button>();
        }
        //Mudar posição;
        popUPB.onClick.AddListener(npc.NextString);
    }

    public void CloseDialoguePopUp()
    {
        if (dialoguePopUp != null) dialoguePopUp.SetActive(false);
        if (popUPB != null) popUPB.onClick.RemoveAllListeners();
    }

    public void ChangePopUpPos(Vector3 newPos)
    {
        dialoguePopUp.transform.position = newPos;
    }
}
