using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Normal Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite itemSprite;

    public bool isInstance = false;

    public bool indestructible = false;

    public Vector2Int slotSize = new Vector2Int(1, 1);

    [SerializeField] int item_value = 0;
    public int Value { get { return item_value; } }
}
