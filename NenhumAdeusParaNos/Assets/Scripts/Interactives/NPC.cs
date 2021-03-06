﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class NPC : Interactives, BattleUnit, IDialogueable
{
    [Header("NPC")]
    [HideInInspector] protected CharacterStats charStats;
    public CharacterStats CharStats { get { return charStats; } }

    public Sprite portrait;

    public Animator anim;
    public Image lifeBar;

    protected NavMeshAgent navMesh;

    protected bool inBattle = false;

    [HideInInspector] protected Player mCharacter;
    public Player MCharacter { get { return mCharacter; } set { mCharacter = value; } }    

    protected bool attacking = false;
    public Weapon myWeapon;
    protected RangedW rangedW;
    protected MeleeW meleeW;
    protected bool strongAtk = false;
    protected bool isRanged = false;
    protected bool ignoreBarrier = false;

    public float defaultSpeed = 6.0f;
    public float rangedKiteSpeed = 4.0f;

    protected bool moveSpeedChanged = false;

    protected Transform inBattleTarget;

    protected float atkInterval = 0.15f;
    protected float timer = 0.0f;

    protected bool canReceiveKnockback = true;

    private void Start()
    {
        charStats = new CharacterStats(this);
        navMesh = GetComponent<NavMeshAgent>();

        //if (anim == null) anim = GetComponentInChildren<Animator>();

        if (myWeapon == null) myWeapon = GetComponentInChildren<Weapon>();

        if (myWeapon != null)
        {
            if (myWeapon is RangedW)
            {
                isRanged = true;
                rangedW = (RangedW)myWeapon;
            }
            else meleeW = (MeleeW)myWeapon;
            myWeapon.myHolder = this;
        }

        navMesh.speed = defaultSpeed;
        Initialize();
    }

    protected virtual void Initialize() { }

    private void Update()
    {
        Movement();
    }

    protected void InBattleBehaviour()
    {
        Ray ray = new Ray(transform.position, inBattleTarget.position - transform.position);
        RaycastHit hit;

        Vector3 lookPos = new Vector3(inBattleTarget.position.x, transform.position.y, inBattleTarget.transform.position.z);
        transform.LookAt(lookPos);


        if (isRanged)
        {
            if (!rangedW.rangedWConfig.stopToShoot || (rangedW.rangedWConfig.stopToShoot && !attacking))
            {
                if ((inBattleTarget.position - transform.position).sqrMagnitude <= myWeapon.GetRange() * myWeapon.GetRange())
                {
                    Vector3 desiredPos = -(inBattleTarget.position - transform.position) + transform.position;
                    MoveNavMesh(desiredPos);
                    if (!moveSpeedChanged) navMesh.speed = rangedKiteSpeed;
                }
                else if ((inBattleTarget.position - transform.position).sqrMagnitude >= rangedW.GetMaxRange() * rangedW.GetMaxRange())
                {
                    Vector3 toPlayerVec = inBattleTarget.position - transform.position;
                    Vector3 desiredPos = toPlayerVec.normalized * (toPlayerVec.magnitude - rangedW.GetMaxRange() * 0.2f) + transform.position;
                    if (!moveSpeedChanged) navMesh.speed = defaultSpeed;
                    MoveNavMesh(desiredPos);
                }
                else
                {
                    navMesh.isStopped = true;
                    //parar animação de andar
                }

                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log(hit.collider.name);
                    if (hit.collider.CompareTag("player") || hit.collider.CompareTag("barrier") || /*Vector3.Distance*/(hit.transform.position - transform.position).sqrMagnitude >= /*Vector3.Distance*/(inBattleTarget.position - transform.position).sqrMagnitude)
                    {
                        TryAttack();
                    }
                }
                else TryAttack();
            }
            else navMesh.isStopped = true;
        }
        else
        {
            if (!meleeW.meleeConfig.stopToAtk || (meleeW.meleeConfig.stopToAtk && !attacking))
            {
                Vector3 toPlayerVec = inBattleTarget.position - transform.position;
                Vector3 desiredPos = toPlayerVec.normalized * (toPlayerVec.magnitude - myWeapon.GetRange() * 0.6f) + transform.position;
                MoveNavMesh(desiredPos);
                if ((inBattleTarget.position - transform.position).sqrMagnitude <= myWeapon.GetRange() * myWeapon.GetRange())
                {
                    TryAttack();
                }
                else if (!ignoreBarrier && Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("barrier") && (hit.transform.position - transform.position).sqrMagnitude <= myWeapon.GetRange() * myWeapon.GetRange())
                    {
                        TryAttack();
                    }
                }
            }
            else navMesh.isStopped = true;
        }
    }

    protected abstract void Movement();

    public void MoveNavMesh(Vector3 pos)
    {
        if (navMesh == null) navMesh = GetComponent<NavMeshAgent>();
        //Debug.Log("Movendo NPc");
        navMesh.isStopped = false;
        navMesh.destination = pos;
    }

    public virtual void EndDialogue()
    {
        if (mCharacter == null) mCharacter = GameManager.gameManager.battleController.MainCharacter;
        OnExit(mCharacter);
        EndInteraction();
        GameManager.gameManager.dialogueController.EndDialogue();
    }

    public void ReceiveItem()
    {
        if (anim != null) anim.SetTrigger("ReceivedItem");
    }

    protected void TryAttack()
    {
        timer += Time.deltaTime;
        if (timer >= atkInterval)
        {
            bool canAtk = true;
            if (isRanged && rangedW.HasAmmo())
            {
                canAtk = true;
            }
            else if (isRanged)
            {
                //animação de reload
                canAtk = false;
            }

            if (canAtk) Attack();
            timer = 0.0f;
        }
    }

    protected void Attack()
    {
        if (!attacking)
        {
            if (anim != null)
            {
                if (isRanged) anim.SetInteger("AtkType", 1);
                else anim.SetInteger("AtkType", 2);
            }            
            //Debug.Log("Atacando");
            attacking = true;

            if (!isRanged)
            {
                if (strongAtk)
                {
                    MeleeW myMelee = (MeleeW)myWeapon;
                    myMelee.SetStrongAttack();
                }
                ComfirmAttack();
            }
            else
            {
                Invoke("DelayedAttack", rangedW.GetDelayToShoot());
            }
        }
    }
    protected void DelayedAttack()
    {
        ComfirmAttack();
    }

    protected abstract void ComfirmAttack();

    protected void AttackCooldown()
    {
        attacking = false;
        if (anim != null) anim.SetInteger("AtkType", 0);
    }

    public virtual bool CanFight()
    {
        return charStats.CanFight;
    }

    public virtual void Die()
    {
        if (anim != null)
        {
            anim.SetInteger("AtkType", 0);
            anim.SetBool("Died", true);
        }
        Destroy(this.gameObject, 3f);
    }

    public virtual void EndBattle()
    {
        
    }

    public virtual Vector3 GetPos()
    {
        return transform.position;
    }

    public virtual bool IsInBattle()
    {
        return inBattle;
    }

    public virtual bool ReceiveDamage(float damage)
    {
        float acLife = charStats.Life;
        charStats.ReceiveDamage(damage);
        if (lifeBar != null) lifeBar.fillAmount = charStats.LifePercentage();
        if (acLife != charStats.Life && anim != null)
        {
            anim.SetBool("Damaging", true);
            Invoke("ResetHit", 0.1f);
        }
        return false;
    }
    void ResetHit()
    {
        anim.SetBool("Damaging", false);
    }

    public virtual Transform GetItemSpawnTransf()
    {
        return null;


    }

    public virtual void StartBattle(bool byTrigger = true)
    {
        
    }

    public virtual void Knockback(float dis)
    {

    }

    public string GetName()
    {
        return Name;
    }

    public Sprite GetPortrait()
    {
        return portrait;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
