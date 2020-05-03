using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeleeSpceial
{
    [CreateAssetMenu(fileName = "Melee Dash", menuName = "Melee Specials/Melee Dash")]
    public class MeleeDash : MeleeSpecial
    {
        public float dashDamage;

        public override void OnAttackEffect()
        {
            try
            {
                Player p = (Player)origin;
                p.DamageDash(dashDamage);
            }
            catch
            {
                INPC npc = (INPC)origin;
                npc.CallDamagingDash(dashDamage);
            }
        }
    }
}