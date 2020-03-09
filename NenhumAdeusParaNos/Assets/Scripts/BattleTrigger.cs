using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //try
        //{
        Collider[] collidersWithin = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
        for (int i = 0; i < collidersWithin.Length; i++)
        {
            try
            {
                Debug.Log(collidersWithin[i].name);
                collidersWithin[i].GetComponent<BattleUnit>().StartBattle();
            }
            catch
            {
            }
        }

        //other.GetComponent<Player>().StartBattle();
        Debug.Log("Battle Started");
        GetComponent<Collider>().enabled = false;
        //}
        //catch
        //{
        //    Debug.Log("Not a player");
        //}
    }
}
