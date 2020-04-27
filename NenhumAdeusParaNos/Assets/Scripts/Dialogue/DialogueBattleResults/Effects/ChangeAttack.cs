using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle_Effect
{
    [CreateAssetMenu(fileName = "New Battle Effect", menuName = "Battle Effects/Change Attack")]
    public class ChangeAttack : BattleEffect
    {
        public bool onNPC = true;
        public float attackMod = 1;

        public override void Effect()
        {
            thisNPC.ChangeAttackMod(1);
        }
    }
}
