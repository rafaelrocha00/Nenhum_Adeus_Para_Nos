using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            GetComponent<Collider>().enabled = false;
            if (!GameManager.gameManager.battleController.ActiveBattle)
            {
                //Invoke("DelayedTrigger", GameManager.gameManager.battleController.WaitTime);
                GameManager.gameManager.battleController.TriggerHostileNPCs(transform.position, GetComponent<SphereCollider>().radius);
                //Collider[] collidersWithin = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
                //for (int i = 0; i < collidersWithin.Length; i++)
                //{
                //    try
                //    {
                //        Debug.Log(collidersWithin[i].name);
                //        collidersWithin[i].GetComponent<BattleUnit>().StartBattle(false);
                //    }
                //    catch
                //    {
                //        Debug.Log("Não é uma battleUnit");
                //    }
                //}
                //if (GameManager.gameManager.battleController.EnoughNPCs())
                //{                
                //    GameManager.gameManager.battleController.StartBattle();
                //}
            }
        }
    }

    //void DelayedTrigger()
    //{
    //    GameManager.gameManager.battleController.TriggerHostileNPCs(transform.position, GetComponent<SphereCollider>().radius);
    //}
}
