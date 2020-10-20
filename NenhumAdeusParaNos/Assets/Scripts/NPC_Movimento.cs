using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Movimento : MonoBehaviour
{
    [SerializeField]List<GameObject> Waypoints = new List<GameObject>();
    bool movendo = true;
    bool podeMover = true;
    int index = 0;
    [SerializeField]List<float> tempoParaEsperar = new List<float>();
    [SerializeField] Animator anim = null;
         
    private void Start()
    {
        MudarDestino();
        StartCoroutine(Esperar());
    }

    private void Update()
    {
        if (podeMover)
        {
            transform.position = Vector3.MoveTowards(transform.position, Waypoints[index].transform.position, Time.deltaTime);
            if(index > 0)
            {
                Rotacao(Waypoints[index].transform.position - Waypoints[index - 1].transform.position);
            }
            else
            {
                Rotacao(Waypoints[index].transform.position - Waypoints[Waypoints.Count -1].transform.position);

            }
        }

        if (movendo && podeMover)
        {
            anim.SetBool("Walking", true);
        }
        else
        {
            anim.SetBool("Walking", false);

        }

    }

    void Rotacao(Vector3 frente)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(frente), Time.deltaTime * 3f);
    }

    IEnumerator Esperar()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, Waypoints[index].transform.position) <= 0.3f)
            {
                if(tempoParaEsperar.Count > index && tempoParaEsperar[index] > 0f)
                {
                    gameObject.transform.rotation = Waypoints[index].transform.rotation;
                    movendo = false;
                    yield return new WaitForSeconds(tempoParaEsperar[index]);

                }
                MudarDestino();
            }
            yield return new WaitForSeconds(0.01f);
        }
       
    }

    public void LookAtPersonagem(Vector3 posicao)
    {
        Quaternion RotacaoAtual = gameObject.transform.rotation;
        gameObject.transform.LookAt(new Vector3(posicao.x, 0, posicao.z));
        RotacaoAtual = Quaternion.Euler(RotacaoAtual.eulerAngles.x, gameObject.transform.rotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.z);
        gameObject.transform.rotation = RotacaoAtual;
    }

    

    void MudarDestino()
    {
        index++;
        if(index == Waypoints.Count)
        {
            index = 0;
        }
        movendo = true;
    }

    public void Parar()
    {
        podeMover = false;
    }

    public void Voltar()
    {
        podeMover = true;
    }

   
}
