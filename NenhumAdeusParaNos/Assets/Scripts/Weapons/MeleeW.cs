using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeW : Weapon
{
    public float strongAtkDamage = 40.0f;
    public float strongAtkCD = 1.0f;
    float actualAtkspeed;
    int atkType = 1;

    float selectedDamage;

    public Animator anim;
    public Collider sCollider;

    //float attackDelay = 0;
    bool hitted = false;

    private void Start()
    {
        actualAtkspeed = defaultAtkSpeed;
        selectedDamage = defaultDamage;
    }

    public void SetStrongAttack()
    {
        selectedDamage = strongAtkDamage;
        actualAtkspeed = strongAtkCD;
        atkType = 2;
    }

    public override float Attack()
    {
        Debug.Log("Atacando");
        sCollider.enabled = true;
        anim.SetInteger("AttackType", atkType);
        //attackDelay = actualAtkspeed;
        Invoke("StopAnim", actualAtkspeed - 0.1f);

        return actualAtkspeed;
    }

    void StopAnim()
    {
        anim.SetInteger("AttackType", 0);
        sCollider.enabled = false;
        selectedDamage = defaultDamage;
        actualAtkspeed = defaultAtkSpeed;
        atkType = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitted && !other.isTrigger)
        {
            try
            {
                //if (other.GetComponent<Player>() != null) other.GetComponent<Player>().CharStats.ReceiveDamage(selectedDamage);
                //else if (other.GetComponent<INPC>() != null) other.GetComponent<INPC>().CharStats.ReceiveDamage(selectedDamage);
                //if (other.GetComponent<BattleUnit>().IsInBattle())
                //{
                    other.GetComponent<BattleUnit>().ReceiveDamage(selectedDamage);
                    hitted = true;
                    Invoke("ResetHit", actualAtkspeed);
                //}
            }
            catch
            {
                Debug.Log("Not a valid target");
            }
        }
    }

    void ResetHit()
    {
        //selectedDamage = defaultDamage;
        //actualAtkspeed = defaultAtkSpeed;
        //atkType = 1;
        hitted = false;
    }
}
