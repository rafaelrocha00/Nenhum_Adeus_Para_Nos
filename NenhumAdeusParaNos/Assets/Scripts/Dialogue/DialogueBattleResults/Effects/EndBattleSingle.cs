using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle_Effect
{
    [CreateAssetMenu(fileName = "New Battle Effect", menuName = "Battle Effects/End Battle Single")]
    public class EndBattleSingle : BattleEffect
    {
        public override void Effect()
        {
            thisNPC.LeaveBattle();
        }
    }
}
