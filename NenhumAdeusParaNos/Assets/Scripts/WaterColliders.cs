using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterColliders : MonoBehaviour
{
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("player");
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning("Colidiu com agua");
        if(collision.gameObject.tag == "player")
        player.GetComponent<FallingSave>().Voltar();
    }
}
