using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Trabalhador : MonoBehaviour
{
    [SerializeField]bool alistado = false;
    [SerializeField]bool seguindo = false;
    [SerializeField] bool trabalhando = false;
    [SerializeField]float distanciaMinima = 3f;
    [SerializeField] float distanciaMinimaParaSeguir = 3f;
    [SerializeField] GameObject Player;
    [SerializeField] SimpleNPC npc;
    [SerializeField] NPC_Movimento mov;
    ControladorTrabalho trabalho;
    float contador;


    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("player");
        trabalho = GameManager.gameManager.GetComponent<ControladorTrabalho>();
        npc = GetComponent<SimpleNPC>();
        mov = GetComponent<NPC_Movimento>();
    }
    private void Update()
    {
        if (alistado && !trabalhando)
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
            if (seguindo)
            {
                trabalho.adicionarNpc(this);
                if (mov != null) mov.Parar();

            }
            else
            {
                trabalho.RemoverNPC(this);
                if (mov != null) mov.Voltar();

            }
        }
    }

    void Seguir()
    {
        if(seguindo && Vector3.Distance(transform.position, Player.transform.position) > distanciaMinimaParaSeguir)
        {
            npc.MoveNavMesh(Player.transform.position + new Vector3(1, 0, 0));
        }
    }

    public void FazerTrabalho(TrabalhoColeta coleta)
    {
        if (trabalhando) return;
        trabalhando = true;
        npc.MoveNavMesh(coleta.transform.position);
        StartCoroutine(Trabalhar(coleta.TempoDeTrabalho, coleta));
    }

    IEnumerator Trabalhar(float tempo, TrabalhoColeta coleta)
    {
        yield return new WaitForSeconds(tempo);
        coleta.gameObject.SetActive(false);
        npc.MoveNavMesh(coleta.transform.position);
        GameManager.gameManager.inventoryController.Inventory.AddItem(coleta.itemObtido);
        trabalhando = false;
    }
}
