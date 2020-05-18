using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullGranade : Granade
{
    public float pullForce = 5.0f;
    public float stunTime = 2.0f;

    protected override void OnLand()
    {
        Debug.Log("OnLandEffect");
        Invoke("GranadeEffect", 0.5f);
    }

    protected override void GranadeEffect()
    {
        if (effect != null) Instantiate(effect, transform.position, Quaternion.identity);
        INPC[] enemies = GameManager.gameManager.dialogueController.GetNearbyNPCs(transform.position, areaOfEffect);
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].Stun(stunTime);
            StartCoroutine(Pull(enemies[i].transform));
        }

        Destroy(this.gameObject, 0.5f);
    }

    //void DetectColliders()
    //{
    //    //Collider[] colliders = Physics.OverlapSphere(transform.position, areaOfEffect, layerMask);
    //    //for (int i = 0; i < colliders.Length; i++)
    //    //{
    //    //    try
    //    //    {
    //    //        if (!colliders[i].isTrigger)
    //    //        {
    //    //            colliders[i].GetComponent<INPC>().Stun(stunTime);
    //    //            StartCoroutine(Pull(colliders[i].transform));
    //    //        }
    //    //    }
    //    //    catch { Debug.Log("Não é NPC"); }
    //    //}
    //    Destroy(this.gameObject, 0.5f);
    //}

    IEnumerator Pull(Transform transf)
    {
        float timer = 0.0f;
        Vector3 origin = transf.position;
        while (timer < 1)
        {
            transf.position = Vector3.Lerp(origin, transform.position, timer);
            timer += Time.deltaTime * pullForce;
            yield return new WaitForEndOfFrame();
        }        
    }
}
