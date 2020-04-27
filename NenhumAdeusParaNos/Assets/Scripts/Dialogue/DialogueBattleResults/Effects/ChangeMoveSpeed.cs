using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle_Effect
{
    [CreateAssetMenu(fileName = "New Battle Effect", menuName = "Battle Effects/Change Move Speed")]
    public class ChangeMoveSpeed : BattleEffect
    {
        public bool onNPC = true;
        public float percentage = 100.0f;
        public float time = 5.0f;

        public override void Effect()
        {
            if (onNPC) thisNPC.ChangeMoveSpeed(percentage, time);
        }
    }
}
