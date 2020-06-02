using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDialogueEx : MonoBehaviour
{
    public Sprite[] sprites_angry;
    public Sprite[] sprites_love;
    public Sprite[] sprites_normal;

    public Sprite[] SpritePack(int i)
    {
        switch (i)
        {
            case 0:
                return sprites_love;
            case 1:
                return sprites_normal;
            case 2:
                return sprites_angry;
            default:
                return null;
        }
    }
}
