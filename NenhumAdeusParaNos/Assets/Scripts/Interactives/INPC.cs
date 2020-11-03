using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class INPC : NPC
{
    [Header("INPC")]
    //[SerializeField] string npcName = "Name";
    [SerializeField] string areaName = "Area Name";
    //public string Name { get { return npcName; } set { npcName = value; } }
    public string AreaName { get { return areaName; } set { areaName = value; } }
    //public Sprite[] expressions = new Sprite[3];
    //public Sprite portrait;

    public enum Personalities { Tsundere, Yandere }
    public enum EnemyType { Lustro, Citzen, Capitalist, Communist, None }

    public Personalities thisPersonality;
    public EnemyType enemyType;
    //public bool hasOtherNPCTalk;
    //public INPC theOtherNPC;
    [Header("----------- Diálogos e Quests -----------")]
    public DialogueOptions dialogues;
    public DialogueOptions dialogues_battle;

    public DialogueWithChoice[] questDialogues;
    public DialogueQuestTrigger[] directQuestDialogue;
    //public Dialogue dialogueWithOtherNPC;
    //public DialogueOptions[] myDialogues = new DialogueOptions[3];
    public bool toBeAccepted = true;

    public Quest questAcceptedToDespawn;
    public Quest questCompletedToDespawn;

    public bool rotates = true;

    [Header("--------------- Comabte ---------------")]
    public bool hostile = false;
    public bool heavy = false;

    bool stunned = false;

    public float dashTime = 0.75f;
    public float dashDistance = 7.5f;
    public float chargeTime = 1.5f;
    public float damageImmuneTime = 5.0f;
    public float dashDamage = 25.0f;
    bool charging = false;
    bool dashing;// { get; private set; }
    bool damageImmune = false;
    int atkCounter = 0;
    public GameObject granadeToThrow;
    public Transform granadeInstPoint;

    float attackModifier = 1.0f;

    Vector3 startPos;

    Quaternion originRot;

    #region Audio Clips
    public AudioClip clip_death;
    #endregion

    protected override void Initialize()
    {
        if (CheckDespawnQuest()) Destroy(this.gameObject);

        originRot = transform.rotation;

        CustomEvents.instance.onDialogueStart += EnterDialogueAnim;
        CustomEvents.instance.onDialogueEnd += ExitDialogueAnim;

        //Acionar evento
        //firstEnable = false;
    }

    private void OnDestroy()
    {
        CustomEvents.instance.onDialogueStart -= EnterDialogueAnim;
        CustomEvents.instance.onDialogueEnd -= ExitDialogueAnim;
    }

    public override void CheckForQuestObjectives(Quest q_)
    {
        base.CheckForQuestObjectives(q_);

        //Debug.Log("Checando quest nos npcs");

        if (q_ is DialogueQuest)
        {
            DialogueQuest q = (DialogueQuest)q_;
            for (int i = 0; i < q.npcsToTalk.Length; i++)
            {
                if (q.npcsToTalk[i].Equals(Name) && !q.talked[i])
                {
                    SpawnQuestMarker();
                    active_quest = q;
                    return;
                }
            }
        }

        if (q_ is AssassinQuest)
        {
            AssassinQuest q = (AssassinQuest)q_;
            if (q.TargetName.Equals(Name))
            {
                SpawnQuestMarker();
                active_quest = q;
                return;
            }
        }

        //if (q_ is KillQuest)
        //{
        //    KillQuest q = (KillQuest)q_;
        //    if (q.enemyType == enemyType)
        //    {
        //        SpawnQuestMarker();
        //        active_quest = q;
        //        return;
        //    }
        //}
    }

    bool CheckDespawnQuest()
    {
        if (questAcceptedToDespawn != null && questCompletedToDespawn != null)
        {

            if (toBeAccepted)
            {
                if (questAcceptedToDespawn.Accepted && !questCompletedToDespawn.Completed) return true;
                else return false;
            }
            else
            {
                if (!questAcceptedToDespawn.Accepted || questCompletedToDespawn.Completed) return true;
                else return false;
            }


        }

        if (questAcceptedToDespawn != null)
        {
            if ((questAcceptedToDespawn.Accepted && toBeAccepted) || 
                (!questAcceptedToDespawn.Accepted && !toBeAccepted))
                return true;
        }
        if (questCompletedToDespawn != null)
        {
            if ((questCompletedToDespawn.Completed && !toBeAccepted) ||
                (!questCompletedToDespawn.Completed && toBeAccepted))
                return true;
        }

        return false;
    }

    public override void Interact(Player player)
    {
        Debug.Log("Interagindo");
        mCharacter = player;
        DesactiveBtp();
        if (GameManager.gameManager.dialogueController.ActiveMainDialogue) return;
        
        if (!inBattle && (questDialogues.Length > 0 || directQuestDialogue.Length > 0))// && !myQuestAccepeted)
        {
            for (int i = 0; i < questDialogues.Length; i++)
            {
                DialogueQuestTrigger dqt = questDialogues[i].SearchQuestDialogue();
                if (dqt != null && dqt.quest.WaitingReturnToNPC && !dqt.quest.Completed)
                {
                    //dqt.quest.completingQuestDialogue.MainCharacter = GameManager.gameManager.battleController.MainCharacter;
                    //dqt.quest.completingQuestDialogue.MyNPC = this;
                    ////StartCoroutine(DelayStartDialogue(dqt.quest.completingQuestDialogue));
                    //GameManager.gameManager.dialogueController.StartDialogue(dqt.quest.completingQuestDialogue, this.transform, expressions[1]);
                    StartDialogue(dqt.quest.completingQuestDialogue, player);
                    return;
                }
            }
            for (int i = 0; i < directQuestDialogue.Length; i++)
            {
                if (directQuestDialogue[i].quest.WaitingReturnToNPC && !directQuestDialogue[i].quest.Completed)
                {
                    //dqt.quest.completingQuestDialogue.MainCharacter = GameManager.gameManager.battleController.MainCharacter;
                    //dqt.quest.completingQuestDialogue.MyNPC = this;
                    ////StartCoroutine(DelayStartDialogue(dqt.quest.completingQuestDialogue));
                    //GameManager.gameManager.dialogueController.StartDialogue(dqt.quest.completingQuestDialogue, this.transform, expressions[1]);
                    StartDialogue(directQuestDialogue[i].quest.completingQuestDialogue, player);
                    return;
                }
            }
            DefaultDialogue(player);
        }
        else if (dialogues != null && dialogues.dialogueOp.Length > 0)
        {
            DefaultDialogue(player);
            //OnExit(player);
            //Dialogue dialogue;
            //if (inBattle)
            //{
            //    if (thisPersonality == Personalities.Tsundere && charStats.LifePercentage() > 0.5f) return;
            //    else if (thisPersonality == Personalities.Yandere && (charStats.LifePercentage() < 0.3f || charStats.LifePercentage() > 0.7f)) return;
            //    GameManager.gameManager.battleController.DisableNPCInteractions();
            //    GameManager.gameManager.MainHud.OpenDialogueTab(expressions[1]);
            //    dialogue = dialogues_battle.GetRandomDialogue();
            //}
            //else dialogue = dialogues.GetRandomDialogue();
            //dialogue.MyNPC = this;
            //dialogue.MainCharacter = player;
            //GameManager.gameManager.dialogueController.StartDialogue(dialogue, transform, expressions[1]);
        }
        CheckQuest();
    }
    void DefaultDialogue(Player player)
    {
        Dialogue dialogue;
        if (inBattle)
        {
            if (thisPersonality == Personalities.Tsundere && charStats.LifePercentage() > 0.5f) return;
            else if (thisPersonality == Personalities.Yandere && (charStats.LifePercentage() < 0.3f || charStats.LifePercentage() > 0.7f)) return;
            GameManager.gameManager.battleController.DisableNPCInteractions();
            GameManager.gameManager.MainHud.OpenDialogueTab(/*expressions[1]*/portrait);
            dialogue = dialogues_battle.GetRandomDialogue();
        }
        else dialogue = dialogues.GetRandomDialogue();
        //dialogue.MyNPC = this;
        //dialogue.MainCharacter = player;
        //GameManager.gameManager.dialogueController.StartDialogue(dialogue, transform, expressions[1]);
        StartDialogue(dialogue, player);

        if (rotates) StartCoroutine(RotateTo(Quaternion.LookRotation(player.transform.position - transform.position)));
    }

    void StartDialogue(Dialogue d, Player p)
    {
        CheckQuestMarker();
        d.MyNPC = this;
        d.MainCharacter = p;
        GameManager.gameManager.dialogueController.StartDialogue(d, transform, /*expressions[1]*/portrait);
    }

    IEnumerator RotateTo(Quaternion newRot)
    {
        float timer = 0.0f;
        while (timer <= 1.0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.fixedDeltaTime * 5.0f);

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Finished Rotate");
    }

    //public void EndDialogue()
    //{
    //    GameManager.gameManager.dialogueController.EndDialogue();
    //}

    protected override void Movement()
    {
        float actualVelocity = navMesh.velocity.magnitude / navMesh.speed;
        if (actualVelocity < 0.01f) return;

        if (anim != null) anim.SetFloat("Vel", actualVelocity);
        if (inBattle && !stunned && !charging)
        {
            InBattleBehaviour();
        }
        else if (charging)
        {
            Vector3 lookPos = new Vector3(mCharacter.transform.position.x, transform.position.y, mCharacter.transform.position.z);
            transform.LookAt(lookPos);
        }
    }

    public override void EndDialogue()
    {
        base.EndDialogue();
        if (rotates) StartCoroutine(RotateTo(originRot));
    }

    protected override void ComfirmAttack()
    {
        float cd = 0.1f;
        if (enemyType == EnemyType.Lustro && !rangedW && strongAtk)
        {
            MeleeW myMelee = (MeleeW)myWeapon;
            myMelee.SetNormalAttack();
        } 
        atkCounter += 1;
        if (enemyType == EnemyType.Lustro)
        {
            if (atkCounter == 3) SpecialAttack();
            else
            {
                cd = myWeapon.Attack(null, attackModifier);
            }
        }
        else cd = myWeapon.Attack(null, attackModifier);
        Invoke("AttackCooldown", cd);
    }

    void SpecialAttack()
    {
        if (enemyType == EnemyType.Lustro)
        {
            if (isRanged)
            {
                Debug.Log("Arremessar Granada");
                //Arremessar granada na direção do player
                StartCoroutine("ThrowGranade");
            }
            else
            {
                if (!heavy)
                {
                    Debug.Log("Dash");
                    CallDamagingDash(dashDamage, true);
                }
                else StartCoroutine("DamageImmune");
            }
        }
        atkCounter = 0;
    }

    IEnumerator ThrowGranade()
    {
        if (anim != null)
        {
            anim.SetInteger("AtkType", 3);
            Invoke("ResetCharge", chargeTime + 0.5f);
        }
        charging = true;
        yield return new WaitForSeconds(chargeTime);
        if (granadeInstPoint == null) granadeInstPoint = transform;
        Vector3 dir = mCharacter.transform.position;// - transform.position) * 1.5f + Vector3.up * 10;
        GameObject auxGo = Instantiate(granadeToThrow, granadeInstPoint.position, granadeInstPoint.rotation) as GameObject;
        Granade aux = auxGo.GetComponent<Granade>();
        aux.onPlayer = true;
        aux.ApplyForce(dir);
    }
    IEnumerator DamageImmune()
    {
        charging = true;
        yield return new WaitForSeconds(chargeTime);
        charging = false;
        damageImmune = true;
        yield return new WaitForSeconds(damageImmuneTime);
        damageImmune = false;
    }
    void ResetCharge()
    {
        charging = false;
        anim.SetInteger("AtkType", 0);
    }

    public void CallDamagingDash(float dmg, bool charge = false)
    {
        Vector3 desiredPos = (mCharacter.transform.position - transform.position).normalized * dashDistance + transform.position;
        StartCoroutine(Dash(desiredPos, charge, true, dmg));
    }
    public IEnumerator Dash(Vector3 dir, bool charge = false, bool doDamage = false, float damage = 0.0f)
    {
        if (charge)
        {
            Stun(dashTime + chargeTime);
            yield return new WaitForSeconds(chargeTime); //Charging dash;
        }

        dashing = true;
        Vector3 originPos = transform.position;
        float distance = (dir - transform.position).magnitude;

        if (doDamage)
        {
            RaycastHit[] hits = hits = Physics.RaycastAll(transform.up * 1.5f + transform.position, dir - (transform.up * 1.5f + transform.position), distance + 0.25f);
            Vector3 from = transform.up * 1.5f + transform.position;
            Vector3 to = dir;
            foreach (var hit in hits)
            {
                Debug.Log(hit.collider.name);
                try
                {
                    if (!hit.collider.gameObject.layer.Equals(this.gameObject.layer))
                    {
                        hit.collider.GetComponent<BattleUnit>().ReceiveDamage(damage);
                    }
                }
                catch { }
            }
        }
        float timer = 0.0f;
        do
        {
            timer += Time.deltaTime / dashTime * dashDistance / distance;
            transform.position = Vector3.Lerp(originPos, dir, timer);
            yield return new WaitForEndOfFrame();
        } while (timer < 1 && dashing);
        dashing = false;
        //anim.SetBool("Dashing", false);
    }

    void Flee()
    {
        mCharacter = GameManager.gameManager.battleController.MainCharacter;
        StartCoroutine("FleeCoroutine");
    }
    IEnumerator FleeCoroutine()
    {
        bool notUnreachable = true;
        navMesh.speed *= 3;
        while (notUnreachable)
        {
            Vector3 desiredPos = -((mCharacter.transform.position - transform.position).normalized) + transform.position;
            MoveNavMesh(desiredPos);
            notUnreachable = navMesh.CalculatePath(desiredPos, navMesh.path);
            if ((mCharacter.transform.position - transform.position).sqrMagnitude >= 40 * 40) Destroy(this.gameObject);// Ou só desativa temporariamente
            yield return new WaitForEndOfFrame();
        }
        navMesh.speed /= 3;
        navMesh.isStopped = true;
    }

    public void Stun(float time)
    {
        stunned = true;
        Invoke("CancelStun", time);
        navMesh.isStopped = true;
    }
    void CancelStun()
    {
        stunned = false;
    }
    public void ChangeMoveSpeed(float moveSpeedPerc, float time)//+100% aumenta, - de 100% diminui
    {
        moveSpeedChanged = true;
        navMesh.speed = navMesh.speed / 100 * moveSpeedPerc;
        Invoke("CancelMoveSpeedChange", time);
    }
    void CancelMoveSpeedChange()
    {
        moveSpeedChanged = false;
        navMesh.speed = defaultSpeed;
    }
    public void ChangeAttackMod(float mod)
    {
        attackModifier = mod;
    }

    public override void StartBattle(bool byDialogue = true)
    {
        if (!inBattle && CanFight())
        {
            DesactiveBtp();
            startPos = transform.position;
            Debug.Log("Bydialogue: " + byDialogue);
            Debug.Log("Hostil: " + hostile);
            if ((!byDialogue && hostile) || byDialogue)
            {                
                //try
                //{
                //    GetComponent<SphereCollider>().enabled = false;
                //}
                //catch { }
                GameManager.gameManager.battleController.AddFighter(this);
                GameManager.gameManager.dialogueController.EndDialogue();
                inBattle = true;
                Debug.Log("NPC entrou na batalha");
                lifeBar.transform.parent.gameObject.SetActive(true);
                StartCoroutine("SetTarget");
            }
            else
            {
                Invoke("Flee", 0.2f);
            }
        }
    }
    IEnumerator SetTarget()
    {
        yield return new WaitForEndOfFrame();
        inBattleTarget = mCharacter.transform;
    }

    public override void EndBattle()
    {
        if (CanFight()) MoveNavMesh(startPos);
        inBattle = false;
        navMesh.speed = defaultSpeed;
        Invoke("ActiveInteractionCollider", 3.0f);
        lifeBar.transform.parent.gameObject.SetActive(false);
        attackModifier = 1.0f;
    }
    public void LeaveBattle()
    {
        GameManager.gameManager.battleController.FindAndRemove(this.gameObject.name);
        EndBattle();
    }

    public override bool ReceiveDamage(float damage)
    {
        if (!damageImmune)
        {
            base.ReceiveDamage(damage);
        }
        return false;
    }

    public override void Die()
    {
        GameManager.gameManager.dialogueController.EndDialogue();
        GameManager.gameManager.battleController.FindAndRemove(name);
        EndBattle();
        navMesh.isStopped = true;
        GameManager.gameManager.questController.CheckQuests(this);
        GameManager.gameManager.audioController.PlayEffect(clip_death);
        base.Die();
    }

    public override Transform GetItemSpawnTransf()
    {
        return myWeapon.transform;
    }

    public override void Knockback(float dis)
    {
        if (canReceiveKnockback && inBattle)
        {
            Vector3 desiredPos = -transform.forward * dis + transform.position;
            StartCoroutine(Dash(desiredPos));
            canReceiveKnockback = false;
            Invoke("ResetReceiveKnockback", 0.1f);

        }
    }
    void ResetReceiveKnockback()
    {
        canReceiveKnockback = true;
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.tag.Equals("player"))
        {
            //if (CheckDespawnQuest()) return;

            if (GameManager.gameManager.battleController.ActiveBattle || GameManager.gameManager.dialogueController.ActiveMainDialogue) return;
            //Se tem quest, inicia uma conversa com o player, sobre a quest
            if (directQuestDialogue.Length > 0)
            {
                Debug.Log("Olhando quests diretas");
                //Não abrir quando quest já completa
                for (int i = 0; i < directQuestDialogue.Length; i++)
                {
                    Debug.Log(directQuestDialogue[i].quest.Accepted);
                    Debug.Log(directQuestDialogue[i].quest.Completed);
                    if (!directQuestDialogue[i].quest.Accepted)
                    {
                        //directQuestDialogue[i].MyNPC = this;
                        //directQuestDialogue[i].MainCharacter = GameManager.gameManager.battleController.MainCharacter;
                        //GameManager.gameManager.dialogueController.StartDialogue(directQuestDialogue[i], transform, expressions[1]);
                        StartDialogue(directQuestDialogue[i], GameManager.gameManager.battleController.MainCharacter);
                        return;
                    }        
                    else if (!directQuestDialogue[i].quest.Completed)
                    {
                        DefaultInteraction(other);
                        return;
                    }
                }
            }
            if (questDialogues.Length > 0)// && !myQuestAccepeted)
            {
                Debug.Log("Olhando quests");
                //Não abrir quando quest já completa
                for (int i = 0; i < questDialogues.Length; i++)
                {
                    DialogueQuestTrigger dqt = questDialogues[i].SearchQuestDialogue();
                    //Debug.Log(dqt.quest.Accepted);
                    //Debug.Log(dqt.quest.Completed);
                    if (dqt == null || !dqt.quest.Accepted)
                    {
                        //questDialogues[i].MyNPC = this;
                        //questDialogues[i].MainCharacter = GameManager.gameManager.battleController.MainCharacter;
                        //GameManager.gameManager.dialogueController.StartDialogue(questDialogues[i], transform, expressions[1]);
                        StartDialogue(questDialogues[i], GameManager.gameManager.battleController.MainCharacter);
                        return;
                    }
                }
            }
            DefaultInteraction(other);
            //else if (hasOtherNPCTalk)
            //{
            //    GameManager.gameManager.dialogueController.StartDialogue(dialogueWithOtherNPC, transform, true);
            //}
            //else
            //{
            //    DefaultInteraction(other);
            //}
        }
    }
    void DefaultInteraction(Collider other)
    {
        if (!canInteract) return;

        Player player = other.GetComponent<Player>();
        if (buttonPref == null)
        {
            //buttonPref = Instantiate(buttonToPressPref, transform.position + Vector3.up * popUPHigh, Quaternion.identity);
            buttonPref = Instantiate(buttonToPressPref, GameManager.gameManager.MainHud.popUpsHolder, false) as GameObject;
            buttonPref.GetComponent<ButtonToPress>().SetTransf(transform, popUPHigh);
        }
        else
        {
            buttonPref.GetComponent<ButtonToPress>().SetTransf(transform, popUPHigh);
            buttonPref.SetActive(true);
        }
        player.InteractingObjs.Add(this);
        Debug.Log("Adicionando npc na lista de interagiveis do player");
        player.CanInteract = true;
    }

    void ActiveInteractionCollider()
    {
        try
        {
            GetComponent<SphereCollider>().enabled = true;
        }
        catch { }     
    }

    public void DisableInteractionCollider()
    {
        try
        {
            GetComponent<SphereCollider>().enabled = false;
        }
        catch { }
    }

    public void EnterDialogueAnim(string npc_name)
    {
        if (!Name.Equals(npc_name) || anim == null) return;

        anim.SetBool("Talking", true);
    }

    public void ExitDialogueAnim(string npc_name)
    {
        if (!Name.Equals(npc_name) || anim == null) return;

        anim.SetBool("Talking", false);
    }
}
