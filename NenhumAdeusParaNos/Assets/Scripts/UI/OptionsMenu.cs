using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] Slider Ambiente;
    [SerializeField] Slider FX;
    [SerializeField] AudioMixer Mixer;
    [SerializeField] Toggle full;

    private void Update()
    {
        Fullscreen();
    }

    public void AtualizarAmbiente(float sliderValue)
    {
        Mixer.SetFloat("VolumeMusica", Mathf.Log10(Ambiente.value) * 20f);
    }

    public void AtualizarFX(float sliderValue)
    {
        Mixer.SetFloat("VolumeFX", Mathf.Log10(FX.value) * 20f);
    }

    void Fullscreen()
    {
        if (full.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
