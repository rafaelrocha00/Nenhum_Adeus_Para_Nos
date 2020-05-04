using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeW : Weapon
{
    public MeleeConfig meleeConfig;

    //public float strongAtkDamage = 40.0f;
    //public float strongAtkCD = 1.0f;
    float actualAtkspeed;
    int atkType = 2;
    bool specialAtkEnabled = true;

    float selectedDamage;

    public Animator anim;
    public Collider sCollider;

    //float attackDelay = 0;
    bool hitted = false;

    private void Start()
    {
        //meleeConfig = (MeleeConfig)weaponConfig;
        weaponConfig = meleeConfig;

        actualAtkspeed = meleeConfig.defaultAttackSpeed;
        selectedDamage = meleeConfig.defaultDamage;        
    }

    public void SetStrongAttack()
    {
        selectedDamage = meleeConfig.special.damage;
        actualAtkspeed = meleeConfig.special.animationTime;
        atkType = meleeConfig.special.animationID;
    }
    public void SetNormalAttack()
    {
        selectedDamage = meleeConfig.defaultDamage;
        actualAtkspeed = meleeConfig.defaultAttackSpeed;
        atkType = 2;
    }

    public override float Attack(Animator animator = null, float attackMod = 1)
    {
        if (atkType >= 3 && !specialAtkEnabled)
        {
            Debug.Log("Incooldown");
            SetNormalAttack();
            return 0;
        }
        else if (atkType >= 3 && specialAtkEnabled)
        {
            Debug.Log("Special Attack");
            specialAtkEnabled = false;
            Invoke("InvokeOnAttackEffect", 0.5f);
            Invoke("SpecialAttackCooldown", meleeConfig.special.cooldown);
        }

        Debug.Log("Atacando");
        sCollider.enabled = true;
        selectedDamage *= attackMod;
        anim.SetInteger("AttackType", atkType);
        if (animator != null) animator.SetInteger("Attacking", atkType);
        //attackDelay = actualAtkspeed;
        Invoke("StopAnim", actualAtkspeed - 0.1f);

        return meleeConfig.defaultAttackSpeed;
    }

    void InvokeOnAttackEffect()
    {
        if (meleeConfig.special.origin == null) meleeConfig.special.origin = myHolder;
        meleeConfig.special.OnAttackEffect();
    }

    void SpecialAttackCooldown()
    {
        specialAtkEnabled = true;
    }

    void StopAnim()
    {
        anim.SetInteger("AttackType", 0);
        sCollider.enabled = false;
        SetNormalAttack();
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
                if (atkType >= 3)meleeConfig.special.OnContactEffect(other.GetComponent<BattleUnit>());
                hitted = true;
                Invoke("ResetHit", meleeConfig.defaultAttackSpeed);
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

    public override void Equip(WeaponConfig config)
    {
        meleeConfig = (MeleeConfig)config;
        weaponConfig = meleeConfig;
    }
}
