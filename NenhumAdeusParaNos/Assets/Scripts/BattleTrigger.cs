using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.gameManager.battleController.ActiveBattle)
        {
            //try
            //{

            Collider[] collidersWithin = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
            for (int i = 0; i < collidersWithin.Length; i++)
            {
                try
                {
                    Debug.Log(collidersWithin[i].name);
                    collidersWithin[i].GetComponent<BattleUnit>().StartBattle(true);
                }
                catch
                {
                    Debug.Log("Não é uma battleUnit");
                }
            }

            //other.GetComponent<Player>().StartBattle();
            Debug.Log("Battle Started");
            GetComponent<Collider>().enabled = false;

            GameManager.gameManager.battleController.StartBattle();
            //}
            //catch
            //{
            //    Debug.Log("Not a player");
            //}
        }
    }
}
