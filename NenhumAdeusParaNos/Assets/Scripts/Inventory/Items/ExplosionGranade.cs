using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionGranade : Granade
{
    public float damage = 60.0f;

    protected override void OnLand()
    {
        Debug.Log("OnLandEffect");
        Invoke("GranadeEffect", 0.5f);
    }

    protected override void GranadeEffect()
    {
        if (effect != null) Instantiate(effect, transform.position, Quaternion.identity);
        if (!onPlayer)
        {
            INPC[] enemies = GameManager.gameManager.dialogueController.GetNearbyNPCs(transform.position, areaOfEffect);

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].ReceiveDamage(damage);
            }
        }
        else
        {
            if ((GameManager.gameManager.battleController.MainCharacter.transform.position - transform.position).sqrMagnitude <= areaOfEffect * areaOfEffect)
                GameManager.gameManager.battleController.MainCharacter.ReceiveDamage(damage);
        }

        Destroy(this.gameObject, 0.5f);
    }
}
