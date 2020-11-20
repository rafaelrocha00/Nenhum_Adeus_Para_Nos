using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeleeSpceial
{
    [CreateAssetMenu(fileName = "Knockback", menuName = "Melee Specials/Knockback")]
    public class Knockback : MeleeSpecial
    {
        public float knockbackDist = 2.5f;

        public override void OnContactEffect(BattleUnit hittedUnit)
        {
            if (hittedUnit is INPC)
            {
                INPC npc = (INPC)hittedUnit;
                npc.Knockback(knockbackDist);
            }
        }
    }
}
