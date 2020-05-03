using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeleeSpceial
{
    [CreateAssetMenu(fileName = "One Hit KO", menuName = "Melee Specials/One Hit KO")]
    public class OneHitKO : MeleeSpecial
    {
        public override void OnContactEffect(BattleUnit hittedUnit)
        {
            hittedUnit.ReceiveDamage(9999);
        }
    }
}