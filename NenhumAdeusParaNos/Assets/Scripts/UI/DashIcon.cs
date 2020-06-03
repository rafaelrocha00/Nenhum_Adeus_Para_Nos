using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashIcon : MonoBehaviour
{
    public Image cooldownIm;
    public GameObject blockImage;
    public Text dashQuant;

    public void SetQuant(int q)
    {
        dashQuant.text = q.ToString();
        if (q == 0) blockImage.SetActive(true);
        else blockImage.SetActive(false);
}

    public void Cooldown(float cooldown)
    {
        StopCoroutine("CoolDown");
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
        //cooldownIm.gameObject.SetActive(false);
    }
}
