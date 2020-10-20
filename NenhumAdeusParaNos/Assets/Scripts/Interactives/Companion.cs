using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Companion : NPC
{
    [Header("Companion")]
    INPC closestEnemy;

    public GameObject myPref;

    public GameObject npcToSpawnPref;

    public Dialogue warningDialogue;

    protected override void Initialize()
    {
        ignoreBarrier = true;
        mCharacter = GameManager.gameManager.battleController.MainCharacter;

        CustomEvents.instance.onExitTrajectory += WarnPlayer;
    }

    private void OnDestroy()
    {
        CustomEvents.instance.onExitTrajectory -= WarnPlayer;
    }

    protected override void Movement()
    {
        float actualVelocity = navMesh.velocity.magnitude / navMesh.speed;

        if (anim != null) anim.SetFloat("Vel", actualVelocity);

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
            Vector3 targPos = toPlayerVec.normalized * (toPlayerVec.magnitude) + transform.position;

            MoveNavMesh(targPos);
        }
    }

    public void WarnPlayer()
    {
        if (warningDialogue == null) return;

        Debug.Log("Avisando o player");

        warningDialogue.MyNPC = this;
        warningDialogue.MainCharacter = mCharacter;

        GameManager.gameManager.dialogueController.StartDialogue(warningDialogue, transform, portrait);
    }

    public void DirectMove(Vector3 newPos)
    {
        navMesh.enabled = false;
        transform.position = newPos;
        navMesh.enabled = true;
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
        inBattleTarget = null;
        lifeBar.transform.parent.gameObject.SetActive(false);
    }
    public override bool ReceiveDamage(float damage)
    {
        charStats.ReceiveDamage(damage);
        if (lifeBar != null) lifeBar.fillAmount = charStats.LifePercentage();
        return false;
    }

    public override void Interact(Player player)
    {
        CheckQuest();
        Debug.Log("Não interage");
    }

    public void SpawnMyNPC()
    {
        if (npcToSpawnPref == null) { this.gameObject.SetActive(false); return; }

        Instantiate(npcToSpawnPref, transform.position, transform.rotation);

        this.gameObject.SetActive(false);
    }
}
