using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogueB : MonoBehaviour
{
    [SerializeField] DialogueBattle[] thisBattleDialogues = new DialogueBattle[2];
    public DialogueBattle[] DialoguesB { get { return thisBattleDialogues; } set { thisBattleDialogues = value; } }

    /*[HideInInspector] */bool refreshing;
    //public bool Refreshing { get { return refreshing; } set { refreshing = value; } }

    public float dialogueCooldown = 10.0f;
    public Image cooldownIm;

    [HideInInspector] bool isMouseOverMe;
    public bool IsMouseOverMe { get { return isMouseOverMe; } set { isMouseOverMe = value; } }

    //Button thisButton;

    //private void Start()
    //{
    //    thisButton = GetComponent<Button>();
    //}
    private void OnDisable()
    {
        isMouseOverMe = false;
    }

    #region Botão de dialogo do slot rapido
    //Vai mudar de Color para Sprite;
    public void SetDialogue(DialogueBattle[] newDialogues, Color newIcon)
    {
        thisBattleDialogues = newDialogues;
        GetComponent<Image>().color = newIcon;
    }

    public DialogueBattle UseMyDialogue()
    {
        int random = Random.Range(0, 2);
        DialogueBattle aux = thisBattleDialogues[random];
        if (!refreshing && aux != null)
        {
            refreshing = true;
            StartCoroutine("CoolDown");
            return aux;
        }
        return null;
    }

    IEnumerator CoolDown()
    {
        cooldownIm.gameObject.SetActive(true);
        float timer = 0.0f;
        while (timer < dialogueCooldown)
        {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
            cooldownIm.fillAmount = 1 - timer / dialogueCooldown;
        }
        refreshing = false;
        cooldownIm.gameObject.SetActive(false);
    }
    #endregion
}
