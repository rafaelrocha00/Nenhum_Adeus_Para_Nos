using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class INPC : Interactives, BattleUnit
{
    public enum Personalities { Dandere, Kuudere, Tsundere, Yandere }

    [HideInInspector] CharacterStats charStats;
    public CharacterStats CharStats { get { return charStats; } }

    //[SerializeField] Behavior behavior;
    public Personalities thisPersonality;// { get { return behavior; } set { behavior = value; } }
    Personality personality;
    NavMeshAgent navMesh;

    public Dialogue myDialogue;
    public Dialogue[] answerDialogues = new Dialogue[5];
    //Dialogue initialDialogue;

    public bool hostile = false;

    bool attacking = false;
    public Weapon myWeapon;
    bool strongAtk = false;
    bool isRanged = false;

    [HideInInspector] Player mCharacter;
    public Player MCharacter { get { return mCharacter; } set { mCharacter = value; } }

    bool interacting = false;

    bool inBattle = false;

    float atkInterval = 0.15f;
    float timer = 0.0f;

    bool firstInteraction = false;

    Vector3 startPos;
    //LayerMask layermask = 1 << 10;

    private void Start()
    {
        personality = new Personality();
        SetPersonalityPercentages();

        charStats = new CharacterStats(this);
        navMesh = GetComponent<NavMeshAgent>();
        if (myWeapon is RangedW) isRanged = true;

        //initialDialogue = myDialogue;
    }
    void SetPersonalityPercentages()
    {
        for (int i = 0; i < 5; i++)
        {
            personality.SetPercentage(i, GameManager.gameManager.personalities[(int)thisPersonality][i]);
        }
    }

    public override void Interact(Player player)
    {
        mCharacter = player;
        myDialogue.MyNPC = this;
        myDialogue.MainCharacter = player;
        DesactiveBtp();
        //GameManager.gameManager.dialogueController.OpenDialoguePopUp(this.transform, this);
        GameManager.gameManager.dialogueController.StartDialogue(myDialogue, transform/*, this*/);
        interacting = true;
        //NextString();
    }

    public void NextString()
    {
        //GameManager.gameManager.dialogueController.UpdateText(myDialogue.NextString());
        GameManager.gameManager.dialogueController.NextString();
    }

    public void EndDialogue()
    {
        //GameManager.gameManager.dialogueController.CloseDialoguePopUp();
        //GameManager.gameManager.dialogueController.EndDialogue();
        myDialogue.ResetDialogue();
        //myDialogue = initialDialogue;
        interacting = false;
        firstInteraction = false;
    }

    public override void OnExit()
    {
        base.OnExit();
        EndDialogue();
    }

    //
    //public void ChangeDialogue(Dialogue newDialogue)
    //{
    //    myDialogue.ResetDialogue();
    //    myDialogue = newDialogue;
    //}
    //

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && interacting && !firstInteraction) firstInteraction = true;
        else if (Input.GetKeyDown(KeyCode.E) && interacting && firstInteraction) NextString();

        if (inBattle)
        { 
            Ray ray = new Ray(transform.position, mCharacter.transform.position - transform.position);
            RaycastHit hit;

            Vector3 lookPos = new Vector3(mCharacter.transform.position.x, transform.position.y, mCharacter.transform.position.z);
            transform.LookAt(lookPos);

            if (isRanged)
            {
                if (Vector3.Distance(mCharacter.transform.position, transform.position) <= myWeapon.range)
                {
                    Vector3 desiredPos = -(mCharacter.transform.position - transform.position) + transform.position;
                    MoveNavMesh(desiredPos);
                }
                else
                {
                    navMesh.isStopped = true;
                    //parar animação de andar
                }

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("player") || Vector3.Distance(hit.transform.position, transform.position) >= Vector3.Distance(mCharacter.transform.position, transform.position))
                    {
                        TryAttack();
                    }
                }
                else TryAttack();
            }
            else
            {
                Vector3 desiredPos = (mCharacter.transform.position - transform.position).normalized * (Vector3.Distance(transform.position, mCharacter.transform.position) - myWeapon.range / 2) + transform.position;
                MoveNavMesh(desiredPos);
                if (Vector3.Distance(mCharacter.transform.position, transform.position) <= myWeapon.range)
                    TryAttack();
            }

        }
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
                int randomType = Random.Range(0, 2);
                if (randomType == 1) strongAtk = false;
                else strongAtk = true;
                Attack();
                timer = 0.0f;
            }
        //}
    }

    void Attack()
    {
        if (!attacking)
        {
            //Debug.Log("Atacando");
            attacking = true;
            float attackCD;
            if (strongAtk && myWeapon is MeleeW)
            {
                MeleeW myMelee = (MeleeW)myWeapon;
                myMelee.SetStrongAttack();
            }
            attackCD = myWeapon.Attack();
            Invoke("AttackCooldown", attackCD);
        }
    }
    void AttackCooldown()
    {
        attacking = false;
    }

    public void ReceiveBattleDialogue(DialogueBattle playerDialogue)
    {
        Debug.Log("Dialogo Recebido");
        float answerChance = personality.CalculatePercentage(playerDialogue.approachType);
        float aux = Random.Range(0, 100);

        if (aux <= answerChance)
        {
            if (answerDialogues[(int)playerDialogue.approachType] != null)
            {
                answerDialogues[(int)playerDialogue.approachType].MyNPC = this;
                answerDialogues[(int)playerDialogue.approachType].MainCharacter = mCharacter;
                //GameManager.gameManager.dialogueController.StartDialogue(answerDialogues[(int)playerDialogue.approachType], transform);
                StartCoroutine(DelayStartDialogue(playerDialogue));
            }
            Debug.Log("Respondendo diálogo");
        }
    }
    IEnumerator DelayStartDialogue(DialogueBattle playerDialogue)
    {
        yield return new WaitForEndOfFrame();
        GameManager.gameManager.dialogueController.StartDialogue(answerDialogues[(int)playerDialogue.approachType], transform);
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

    public void StartBattle(bool byDialogue = true)
    {
        if (!inBattle)
        {
            startPos = transform.position;
            OnExit();
            if ((!byDialogue && hostile) || byDialogue)
            {                
                GetComponent<SphereCollider>().enabled = false;
                GameManager.gameManager.battleController.AddFighter(this);
                inBattle = true;
                Debug.Log("NPC entrou na batalha");             
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
        Invoke("ActiveInteractionCollider", 3.0f);
    }

    public bool CanFight()
    {
        return charStats.CanFight;
    }

    public bool IsInBattle()
    {
        return inBattle;
    }

    public void ReceiveDamage(float damage)
    {
        charStats.ReceiveDamage(damage);
    }

    void ActiveInteractionCollider()
    {
        GetComponent<SphereCollider>().enabled = true;
    }
}
