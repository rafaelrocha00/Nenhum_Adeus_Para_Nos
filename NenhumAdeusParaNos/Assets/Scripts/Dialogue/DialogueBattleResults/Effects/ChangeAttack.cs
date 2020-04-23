using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle_Effect
{
    [CreateAssetMenu(fileName = "New Battle Effect", menuName = "Battle Effects/Change Attack")]
    public class ChangeAttack : BattleEffect
    {
        public bool onNPC = true;
        public float percentage = 100.0f;

        public override void Effect()
        {
            throw new System.NotImplementedException();
        }
    }
}
