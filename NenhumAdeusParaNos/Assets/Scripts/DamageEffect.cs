using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//[ExecuteInEditMode]
public class DamageEffect : MonoBehaviour
{
    public Shader effectShader;
    public Color color;
    public Texture2D mask;
    public Texture2D mask2;
    [Range(0, 1)] public float density = 1;

    [Space] public AudioClip sound;
    public AudioSource audioSource;
    
    private Material effectMaterial;
    
    private static readonly int Density = Shader.PropertyToID("_Density");
    private static readonly int Color = Shader.PropertyToID("_Color");
    private static readonly int Mask = Shader.PropertyToID("_Mask");
    private static readonly int Mask2 = Shader.PropertyToID("_Mask2");

    private void Start()
    {
        effectMaterial = new Material(effectShader);
        audioSource.clip = sound;
        audioSource.loop = true;
        audioSource.Play();
        audioSource.volume = density;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (effectMaterial == null) return;
        
        effectMaterial.SetTexture(Mask, mask);
        effectMaterial.SetTexture(Mask2, mask2);
        effectMaterial.SetFloat(Density, density);
        effectMaterial.SetColor(Color, color);
        Graphics.Blit(src, dst, effectMaterial);

        audioSource.volume = density;
    }
}
