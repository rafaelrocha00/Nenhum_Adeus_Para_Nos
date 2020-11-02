using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Item", menuName = "Item/Resource Item")]
public class ResourceItem : Item
{
    public enum MaterialType { Wood, Metal, Plastic, Stone, Trash, Paper };

    [SerializeField] MaterialType matType = MaterialType.Wood;
    public MaterialType MatType { get { return matType; } }

    [SerializeField] int bonusTime = 0;
    public int BonusTime { get { return bonusTime; } }

    [SerializeField] char rarity = 'E';
    public int Rarity { get { return rarity; } }

    public float GetDropChance()
    {
        switch (rarity)
        {
            case 'E':
                return 0.1f;
            case 'D':
                return 0.25f;
            case 'C':
                return 0.3f;
            case 'B':
                return 0.25f;
            case 'A':
                return 0.1f;
            case 'S':
                return 0.05f;
            default:
                return 0.1f;
        }
    }
}
