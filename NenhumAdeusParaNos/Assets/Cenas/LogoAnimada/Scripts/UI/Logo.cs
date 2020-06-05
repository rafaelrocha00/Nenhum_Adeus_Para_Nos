using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
    [SerializeField]string nomeCena = "";

    private void OnEnable()
    {
        Debug.Log("Trocar Cena foi chamado");
        // C_Jogo.instancia.TrocarLocal(nomeCena);
        Invoke("ChangeScene", 4f);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(nomeCena);
    }
}
