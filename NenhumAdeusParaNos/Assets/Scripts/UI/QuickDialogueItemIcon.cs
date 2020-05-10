using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickDialogueItemIcon : MonoBehaviour
{
    Image myImage;
    //[SerializeField] DialogueBattle[] thisBattleDialogues = new DialogueBattle[2];
    //public DialogueBattle[] DialoguesB { get { return thisBattleDialogues; } set { thisBattleDialogues = value; } }

    /*[HideInInspector] *///bool refreshing;
    //public bool Refreshing { get { return refreshing; } set { refreshing = value; } }

    //public float dialogueCooldown = 10.0f;
    public Image cooldownIm;

    //[HideInInspector] bool isMouseOverMe;
    //public bool IsMouseOverMe { get { return isMouseOverMe; } set { isMouseOverMe = value; } }

    //Button thisButton;

    //private void Start()
    //{
    //    thisButton = GetComponent<Button>();
    //}
    //private void OnDisable()
    //{
    //    isMouseOverMe = false;
    //}

    private void OnDisable()
    {
        if (cooldownIm != null) cooldownIm.gameObject.SetActive(false);
    }

    #region Botão de dialogo do slot rapido
    //Vai mudar de Color para Sprite;
    //public void SetDialogue(DialogueBattle[] newDialogues, Color newIcon)
    //{
    //    thisBattleDialogues = newDialogues;
    //    GetComponent<Image>().color = newIcon;
    //}

    //public DialogueBattle UseMyDialogue()
    //{
    //    int random = Random.Range(0, 2);
    //    DialogueBattle aux = thisBattleDialogues[random];
    //    if (!refreshing && aux != null)
    //    {
    //        refreshing = true;
    //        StartCoroutine("CoolDown");
    //        return aux;
    //    }
    //    return null;
    //}

    private void Start()
    {
        myImage = GetComponent<Image>();
    }

    public void PlaySpriteAnim()
    {
        //myImage.color = color;
        try
        {
            GetComponent<SpriteAnimator>().Play();
        }
        catch { }
    }
    public void StopSpriteAnim()
    {
        try
        {
            GetComponent<SpriteAnimator>().Stop();
        }
        catch { }
    }

    public void SetSprite(Sprite sprite)
    {
        myImage.sprite = sprite;        
    }
    public void SetColor(Color color)
    {
        myImage.color = color;
    }

    public void Cooldown(float cooldown)
    {
        StartCoroutine(CoolDown(cooldown));
    }

    IEnumerator CoolDown(float cooldown)
    {
        cooldownIm.gameObject.SetActive(true);
        float timer = 0.0f;
        while (timer < cooldown)
        {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
            cooldownIm.fillAmount = 1 - timer / cooldown;
        }
        //refreshing = false;
        cooldownIm.gameObject.SetActive(false);
    }
    #endregion
}
