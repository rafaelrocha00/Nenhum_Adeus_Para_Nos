using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle_Effect
{
    [CreateAssetMenu(fileName = "New Battle Effect", menuName = "Battle Effects/End Battle All")]
    public class EndBattleAll : BattleEffect
    {
        public override void Effect()
        {
            GameManager.gameManager.battleController.EndAllFightersBattle();
        }
    }
}
