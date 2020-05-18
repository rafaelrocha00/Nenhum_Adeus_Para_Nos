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

    private void Start()
    {
        image.sprite = sprites[0];
    }

    public void Play(bool infinity = false)
    {
        StartCoroutine(Animate(infinity, 2));
    }
    public void Stop()
    {
        StopAllCoroutines();
    }

    IEnumerator Animate(bool infinity, int cycles = -1)
    {
        while (infinity || cycles > 0)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                image.sprite = sprites[i];
                yield return new WaitForSeconds(animTime);
            }
            cycles--;
        }
    }
}
