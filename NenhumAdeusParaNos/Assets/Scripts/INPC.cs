using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class INPC : NPC/*Interactives*//*, BattleUnit*/
{
    [SerializeField] string npcName = "Name";
    [SerializeField] string areaName = "Area Name";
    public string Name { get { return npcName; } set { npcName = value; } }
    public string AreaName { get { return areaName; } set { areaName = value; } }
    public Sprite[] expressions = new Sprite[3];

    public enum Personalities { Tsundere, Yandere }
    public enum EnemyType { Lustro, Citzen, Capitalist, Communist, None }
    //public enum Faction { Communist, Non_Communist, None }

    //[HideInInspector] CharacterStats charStats;
    //public CharacterStats CharStats { get { return charStats; } }

    //public Animator anim;
    //public Image lifeBar;

    //[SerializeField] Behavior behavior;
    public Personalities thisPersonality;// { get { return behavior; } set { behavior = value; } }
    public EnemyType enemyType;
    //public Faction faction;
    public bool hasOtherNPCTalk;
    public INPC theOtherNPC;

    //Personality personality;
    //NavMeshAgent navMesh;

    [SerializeField] bool waitingForAnswer = false;

    //[SerializeField] bool myQuestAccepeted = false;
    //public bool MyQuestAccepted { set { myQuestAccepeted = value; } }
    public DialogueWithChoice[] questDialogues;
    public Dialogue dialogueWithOtherNPC;
    public DialogueOptions[] myDialogues = new DialogueOptions[3];   
    //public Dialogue[] answerDialogues = new Dialogue[3];
    List<DialogueBattle.ApproachType> receivedAp = new List<DialogueBattle.ApproachType>();
    //Dialogue initialDialogue;

    public bool hostile = false;
    public bool heavy = false;

    //bool attacking = false;
    //public Weapon myWeapon;
    //RangedW rangedW;
    //bool strongAtk = false;
    //bool isRanged = false;

    bool stunned = false;
    //bool moveSpeedChanged = false;

    //public float defaultSpeed = 6.0f;
    //public float rangedKiteSpeed = 4.0f;
    public float dashTime = 0.75f;
    public float dashDistance = 7.5f;
    public float chargeTime = 1.5f;
    public float damageImmuneTime = 5.0f;
    public float dashDamage = 25.0f;
    //float dashDamage = 0.0f;
    bool charging = false;
    bool dashing;// { get; private set; }
    bool damageImmune = false;
    int atkCounter = 0;
    public GameObject granadeToThrow;
    public Transform granadeInstPoint;

    //[HideInInspector] Player mCharacter;
    //public Player MCharacter { get { return mCharacter; } set { mCharacter = value; } }

    //bool interacting = false;

    //bool inBattle = false;

    float attackModifier = 1.0f;
    //float atkInterval = 0.15f;
    //float timer = 0.0f;

    //bool firstInteraction = false;

    Vector3 startPos;
    //LayerMask layermask = 1 << 10;

    #region Audio Clips
    public AudioClip clip_death;
    #endregion

    //private void Start()
    //{
    //    personality = new Personality();
    //    SetPersonalityPercentages();

    //    charStats = new CharacterStats(this);
    //    navMesh = GetComponent<NavMeshAgent>();

    //    if (anim == null) anim = GetComponentInChildren<Animator>();

    //    if (myWeapon == null) myWeapon = GetComponentInChildren<Weapon>();
    //    if (myWeapon is RangedW)
    //    {
    //        isRanged = true;
    //        rangedW = (RangedW)myWeapon;
    //    }

    //    navMesh.speed = defaultSpeed;
    ////}
    //protected override void Initialize()
    //{
    //    //personality = new Personality();
    //    //SetPersonalityPercentages();
    //}
    //void SetPersonalityPercentages()
    //{
    //    for (int i = 0; i < 5; i++)
    //    {
    //        personality.SetPercentage(i, GameManager.gameManager.personalities[(int)thisPersonality][i]);
    //    }
    //}

    //public override void Interact(Player player)
    //{
    //    //mCharacter = player;
    //    //myDialogue.MyNPC = this;
    //    //myDialogue.MainCharacter = player;
    //    //DesactiveBtp();
    //    ////GameManager.gameManager.dialogueController.OpenDialoguePopUp(this.transform, this);
    //    //GameManager.gameManager.dialogueController.StartDialogue(myDialogue, transform/*, this*/);
    //    //interacting = true;
    //    ////NextString();
    //}

    public void NextString()
    {
        //GameManager.gameManager.dialogueController.UpdateText(myDialogue.NextString());
        GameManager.gameManager.dialogueController.NextString();
    }

    public void EndDialogue()
    {
        //GameManager.gameManager.dialogueController.CloseDialoguePopUp();
        GameManager.gameManager.dialogueController.EndDialogue();
        //myDialogue.ResetDialogue();
        //myDialogue = initialDialogue;
        //interacting = false;
        //firstInteraction = false;
    }

    //public override void OnExit()
    //{
    //    base.OnExit();
    //    //EndDialogue();
    //}

    //
    //public void ChangeDialogue(Dialogue newDialogue)
    //{
    //    myDialogue.ResetDialogue();
    //    myDialogue = newDialogue;
    //}
    //

    //private void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.E) && interacting && !firstInteraction) firstInteraction = true;
    //    //else if (Input.GetKeyDown(KeyCode.E) && interacting && firstInteraction) NextString();
    //    Movement();
    //    //if (inBattle && Input.GetKeyDown(KeyCode.L)) Stun(2.0f);
    //    //if (inBattle && Input.GetKeyDown(KeyCode.H)) ChangeMoveSpeed(120.0f, 2.0f);
    //}

    protected override void Movement()
    {
        float actualVelocity = navMesh.velocity.magnitude / navMesh.speed;
        if (anim != null) anim.SetFloat("Vel", actualVelocity);
        if (inBattle && !stunned && !charging)
        {
            //Ray ray = new Ray(transform.position, mCharacter.transform.position - transform.position);
            //RaycastHit hit;

            //Vector3 lookPos = new Vector3(mCharacter.transform.position.x, transform.position.y, mCharacter.transform.position.z);
            //transform.LookAt(lookPos);


            //if (isRanged)
            //{
            //    if ((mCharacter.transform.position - transform.position).sqrMagnitude <= myWeapon.GetRange() * myWeapon.GetRange())
            //    {
            //        Vector3 desiredPos = -(mCharacter.transform.position - transform.position) + transform.position;
            //        MoveNavMesh(desiredPos);
            //        if (!moveSpeedChanged) navMesh.speed = rangedKiteSpeed;
            //    }
            //    else if ((mCharacter.transform.position - transform.position).sqrMagnitude >= rangedW.GetMaxRange() * rangedW.GetMaxRange())
            //    {
            //        Vector3 toPlayerVec = mCharacter.transform.position - transform.position;
            //        Vector3 desiredPos = toPlayerVec.normalized * (toPlayerVec.magnitude - rangedW.GetMaxRange() * 0.2f) + transform.position;
            //        if (!moveSpeedChanged) navMesh.speed = defaultSpeed;
            //        MoveNavMesh(desiredPos);
            //    }
            //    else
            //    {
            //        navMesh.isStopped = true;
            //        //parar animação de andar
            //    }

            //    if (Physics.Raycast(ray, out hit))
            //    {
            //        //Debug.Log(hit.collider.name);
            //        if (hit.collider.CompareTag("player") || hit.collider.CompareTag("barrier") || /*Vector3.Distance*/(hit.transform.position - transform.position).sqrMagnitude >= /*Vector3.Distance*/(mCharacter.transform.position - transform.position).sqrMagnitude)
            //        {
            //            TryAttack();
            //        }
            //    }
            //    else TryAttack();
            //}
            //else
            //{
            //    Vector3 toPlayerVec = mCharacter.transform.position - transform.position;
            //    Vector3 desiredPos = toPlayerVec.normalized * (toPlayerVec.magnitude - myWeapon.GetRange() * 0.6f) + transform.position;
            //    MoveNavMesh(desiredPos);
            //    if ((mCharacter.transform.position - transform.position).sqrMagnitude <= myWeapon.GetRange() * myWeapon.GetRange())
            //    {
            //        TryAttack();
            //    }
            //    else if (Physics.Raycast(ray, out hit))
            //    {
            //        if (hit.collider.CompareTag("barrier") && (hit.transform.position - transform.position).sqrMagnitude <= myWeapon.GetRange() * myWeapon.GetRange())
            //        {
            //            TryAttack();
            //        }
            //    }
            //}
            InBattleBehaviour();
        }
        else if (charging)
        {
            Vector3 lookPos = new Vector3(mCharacter.transform.position.x, transform.position.y, mCharacter.transform.position.z);
            transform.LookAt(lookPos);
        }
    }

    //void MoveNavMesh(Vector3 pos)
    //{

    //    navMesh.isStopped = false;
    //    navMesh.destination = pos;

    //    //if (inBattle)
    //    //{
    //    //??? if (isRanged) //animação de andar para trás;
    //    //else animação de andar pra frente;

    //    //CheckLook_WalkDir(navMesh.steeringTarget.normalized);
    //    //}
    //    //else
    //    //{
    //    //animção de andar pra frente
    //    //}
    //}

    //void CheckLook_WalkDir(Vector3 moveDir)
    //{
    //    float auxDot = Vector3.Dot(transform.forward, moveDir);
    //    if (auxDot > 0.0f)
    //    {
    //        Debug.Log("Andando e olhando para mesma direção");
    //    }
    //    //else if (auxDot > -0.5f)
    //    //{
    //    //    float auxDotRight = Vector3.Dot(transform.right, moveDir);
    //    //    if (auxDotRight > 0)
    //    //    {
    //    //        //Andando pra direita e olhando pre frente
    //    //    }
    //    //    else
    //    //    {
    //    //        //Andando pra esquerda e olhando pre frente
    //    //    }
    //    //}
    //    else
    //    {
    //        Debug.Log("Andando pra trás e olhando pra frente");
    //    }
    //}

    //void TryAttack()
    //{
    //    //if (isRanged)
    //    //{
    //    timer += Time.deltaTime;
    //    if (timer >= atkInterval)
    //    {
    //        bool canAtk = true;
    //        if (isRanged && rangedW.HasAmmo())
    //        {
    //            canAtk = true;
    //        }
    //        else if (isRanged)
    //        {
    //            //animação de reload
    //            canAtk = false;
    //        }
    //        int randomType = Random.Range(0, 2);
    //        if (randomType == 1) strongAtk = false;
    //        else strongAtk = true;
    //        if (canAtk) Attack();
    //        timer = 0.0f;
    //    }
    //    //}
    //}

    //void Attack()
    //{
    //    if (!attacking)
    //    {
    //        if (isRanged) anim.SetInteger("AtkType", 1);
    //        //Debug.Log("Atacando");
    //        attacking = true;
    //        //float attackCD;
    //        if (strongAtk && myWeapon is MeleeW)
    //        {
    //            MeleeW myMelee = (MeleeW)myWeapon;
    //            myMelee.SetStrongAttack();
    //        }
    //        if (!isRanged)
    //        {
    //            //attackCD = myWeapon.Attack(null, attackModifier);
    //            //Invoke("AttackCooldown", attackCD);
    //            ComfirmAttack();
    //        }
    //        else
    //        {
    //            Invoke("DelayedAttack", rangedW.GetDelayToShoot());
    //        }
    //    }
    //}
    //void DelayedAttack()
    //{
    //    //float cd = myWeapon.Attack(null, attackModifier);
    //    //Invoke("AttackCooldown", cd);
    //    ComfirmAttack();
    //}
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

    //void AttackCooldown()
    //{
    //    attacking = false;
    //    if (isRanged) anim.SetInteger("AtkType", 0);
    //}

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
                    //Vector3 desiredPos = (mCharacter.transform.position - transform.position).normalized * dashDistance + transform.position;
                    //StartCoroutine(Dash(desiredPos, true , true, dashDamage));
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
        //dashDamage = damage;
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
        //anim.SetBool("Dashing", true);
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
    //Vector3 from = Vector3.zero;
    //Vector3 to = Vector3.zero;
    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(from, to);
    //}

    public void SetWaitingForAnswer()
    {
        waitingForAnswer = true;
        StopCoroutine("CancelDialogue");
        StartCoroutine("CancelDialogue");
        //GameManager.gameManager.dialogueController.WaitForAnswerTimer(10)
    }
    IEnumerator CancelDialogue()
    {
        yield return new WaitForSeconds(12);
        if (waitingForAnswer)
        {
            waitingForAnswer = false;
            Debug.Log("Cancelando resposta");
            GameManager.gameManager.dialogueController.EndDialogue();
        }
    }
    public void CancelBattleDialogue()
    {
        Debug.Log("Limpando ultimos approaches");
        receivedAp.Clear();
    }
    public void ReceiveBattleDialogue(DialogueBattle playerDialogue)
    {
        Debug.Log("Dialogo Recebido");

        if (!inBattle && waitingForAnswer)
        {
            //Debug.Log("ALOALOALO");
            GameManager.gameManager.dialogueController.ChooseOption((int)playerDialogue.approachType, expressions[1]);
            waitingForAnswer = false;
        }
        else if (inBattle)
        {
            StopCoroutine("CancelDialogue");
            //float answerChance = personality.CalculatePercentage(playerDialogue.approachType);
            float aux = Random.Range(0, 100);

            receivedAp.Add(playerDialogue.approachType);
            //if (receivedAp.Count == 1)
            //{
            //    GameManager.gameManager.dialogueController.StartPlayerAnswerCountown(this);
            //}

            if (receivedAp.Count < 3)
            {
                if (receivedAp.Count == 1)
                {
                    GameManager.gameManager.dialogueController.StartPlayerAnswerCountown(this);
                    Debug.Log("Startando Dialogo Em Combate");
                }
                else
                {
                    GameManager.gameManager.dialogueController.ContinueBattleDialogue();
                }
                Debug.Log("Respondendo dialogo");
                //int random = Random.Range(0, 2);
                //Dialogue answer = GameManager.gameManager.npcAnswers.GetFailAnswer(thisPersonality, playerDialogue.approachType, random);
                Dialogue answer = GameManager.gameManager.dialogueController.GetAnswer((int)enemyType, (int)thisPersonality, (int)playerDialogue.approachType, receivedAp.Count - 1);
                answer.MyNPC = this;
                StartCoroutine(DelayStartDialogue(answer, (int)playerDialogue.approachType));
            }
            else if (receivedAp.Count == 3)
            {
                //Trigar o dialogo especifico
                Debug.Log("Trigando o dialogo especial");                
                Dialogue auxDialogue = GameManager.gameManager.dialogueController.GetBattleResult((int)enemyType, (int)thisPersonality, receivedAp.ToArray(), this, mCharacter);
                GameManager.gameManager.dialogueController.CancelBattleDialogue();
                auxDialogue.MyNPC = this;
                auxDialogue.MainCharacter = mCharacter;
                //if (auxDialogue is DialogueBattleResult)
                //{
                //    DialogueBattleResult auxBR = (DialogueBattleResult)auxDialogue;
                //    auxBR.StartEffects();
                //}
                StartCoroutine(DelayStartDialogue(auxDialogue, (int)playerDialogue.approachType));                
            }

            //if (aux <= answerChance)
            //{
            //    if (answerDialogues[(int)playerDialogue.approachType] != null)
            //    {
            //        answerDialogues[(int)playerDialogue.approachType].MyNPC = this;
            //        answerDialogues[(int)playerDialogue.approachType].MainCharacter = mCharacter;
            //        //GameManager.gameManager.dialogueController.StartDialogue(answerDialogues[(int)playerDialogue.approachType], transform);
            //        StartCoroutine(DelayStartDialogueBattle(playerDialogue));
            //    }

            //    Debug.Log("Respondendo diálogo");
            //}
            //if (receivedAp.Count < 3)
            //{
            //    Debug.Log("Dialogo Falhou");
            //    int random = Random.Range(0, 2);
            //    //Dialogue answer = GameManager.gameManager.npcAnswers.GetFailAnswer(thisPersonality, playerDialogue.approachType, random);
            //    Dialogue answer = GameManager.gameManager.dialogueController.GetAnswer((int)enemyType, (int)thisPersonality, (int)playerDialogue.approachType, random);
            //    answer.MyNPC = this;
            //    StartCoroutine(DelayStartDialogue(answer));
            //}
        }
        else
        {
            if (questDialogues.Length > 0)// && !myQuestAccepeted)
            {
                for (int i = 0; i < questDialogues.Length; i++)
                {
                    DialogueQuestTrigger dqt = questDialogues[i].SearchQuestDialogue();
                    if (dqt.quest.WaitingReturnToNPC && !dqt.quest.Completed)
                    {
                        dqt.quest.completingQuestDialogue.MainCharacter = GameManager.gameManager.battleController.MainCharacter;
                        dqt.quest.completingQuestDialogue.MyNPC = this;
                        StartCoroutine(DelayStartDialogue(dqt.quest.completingQuestDialogue));
                        return;
                    }
                }
            }           
            //Dialogue aux = myDialogues[(int)playerDialogue.approachType].GetRandomDialogue();
            //int auxID = 0;

            //switch ((int)playerDialogue.approachType)
            //{
            //    case 0:
            //        auxID = (myDialogues[0] != null) ? 0 : 1;
            //        break;
            //    case 1:
            //        auxID = (myDialogues[1] != null) ? 1 : 0;
            //        break;
            //    case 2:
            //        auxID = (myDialogues[2] != null) ? 2 : 3;
            //        break;
            //    case 3:
            //        auxID = (myDialogues[3] != null) ? 3 : 2;
            //        break;
            //    default:
            //        break;
            //}
            //aux = myDialogues[auxID].GetRandomDialogue();
            //if (aux.MainCharacter == null) aux.MainCharacter = GameManager.gameManager.battleController.MainCharacter;
            //aux.MyNPC = this;
            //StartCoroutine(DelayStartDialogue(aux));

            Dialogue aux = null;
            try
            {
                aux = myDialogues[(int)playerDialogue.approachType].GetRandomDialogue();
            }
            catch
            {
                Debug.Log("Pegando primeira resposta disponível");
                for (int i = 0; i < myDialogues.Length; i++)
                {
                    if (myDialogues[i] != null)
                    {
                        aux = myDialogues[i].GetRandomDialogue();
                        break;
                    }
                }
            }
            if (aux != null)
            {
                if (aux.MainCharacter == null) aux.MainCharacter = GameManager.gameManager.battleController.MainCharacter;
                aux.MyNPC = this;
                StartCoroutine(DelayStartDialogue(aux));
            }
        }
    }
    IEnumerator DelayStartDialogueBattle(DialogueBattle playerDialogue)
    {
        yield return new WaitForEndOfFrame();
        //GameManager.gameManager.dialogueController.StartDialogue(answerDialogues[(int)playerDialogue.approachType], transform);
        //Dialogue answer = GameManager.gameManager.dialogueController.GetAnswer((int)enemyType, (int)thisPersonality, (int)playerDialogue.approachType, 2);
        //GameManager.gameManager.dialogueController.StartDialogue(answer, transform);
    }
    IEnumerator DelayStartDialogue(Dialogue failDialogue, int ex = 1)
    {
        yield return new WaitForEndOfFrame();
        GameManager.gameManager.dialogueController.StartDialogue(failDialogue, transform, expressions[ex]);
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
            startPos = transform.position;
            //OnExit();
            Debug.Log("Bydialogue: " + byDialogue);
            Debug.Log("Hostil: " + hostile);
            if ((!byDialogue && hostile) || byDialogue)
            {                
                try
                {
                    GetComponent<SphereCollider>().enabled = false;
                }
                catch { }
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

    //public override bool CanFight()
    //{
    //    return charStats.CanFight;
    //}

    //public override bool IsInBattle()
    //{
    //    return inBattle;
    //}

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
    //public override Vector3 GetPos()
    //{
    //    return transform.position;
    //}

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
        if (other.tag.Equals("player") && !GameManager.gameManager.battleController.ActiveBattle)
        {
            //Se tem quest, inicia uma conversa com o player, sobre a quest
            if (questDialogues.Length > 0)// && !myQuestAccepeted)
            {
                Debug.Log("Olhando quests");
                //Não abrir quando quest já completa
                for (int i = 0; i < questDialogues.Length; i++)
                {                    
                    DialogueQuestTrigger dqt = questDialogues[i].SearchQuestDialogue();
                    Debug.Log(dqt.quest.Accepted);
                    Debug.Log(dqt.quest.Completed);
                    if (!dqt.quest.Accepted)
                    {
                        questDialogues[i].MyNPC = this;
                        questDialogues[i].MainCharacter = GameManager.gameManager.battleController.MainCharacter;
                        GameManager.gameManager.dialogueController.StartDialogue(questDialogues[i], transform, expressions[1]);
                        //BeginQuestDialogue(questDialogues[i]);
                    }
                }

                //questDialogue.MyNPC = this;
                //questDialogue.MainCharacter = GameManager.gameManager.battleController.MainCharacter;
                //GameManager.gameManager.dialogueController.StartDialogue(questDialogue, transform);
            }
            else if (hasOtherNPCTalk)
            {
                //GameManager.gameManager.dialogueController.StartDialogue(dialogueWithOtherNPC, transform, true);
            }
        }
    }

    void ActiveInteractionCollider()
    {
        try
        {
            GetComponent<SphereCollider>().enabled = true;
        }
        catch { }     
    }
}
