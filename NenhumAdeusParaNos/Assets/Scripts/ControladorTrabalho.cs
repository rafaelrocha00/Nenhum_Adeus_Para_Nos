using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorTrabalho : MonoBehaviour
{
    List<NPC_Trabalhador> npcs = new List<NPC_Trabalhador>();
    GameObject jogador;
    [SerializeField] float raioDeVisao = 3f;

    void Start()
    {
        jogador = GameObject.FindGameObjectWithTag("player");
    }

    public void adicionarNpc(NPC_Trabalhador npc)
    {
        npcs.Add(npc);
    }

    public void RemoverNPC(NPC_Trabalhador npc)
    {
        npcs.Remove(npc);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (jogador == null) jogador = GameObject.FindGameObjectWithTag("player");
            Collider[] hitColliders = Physics.OverlapSphere(jogador.transform.position, raioDeVisao);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if(hitColliders[i].gameObject.tag == "Coletavel")
                {
                    DarTrabalho(hitColliders[i].gameObject.GetComponent<TrabalhoColeta>());
                }
            }

        }
    }

    void DarTrabalho(TrabalhoColeta coleta)
    {
        int MelhorNPC = 0;
        float DistanciaMinima = 999f;
        for (int i = 0; i < npcs.Count; i++)
        {
            float dis = Vector3.Distance(npcs[i].transform.position, coleta.transform.position);
            if(dis < DistanciaMinima)
            {
                MelhorNPC = i;
                DistanciaMinima = dis;
            }
        }

        npcs[MelhorNPC].FazerTrabalho(coleta);
    }
}
