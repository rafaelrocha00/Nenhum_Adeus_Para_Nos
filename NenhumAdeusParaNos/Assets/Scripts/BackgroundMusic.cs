using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip mus;
    [Range(0, 1)] public float volume = 1;

    private void Start()
    {
        GameManager.gameManager.audioController.PlayMusic(mus, volume);
    }
}
