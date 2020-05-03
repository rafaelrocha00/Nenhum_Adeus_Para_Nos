using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default MeleeSpecial", menuName = "Melee Specials/Default")]
public class MeleeSpecial : ScriptableObject
{
    public float damage = 40.0f;
    [Range(3, 20)] public int animationID = 3;
    public float animationTime = 1.5f;
    public float cooldown = 2.0f;

    [HideInInspector] public BattleUnit origin { get; set; }

    public virtual void OnContactEffect(BattleUnit hittedUnit)
    {
        Debug.Log("Doesn't have this effect");
    }

    public virtual void OnAttackEffect()
    {
        Debug.Log("Doesn't have this effect");
    }

    private void OnDisable()
    {
        origin = null;
    }
}
