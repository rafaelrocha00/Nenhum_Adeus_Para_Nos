using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeleeSpceial
{
    [CreateAssetMenu(fileName = "On Contact Stun", menuName = "Melee Specials/On Contact Stun")]
    public class StunOnContact : MeleeSpecial
    {
        public float stunTime = 2.5f;

        public override void OnContactEffect(BattleUnit hittedUnit)
        {
            if (hittedUnit is INPC)
            {
                INPC npc = (INPC)hittedUnit;
                npc.Stun(stunTime);
            }
        }
    }
}