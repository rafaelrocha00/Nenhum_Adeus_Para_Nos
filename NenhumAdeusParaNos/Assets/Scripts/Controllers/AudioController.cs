using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource source_music;
    public Transform effectSourceHolder;
    public AudioSource[] source_effects;

    private void Start()
    {
        source_effects = GetComponentsInChildren<AudioSource>();
    }

    public void PlayMusic(AudioClip clip, float vol = 1)
    {
        if (clip == null)
        {
            source_music.Stop();
            return;
        }

        source_music.volume = vol;
        source_music.clip = clip;
        source_music.Play();
    }

    public void PlayEffect(AudioClip clip, bool loop = false, float timeToStop = 0, bool randomPitch = false)
    {
        if (clip == null) return;

        for (int i = 0; i < source_effects.Length; i++)
        {
            if (!source_effects[i].isPlaying)
            {
                source_effects[i].clip = clip;
                source_effects[i].loop = loop;
                if (randomPitch) source_effects[i].pitch = Random.Range(0.85f, 1.15f);
                else source_effects[i].pitch = 1;

                source_effects[i].Play();                

                if (loop) StartCoroutine(StopEffect(i, timeToStop));
                break;
            }
        }
    }

    IEnumerator StopEffect(int idx, float t)
    {
        yield return new WaitForSeconds(t);
        source_effects[idx].Stop();
    }
}
