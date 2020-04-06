using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Normal Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;

    public Vector2Int slotSize = new Vector2Int(1, 1);
}
