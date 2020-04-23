using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle_Effect
{
    [CreateAssetMenu(fileName = "New Battle Effect", menuName = "Battle Effects/Stun")]
    public class Stun : BattleEffect
    {
        public bool onNPC = true;
        public float time = 5.0f;

        public override void Effect()
        {
            throw new System.NotImplementedException();
        }
    }
}
