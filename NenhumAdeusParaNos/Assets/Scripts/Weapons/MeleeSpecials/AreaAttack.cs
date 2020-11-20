using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeleeSpceial
{
    [CreateAssetMenu(fileName = "Area Attack", menuName = "Melee Specials/Area Attack")]
    public class AreaAttack : MeleeSpecial
    {
        public float areaOfEffect = 6.0f;
        public float areaDamage = 30.0f;

        public override void OnAttackEffect()
        {
            //Vector3 oriP = origin.GetPos();
            MonoBehaviour mb = origin as MonoBehaviour;

            //if (origin is Player)
            //{
            //    INPC[] affectedE = GameManager.gameManager.dialogueController.GetNearbyNPCs(oriP, areaOfEffect);
            //    foreach (var npc in affectedE)
            //    {
            //        npc.ReceiveDamage(areaDamage);
            //    }
            //}
            //Dar dano no player e na sua companion se estiverem no alcance

            Collider[] cols = Physics.OverlapSphere(mb.transform.position, areaOfEffect);
            foreach (var col in cols)
            {
                if (!col.gameObject.layer.Equals(mb.gameObject.layer))
                {
                    //try
                    // {
                    //    col.GetComponent<BattleUnit>().ReceiveDamage(areaDamage);
                    //}
                    //catch { Debug.Log("Not a Battle unit"); }
                    if (col.GetComponent<BattleUnit>() == null) Debug.Log("Not a Battle unit");
                    else
                    {
                        col.GetComponent<BattleUnit>().ReceiveDamage(areaDamage);
                    }
                }
            }
        }
    }
}