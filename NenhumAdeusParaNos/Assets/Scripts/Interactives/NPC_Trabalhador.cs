using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Trabalhador : MonoBehaviour
{
    [SerializeField]bool alistado = false;
    [SerializeField] bool seguindo = false;
    [SerializeField]float distanciaMinima = 3f;
    [SerializeField] float distanciaMinimaParaSeguir = 3f;
    [SerializeField] GameObject Player;
    [SerializeField] SimpleNPC npc;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        npc = GetComponent<SimpleNPC>();
    }
    private void Update()
    {
        if (alistado)
        {
            checar();
            Seguir();
        }
    }
    void checar()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) <= distanciaMinima && Input.GetKeyDown(KeyCode.L))
        {
            seguindo = !seguindo;
        }
    }

    void Seguir()
    {
        if(seguindo && Vector3.Distance(transform.position, Player.transform.position) > distanciaMinimaParaSeguir)
        {
            npc.MoveNavMesh(Player.transform.position + new Vector3(1, 0, 0));
        }
    }
}
