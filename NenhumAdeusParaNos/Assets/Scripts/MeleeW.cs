using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeW : MonoBehaviour
{
    public float weakAtkDmage = 10;
    public float strongAtkDamage = 40;
    float selectedDamage;

    public Animator anim;
    public Collider sCollider;

    float attackDelay = 0;
    bool hitted = false;

    public void Attack(float delay, int strongAtk)
    {
        if (strongAtk == 2) selectedDamage = strongAtkDamage;
        else selectedDamage = weakAtkDmage;

        sCollider.enabled = true;
        anim.SetInteger("AttackType", strongAtk);
        attackDelay = delay;
        Invoke("StopAnim", delay - 0.1f);
    }

    void StopAnim()
    {
        anim.SetInteger("AttackType", 0);
        sCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hitted)
        {
            //try
            //{
                if (other.GetComponent<Player>() != null) other.GetComponent<Player>().CharStats.ReceiveDamage(selectedDamage);
                else if (other.GetComponent<INPC>() != null) other.GetComponent<INPC>().CharStats.ReceiveDamage(selectedDamage);

                hitted = true;
                Invoke("ResetHit", attackDelay);
            //}
            //catch
            //{
            //    Debug.Log("Not an enemy");
            //}
        }
    }

    void ResetHit()
    {
        hitted = false;
    }
}
