using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteAnimator : MonoBehaviour
{
    public float animTime = 0.15f;
    Image image;

    [SerializeField] Sprite[] sprites = new Sprite[1];
    public Sprite[] Sprites { get { return sprites; } }

    public bool playOnEnable = false;
    public float delayToPlay = 0.0f;
    public bool hideOnEndAnim = false;

    public void SetSprites(Sprite[] sps)
    {
        sprites = new Sprite[sps.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = sps[i];
        }
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        image.sprite = sprites[0];
        if (playOnEnable)
        {
            //Play();
            Invoke("StartPlay", delayToPlay);
        }
    }

    void StartPlay()
    {
        Play();
    }
    //private void Start()
    //{        

    //    if (playOnEnable) Play();
    //}

    public void Play(bool infinity = false, int cycles = 1)
    {
        StartCoroutine(Animate(infinity, cycles));
    }
    public void Stop()
    {
        StopAllCoroutines();
    }

    IEnumerator Animate(bool infinity, int cycles = -1)
    {
        if (image == null) image = GetComponent<Image>();
        while (infinity || cycles > 0)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                image.sprite = sprites[i];
                yield return new WaitForSeconds(animTime);
            }
            cycles--;
        }

        if (hideOnEndAnim) gameObject.SetActive(false);
    }
}
