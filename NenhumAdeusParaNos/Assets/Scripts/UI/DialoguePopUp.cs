using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePopUp : MonoBehaviour
{
    Transform attachedTransform;
    Camera mainCam;

    public Text dialogueText;
    public Image speakerImage;
    public Button thisButton;
    float fixedY = 0;
    //Vector3 fixedDistance;

    public PlayerDialogueEx playerD;

    public Image timerBar;

    public GameObject dialogueOptionsTab;
    public DialogueOption[] dialogueOptions;

    private void Awake()
    {
        attachedTransform = transform;
        //fixedDistance = Vector3.zero;

        Debug.Log("Start");
    }

    private void Start()
    {
        mainCam = GameManager.gameManager.MainCamera.GetComponent<Camera>();
    }

    private void OnDisable()
    {
        timerBar.transform.parent.gameObject.SetActive(false);
    }

    public void SetTransform(Transform atTransform)
    {
        attachedTransform = atTransform;
        //transform.position = mainCam.WorldToScreenPoint(attachedTransform.position);
        fixedY = attachedTransform.position.y;
        //float xDis;
        //float yDis;
        //float zDis;
        //xDis = transform.position.x - attachedTransform.position.x;
        //yDis = transform.position.y - attachedTransform.position.y;
        //zDis = transform.position.z - attachedTransform.position.z;
        //fixedDistance = new Vector3(xDis, yDis, zDis);

        Debug.Log("Setando Transf");
    }

    private void Update()
    {
        transform.position = mainCam.WorldToScreenPoint(new Vector3(attachedTransform.position.x, fixedY, attachedTransform.position.z));// + fixedDistance;   
    }

    public void SetText(string tex)
    {
        dialogueText.text = tex;
    }
    public void InitialSet(Sprite sp = null, /*bool player = false,*/ int aType = 0)
    {
        //playerD.gameObject.SetActive(false);
        //thisButton.onClick.AddListener(act);
        speakerImage.sprite = sp;
        //if (player)
        //{
        //    playerD.gameObject.SetActive(true);
        //    transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(-75, 0);
        //    SpriteAnimator sa = playerD.GetComponent<SpriteAnimator>();
        //    sa.SetSprites(playerD.SpritePack(aType));
        //    sa.Play(true);
        //}
        //else
        //{
        //    playerD.gameObject.SetActive(false);
        //    transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        //}
    }
    //public void RemoveOnClick()
    //{
    //    thisButton.onClick.RemoveAllListeners();
    //}

    public void StartTimer(float t)
    {
        timerBar.transform.parent.gameObject.SetActive(true);
        StartCoroutine(Timer(t));
    }
    IEnumerator Timer(float to)
    {
        float t = to;
        while (t > 0)
        {
            timerBar.rectTransform.localScale = new Vector3(t / to, 1);
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        timerBar.transform.parent.gameObject.SetActive(false);
    }

    public void OpenChoicesTab(DialogueWithChoice d)
    {
        dialogueOptionsTab.SetActive(true);

        for (int i = 0; i < d.options.Length; i++)
        {
            if (d.optionToUnlock == i)
            {
                InvenSlot aux = GameManager.gameManager.inventoryController.Inventory.FindItem(d.requiredItem.itemName);
                if (aux != null) dialogueOptions[i].SetOption(d.options[i], i, d.dialogueChoices[i], d.MyNPC, d.MainCharacter);
            }
            else
            {
                dialogueOptions[i].SetOption(d.options[i], i, d.dialogueChoices[i], d.MyNPC, d.MainCharacter);
            }
        }
    }
    public void CloseChoicesTab()
    {
        dialogueOptionsTab.SetActive(false);
        for (int i = 0; i < dialogueOptions.Length; i++)
        {
            dialogueOptions[i].CleanText();
        }
    }
}
