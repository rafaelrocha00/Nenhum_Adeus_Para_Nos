using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Companion : NPC
{
    INPC closestEnemy;

    protected override void Initialize()
    {
        ignoreBarrier = true;
        mCharacter = GameManager.gameManager.battleController.MainCharacter;
    }

    protected override void Movement()
    {
        if (inBattle)
        {
            if (inBattleTarget == null)
            {
                try
                {
                    INPC[] enemies = GameManager.gameManager.battleController.AllEnemyFighters.ToArray();
                    closestEnemy = enemies[0];
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        if ((enemies[i].transform.position - transform.position).sqrMagnitude < (closestEnemy.transform.position - transform.position).sqrMagnitude)
                            closestEnemy = enemies[i];
                    }
                    inBattleTarget = closestEnemy.transform;
                }
                catch { }
            }
            else { InBattleBehaviour(); }
        }
        else
        {
            if (mCharacter == null) mCharacter = GameManager.gameManager.battleController.MainCharacter;
            Vector3 toPlayerVec = mCharacter.transform.position - transform.position;
            Vector3 targPos = toPlayerVec.normalized * (toPlayerVec.magnitude - 2) + transform.position;

            MoveNavMesh(targPos);
        }
    }

    protected override void ComfirmAttack()
    {
        float cd = myWeapon.Attack(null);
        Invoke("AttackCooldown", cd);
    }

    public override void StartBattle(bool byTrigger = true)
    {
        inBattle = true;
        lifeBar.transform.parent.gameObject.SetActive(true);
    }
    public override void EndBattle()
    {
        inBattle = false;
        lifeBar.transform.parent.gameObject.SetActive(false);
    }
    public override bool ReceiveDamage(float damage)
    {
        charStats.ReceiveDamage(damage);
        if (lifeBar != null) lifeBar.fillAmount = charStats.LifePercentage();
        return false;
    }
}
