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
    //Vector3 fixedDistance;

    public Image timerBar;

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
        transform.position = mainCam.WorldToScreenPoint(attachedTransform.position);// + fixedDistance;   
    }

    public void SetText(string tex)
    {
        dialogueText.text = tex;
    }
    public void InitialSet(UnityEngine.Events.UnityAction act, Sprite sp = null)
    {
        thisButton.onClick.AddListener(act);
        speakerImage.sprite = sp;
    }
    public void RemoveOnClick()
    {
        thisButton.onClick.RemoveAllListeners();
    }

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
}
