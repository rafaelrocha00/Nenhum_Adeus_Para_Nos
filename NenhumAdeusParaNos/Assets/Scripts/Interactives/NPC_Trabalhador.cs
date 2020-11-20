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
    [SerializeField] NavMeshAgent agent;
    [SerializeField] NPC_Movimento mov;
    ControladorTrabalho trabalho;
    [SerializeField] Animator anim;
    float contador;


    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("player");
        trabalho = GameManager.gameManager.GetComponent<ControladorTrabalho>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
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
            Mover(Player.transform.position, 1f);
        }

        if(agent.isStopped)
        {
            anim.SetBool("Walk", false);
        }
        else
        {
            anim.SetBool("Walk", true);
        }


    }

    void Mover(Vector3 posicao, float ofsset = 0.1f)
    {
        agent.isStopped = false;
        agent.destination = posicao;
        agent.stoppingDistance = ofsset;
    }

    public void FazerTrabalho(TrabalhoColeta coleta)
    {
        if (trabalhando) return;
        trabalhando = true;
        Mover(coleta.transform.position);
        StartCoroutine(Trabalhar(coleta.TempoDeTrabalho, coleta));
    }

    IEnumerator Trabalhar(float tempo, TrabalhoColeta coleta)
    {
        while (!agent.isStopped)
        {
            yield return new WaitForSeconds(0.1f);
        }
        anim.SetBool("Hit", true);
        yield return new WaitForSeconds(tempo);
        anim.SetBool("Hit", false);
        coleta.gameObject.SetActive(false);
        Mover(coleta.transform.position);
        GameManager.gameManager.inventoryController.Inventory.AddItem(coleta.itemObtido);
        trabalhando = false;
    }
}
