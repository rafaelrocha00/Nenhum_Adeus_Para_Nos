using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    [SerializeField]float RaioDeVisao = 3;
    [SerializeField]float VelocidadeCorrendo = 5;
    [SerializeField]float TempoGeralParado = 2;
    [SerializeField]float VelocidadeAndando = 1;
    [SerializeField] float VelocidadeRotacao = 3f;
    [SerializeField] bool FogeDoJogador = true;
    bool vagando = false;
    //bool fugindo = false;
    Vector3 direcaoFinal = Vector3.zero;
    Transform jogador;
    Animator animator;
    NavMeshAgent agente;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        jogador = GameManager.gameManager.battleController.MainCharacter.gameObject.transform;
        agente = GetComponentInChildren<NavMeshAgent>();

        vagando = true;
        StartCoroutine(EncontrarNovaDirecao());
    }

    private void Update()
    {

        if (Vector3.Distance(transform.position, jogador.position) <= RaioDeVisao && FogeDoJogador)
        {
            vagando = false;
            //fugindo = true;
            agente.speed = VelocidadeCorrendo;
            Fugir();
        }
        else
        {
            agente.speed = VelocidadeAndando;
            vagando = true;
            //fugindo = false;
        }

        if (direcaoFinal != Vector3.zero)
        {
            agente.SetDestination(direcaoFinal+transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direcaoFinal),Time.deltaTime*VelocidadeRotacao);

            animator.SetBool("Walking", true);

        }
        else
        {
            animator.SetBool("Walking", false);

        }

    

    }

    IEnumerator EncontrarNovaDirecao()
    {
        while (true)
        {
            if (vagando)
            {
                Vector2 direcao = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                direcaoFinal = new Vector3(direcao.x, 0, direcao.y);
            }
       

            yield return new WaitForSeconds(TempoGeralParado);
        }

    }

    void Fugir()
    {


                direcaoFinal = Vector3.Normalize(new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(jogador.position.x, 0, jogador.position.z));
        
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, RaioDeVisao);
        Gizmos.DrawLine(transform.position, direcaoFinal + transform.position);
    }
}
