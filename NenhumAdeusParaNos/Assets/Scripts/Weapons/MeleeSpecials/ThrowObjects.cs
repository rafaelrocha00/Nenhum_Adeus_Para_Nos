using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeleeSpceial
{
    [CreateAssetMenu(fileName = "Throw Objects", menuName = "Melee Specials/Throw Objects")]
    public class ThrowObjects : MeleeSpecial
    {
        public GameObject gunPref;

        public override void OnAttackEffect()
        {
            Debug.Log("On Attack");
            MonoBehaviour mb = origin as MonoBehaviour;
            if (mb != null)
            {
                Debug.Log("Spawning Gun");
                GameObject go = Instantiate(gunPref, origin.GetItemSpawnTransf().position, mb.transform.rotation) as GameObject;
                go.transform.SetParent(mb.transform);
                go.layer = mb.gameObject.layer;

                /*RangedW rw = */go.GetComponent<RangedW>().BurstAttack(true);
            }
        }
    }
}