using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Item", menuName = "Item/Resource Item")]
public class ResourceItem : Item
{
    [SerializeField] int bonusTime = 0;
    public int BonusTime { get { return bonusTime; } }

    [SerializeField] int combinedValue = 0;
    public int CombinedValue { get { return combinedValue; } }


}
