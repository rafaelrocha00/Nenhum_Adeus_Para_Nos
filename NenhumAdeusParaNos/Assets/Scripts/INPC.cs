using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class INPC : MonoBehaviour/*Interactives*/, BattleUnit
{
    public enum Personalities { DereDere, Kuudere, Tsundere, Yandere }
    public enum EnemyType { Monstro, Estatal, NaoEstatal, Cidadao }

    [HideInInspector] CharacterStats charStats;
    public CharacterStats CharStats { get { return charStats; } }

    public Animator anim;
    public Image lifeBar;

    //[SerializeField] Behavior behavior;
    public Personalities thisPersonality;// { get { return behavior; } set { behavior = value; } }
    public EnemyType enemyType;
    Personality personality;
    NavMeshAgent navMesh;

    [SerializeField] bool waitingForAnswer = false;

    public DialogueOptions[] myDialogues = new DialogueOptions[4];   
    public Dialogue[] answerDialogues = new Dialogue[4];
    //Dialogue initialDialogue;

    public bool hostile = false;

    bool attacking = false;
    public Weapon myWeapon;
    RangedW rangedW;
    bool strongAtk = false;
    bool isRanged = false;

    bool stunned = false;
    bool moveSpeedChanged = false;

    public float defaultSpeed = 6.0f;
    public float rangedKiteSpeed = 4.0f;

    [HideInInspector] Player mCharacter;
    public Player MCharacter { get { return mCharacter; } set { mCharacter = value; } }

    //bool interacting = false;

    bool inBattle = false;

    float atkInterval = 0.15f;
    float timer = 0.0f;

    //bool firstInteraction = false;

    Vector3 startPos;
    //LayerMask layermask = 1 << 10;

    private void Start()
    {
        personality = new Personality();
        SetPersonalityPercentages();

        charStats = new CharacterStats(this);
        navMesh = GetComponent<NavMeshAgent>();

        if (anim == null) anim = GetComponentInChildren<Animator>();

        if (myWeapon == null) myWeapon = GetComponentInChildren<Weapon>();
        if (myWeapon is RangedW)
        {
            isRanged = true;
            rangedW = (RangedW)myWeapon;
        }

        navMesh.speed = defaultSpeed;
    }
    void SetPersonalityPercentages()
    {
        for (int i = 0; i < 5; i++)
        {
            personality.SetPercentage(i, GameManager.gameManager.personalities[(int)thisPersonality][i]);
        }
    }

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

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.E) && interacting && !firstInteraction) firstInteraction = true;
        //else if (Input.GetKeyDown(KeyCode.E) && interacting && firstInteraction) NextString();

        float actualVelocity = navMesh.velocity.magnitude / navMesh.speed;
        if (isRanged) anim.SetFloat("Vel", actualVelocity);
        if (inBattle && !stunned)
        {             
            Ray ray = new Ray(transform.position, mCharacter.transform.position - transform.position);
            RaycastHit hit;

            Vector3 lookPos = new Vector3(mCharacter.transform.position.x, transform.position.y, mCharacter.transform.position.z);
            transform.LookAt(lookPos);


            if (isRanged)
            {
                if ((mCharacter.transform.position - transform.position).sqrMagnitude <= myWeapon.GetRange() * myWeapon.GetRange())
                {
                    Vector3 desiredPos = -(mCharacter.transform.position - transform.position) + transform.position;
                    MoveNavMesh(desiredPos);
                    if (!moveSpeedChanged) navMesh.speed = rangedKiteSpeed;
                }
                else if ((mCharacter.transform.position - transform.position).sqrMagnitude >= rangedW.GetMaxRange() * rangedW.GetMaxRange())
                {
                    Vector3 toPlayerVec = mCharacter.transform.position - transform.position;
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
                    if (hit.collider.CompareTag("player") || /*Vector3.Distance*/(hit.transform.position - transform.position).sqrMagnitude >= /*Vector3.Distance*/(mCharacter.transform.position - transform.position).sqrMagnitude)
                    {
                        TryAttack();
                    }
                }
                else TryAttack();
            }
            else
            {
                Vector3 toPlayerVec = mCharacter.transform.position - transform.position;
                Vector3 desiredPos = toPlayerVec.normalized * (toPlayerVec.magnitude - myWeapon.GetRange() * 0.6f) + transform.position;
                MoveNavMesh(desiredPos);
                if ((mCharacter.transform.position - transform.position).sqrMagnitude <= myWeapon.GetRange() * myWeapon.GetRange())
                    TryAttack();
            }
        }


        //if (inBattle && Input.GetKeyDown(KeyCode.L)) Stun(2.0f);
        //if (inBattle && Input.GetKeyDown(KeyCode.H)) Slow(40.0f, 2.0f);

    }

    void MoveNavMesh(Vector3 pos)
    {

        navMesh.isStopped = false;
        navMesh.destination = pos;

        //if (inBattle)
        //{
        //??? if (isRanged) //animação de andar para trás;
        //else animação de andar pra frente;

        //CheckLook_WalkDir(navMesh.steeringTarget.normalized);
        //}
        //else
        //{
        //animção de andar pra frente
        //}
    }

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

    void TryAttack()
    {
        //if (isRanged)
        //{
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
            int randomType = Random.Range(0, 2);
            if (randomType == 1) strongAtk = false;
            else strongAtk = true;
            if (canAtk) Attack();
            timer = 0.0f;
        }
        //}
    }

    void Attack()
    {
        if (!attacking)
        {
            if (isRanged) anim.SetInteger("AtkType", 1);
            //Debug.Log("Atacando");
            attacking = true;
            float attackCD;
            if (strongAtk && myWeapon is MeleeW)
            {
                MeleeW myMelee = (MeleeW)myWeapon;
                myMelee.SetStrongAttack();
            }
            if (!isRanged)
            {
                attackCD = myWeapon.Attack();
                Invoke("AttackCooldown", attackCD);
            }
            else
            {
                Invoke("DelayedAttack", rangedW.GetDelayToShoot());
            }
        }
    }
    void DelayedAttack()
    {
        float cd = myWeapon.Attack();
        Invoke("AttackCooldown", cd);
    }

    void AttackCooldown()
    {
        attacking = false;
        if (isRanged) anim.SetInteger("AtkType", 0);
    }

    public void SetWaitingForAnswer()
    {
        waitingForAnswer = true;
        Invoke("CancelDialogue", 10);
        //GameManager.gameManager.dialogueController.WaitForAnswerTimer(10)
    }
    void CancelDialogue()
    {
        if (waitingForAnswer)
        {
            waitingForAnswer = false;
            GameManager.gameManager.dialogueController.EndDialogue();
        }
    }
    public void ReceiveBattleDialogue(DialogueBattle playerDialogue)
    {
        Debug.Log("Dialogo Recebido");
        if (waitingForAnswer)
        {
            //Debug.Log("ALOALOALO");
            GameManager.gameManager.dialogueController.ChooseOption((int)playerDialogue.approachType);
            waitingForAnswer = false;
        }
        else if (inBattle)
        {            
            float answerChance = personality.CalculatePercentage(playerDialogue.approachType);
            float aux = Random.Range(0, 100);

            if (aux <= answerChance)
            {
                if (answerDialogues[(int)playerDialogue.approachType] != null)
                {
                    answerDialogues[(int)playerDialogue.approachType].MyNPC = this;
                    answerDialogues[(int)playerDialogue.approachType].MainCharacter = mCharacter;
                    //GameManager.gameManager.dialogueController.StartDialogue(answerDialogues[(int)playerDialogue.approachType], transform);
                    StartCoroutine(DelayStartDialogueBattle(playerDialogue));
                }

                Debug.Log("Respondendo diálogo");
            }
            else
            {
                Debug.Log("Dialogo Falhou");
                int random = Random.Range(0, 2);
                //Dialogue answer = GameManager.gameManager.npcAnswers.GetFailAnswer(thisPersonality, playerDialogue.approachType, random);
                Dialogue answer = GameManager.gameManager.dialogueController.GetAnswer((int)enemyType, (int)thisPersonality, (int)playerDialogue.approachType, random);
                answer.MyNPC = this;
                StartCoroutine(DelayStartDialogue(answer));
            }
        }
        else
        {
            Dialogue aux;
            int auxID = 0;

            switch ((int)playerDialogue.approachType)
            {
                case 0:
                    auxID = (myDialogues[0] != null) ? 0 : 1;
                    break;
                case 1:
                    auxID = (myDialogues[1] != null) ? 1 : 0;
                    break;
                case 2:
                    auxID = (myDialogues[2] != null) ? 2 : 3;
                    break;
                case 3:
                    auxID = (myDialogues[3] != null) ? 3 : 2;
                    break;
                default:
                    break;
            }
            aux = myDialogues[auxID].GetRandomDialogue();
            if (aux.MainCharacter == null) aux.MainCharacter = GameManager.gameManager.battleController.MainCharacter;
            aux.MyNPC = this;
            StartCoroutine(DelayStartDialogue(aux));
        }
    }
    IEnumerator DelayStartDialogueBattle(DialogueBattle playerDialogue)
    {
        yield return new WaitForEndOfFrame();
        GameManager.gameManager.dialogueController.StartDialogue(answerDialogues[(int)playerDialogue.approachType], transform);
        //Dialogue answer = GameManager.gameManager.dialogueController.GetAnswer((int)enemyType, (int)thisPersonality, (int)playerDialogue.approachType, 2);
        //GameManager.gameManager.dialogueController.StartDialogue(answer, transform);
    }
    IEnumerator DelayStartDialogue(Dialogue failDialogue)
    {
        yield return new WaitForEndOfFrame();
        GameManager.gameManager.dialogueController.StartDialogue(failDialogue, transform);
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

    public void StartBattle(bool byDialogue = true)
    {
        if (!inBattle)
        {
            startPos = transform.position;
            //OnExit();
            Debug.Log("Bydialogue: " + byDialogue);
            Debug.Log("Hostil: " + hostile);
            if ((!byDialogue && hostile) || byDialogue)
            {                
                GetComponent<SphereCollider>().enabled = false;
                GameManager.gameManager.battleController.AddFighter(this);
                inBattle = true;
                Debug.Log("NPC entrou na batalha");
                lifeBar.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                Invoke("Flee", 0.2f);
            }
        }
    }

    public void EndBattle()
    {
        if (CanFight()) MoveNavMesh(startPos);
        inBattle = false;
        navMesh.speed = defaultSpeed;
        Invoke("ActiveInteractionCollider", 3.0f);
        lifeBar.transform.parent.gameObject.SetActive(false);
    }

    public bool CanFight()
    {
        return charStats.CanFight;
    }

    public bool IsInBattle()
    {
        return inBattle;
    }

    public bool ReceiveDamage(float damage)
    {
        bool aux = charStats.ReceiveDamage(damage);
        if (lifeBar != null) lifeBar.fillAmount = charStats.LifePercentage();
        return aux;
    }

    public void Die()
    {
        GameManager.gameManager.dialogueController.EndDialogue();
        GameManager.gameManager.battleController.FindAndRemove(name);
        Destroy(this.gameObject);
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    void ActiveInteractionCollider()
    {
        GetComponent<SphereCollider>().enabled = true;
    }
}
