using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCancelMusic : MonoBehaviour
{
    [SerializeField] AudioClip newMusic = null;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(MudarMusica);
    }

    void MudarMusica()
    {
        GameManager.gameManager.audioController.PlayMusic(newMusic, 0.3f);
    }
 
}
