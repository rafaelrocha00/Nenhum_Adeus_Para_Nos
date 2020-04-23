using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle_Effect
{
    public abstract class BattleEffect : ScriptableObject
    {
        protected INPC thisNPC;
        protected Player mainC;

        public void Inialize(INPC npc, Player mc)
        {
            thisNPC = npc;
            mainC = mc;
        }

        public abstract void Effect();

    }
}
