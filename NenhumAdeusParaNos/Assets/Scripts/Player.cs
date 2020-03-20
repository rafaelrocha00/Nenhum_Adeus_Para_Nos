using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, BattleUnit
{
    //public DialogueBattle[] battleDialogues = new DialogueBattle[5];

    [HideInInspector] CharacterStats charStats;
    public CharacterStats CharStats { get { return charStats; } }

    public float defaultSpeed = 3.5f;
    public float maxSpeed = 6.0f;
    public float runningSpeed = 9.0f;
    public float rotateSpeed = 1.0f;
    public float acceleration = 3.0f;
    public float accelerationTime = 1.0f;
    public float dashSpeed = 25.0f;
    public float dashTime = 0.75f;

    public MeleeW equippedMelee;
    public RangedW equippedRanged;

    public Weapon myWeapon;

    public float moveSpeed;
    float acceleratedSpeed;
    Vector3 forward, right;

    float moveTime = 0.0f;
    bool running;

    [HideInInspector] bool dashing = false;
    public bool Dashing { get { return dashing; } }

    public float strongAtkHoldTime = 0.7f;
    float strongAtkTimer = 0.0f;
    bool releasedAtk = false;
    //public float defaultAttackCD = 0.5f;
    //public float strongAttackCD = 1.0f;
    //float attackCD;
    //int strongAtk = 1;
    bool strongAtk = false;

    Vector3 battleAim = new Vector3(0, 0, 0);
    bool aimLocked = false;
    INPC targetedEnemy;

    bool isWeaponHide = false;

    [HideInInspector] bool battleDialoguing = false;
    public bool BattleDialoguing { get { return battleDialoguing; } set { battleDialoguing = value; } }
    bool attacking = false;

    CamMove cam;

    [HideInInspector] bool canInteract;
    public bool CanInteract { get { return canInteract; } set { canInteract = value; } }
    [HideInInspector] Interactives interactingObj;
    public Interactives InteractingObj { get { return interactingObj; } set { interactingObj = value; } }

    bool inBattle;
    LayerMask layermask = 1 << 0;

    private void Start()
    {
        //for (int i = 0; i < battleDialogues.Length; i++)
        //{
        //    if (battleDialogues[i] != null) battleDialogues[i].MainCharacter = this;
        //}

        charStats = new CharacterStats(this);

        moveSpeed = defaultSpeed;

        forward = Camera.main.transform.forward;
        cam = Camera.main.GetComponent<CamMove>();
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        GameManager.gameManager.MainHud.MainCharacter = this;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire3")) RunSwitch(true);
        if (Input.GetButtonUp("Fire3")) RunSwitch(false);


        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (!dashing) Move();
        }
        else
        {
            if (!running) moveSpeed = defaultSpeed;
            moveTime = 0.0f;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Item rápido");
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            SwitchWeapon();
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            //Trocar item
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            HideShowWeapon();
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (myWeapon is RangedW)
                {
                    Attack();
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (!releasedAtk && myWeapon is MeleeW)
                {
                    strongAtkTimer += Time.deltaTime;
                    if (strongAtkTimer >= strongAtkHoldTime)
                    {
                        releasedAtk = true;
                        Debug.Log("AtaqueForte");
                        strongAtk = true;
                        //attackCD = strongAttackCD;

                        Attack();
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (myWeapon is MeleeW)
                {
                    Debug.Log(strongAtkTimer);
                    if (strongAtkTimer < strongAtkHoldTime && !releasedAtk)
                    {
                        //Debug.Log("AtaqueFraco");
                        strongAtk = false;
                        //attackCD = defaultAttackCD;

                        Attack();
                    }
                    releasedAtk = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (canInteract)
            {
                interactingObj.Interact(this);
                canInteract = false;
            }
        }

        if (inBattle)
        {
            if (Input.GetMouseButtonDown(2))
            {
                LockAim();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                UseDialogue(0);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                UseDialogue(1);
            }
        }
    }

    void RunSwitch(bool value)
    {
        if (value)
        {
            running = value;
            acceleratedSpeed = moveSpeed;
            moveSpeed = runningSpeed;
        }
        else
        {
            running = value;
            moveSpeed = acceleratedSpeed;
        }
    }

    void Move()
    {
        moveTime += Time.deltaTime;
        if (moveTime < accelerationTime) moveSpeed += acceleration * Time.deltaTime;
        else if (!running && moveSpeed < maxSpeed) moveSpeed = maxSpeed;

        if (running) moveSpeed = Mathf.Clamp(moveSpeed, 0, runningSpeed);

        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 rightMov = right * xMov;
        Vector3 upMov = forward * zMov;

        Vector3 heading = Vector3.Normalize(rightMov + upMov);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, heading, rotateSpeed * Time.deltaTime, 0);

        transform.position += heading * moveSpeed * Time.deltaTime;
        if (!inBattle) transform.rotation = Quaternion.LookRotation(newDirection);
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //Vector3 lookPos;

            if (!aimLocked)
            {
                if (Physics.Raycast(ray, out hit, 1000, layermask))
                {
                    battleAim = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    //Debug.Log(lookPos);
                }
            }
            else battleAim = new Vector3(targetedEnemy.transform.position.x, transform.position.y, targetedEnemy.transform.position.z);

            transform.LookAt(battleAim);
            CheckLook_WalkDir(heading);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Dash(heading));
        }
        //cam.Move(transform.position);        
    }
    IEnumerator Dash(Vector3 dir)
    {
        dashing = true;
        float timer = 0.0f;
        do
        {
            transform.position += dir * dashSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        } while (timer < dashTime);
        dashing = false;
    }

    void CheckLook_WalkDir(Vector3 moveDir)
    {
        float auxDot = Vector3.Dot(transform.forward, moveDir);
        if (auxDot > 0.5f)
        {
            //Andando e olhando para mesma direção
        }
        else if (auxDot > -0.5f)
        {
            float auxDotRight = Vector3.Dot(transform.right, moveDir);
            if (auxDotRight > 0)
            {
                //Andando pra direita e olhando pre frente
            }
            else
            {
                //Andando pra esquerda e olhando pre frente
            }
        }
        else
        {
            //Andando pra trás e olhando pra frente
        }
    }

    void LockAim()
    {
        if (!aimLocked)
        {
            FindNearestEnemy();
        }

        aimLocked = !aimLocked;
    }

    void FindNearestEnemy()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        INPC[] allEnemies = GameManager.gameManager.battleController.AllEnemyFighters.ToArray();
        if (Physics.Raycast(ray, out hit, 1000, layermask))
        {

            for (int i = 0; i < allEnemies.Length; i++)
            {
                if (i > 0)
                {
                    if (Vector3.Distance(hit.point, allEnemies[i].transform.position) < Vector3.Distance(hit.point, allEnemies[i - 1].transform.position))
                        targetedEnemy = allEnemies[i];
                }
                else targetedEnemy = allEnemies[i];
            }
        }
        else targetedEnemy = allEnemies[0];
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
        strongAtkTimer = 0;       
    }

    void SwitchWeapon()
    {
        if (!isWeaponHide)
        {
            if (myWeapon is MeleeW)
            {
                myWeapon = equippedRanged;
                equippedRanged.gameObject.SetActive(true);
                equippedMelee.gameObject.SetActive(false);
            }
            else
            {
                myWeapon = equippedMelee;
                equippedRanged.gameObject.SetActive(false);
                equippedMelee.gameObject.SetActive(true);
            }
        }
    }

    void HideShowWeapon()
    {
        if (!isWeaponHide)
        {
            equippedMelee.gameObject.SetActive(false);
            equippedRanged.gameObject.SetActive(false);
        }
        else
        {
            if (myWeapon is MeleeW) equippedMelee.gameObject.SetActive(true);
            else equippedRanged.gameObject.SetActive(true);
        }
        isWeaponHide = !isWeaponHide;
    }

    public void UseDialogue(int idx)
    {
        //Debug.Log("Tentando usar dialogo");
        if (!GameManager.gameManager.dialogueController.ActiveDialogue && !GameManager.gameManager.MainHud.IsQuickMenuActive)
        {
            //Debug.Log("Tentando usar dialogo em batalha");
            try
            {
                //battleDialoguing = true;
                DialogueBattle actualDialogueBattle = GameManager.gameManager.MainHud.GetDialogueFromSlot(idx);
                actualDialogueBattle.MainCharacter = this;
                if (!aimLocked) FindNearestEnemy();
                actualDialogueBattle.TagetedNPC = targetedEnemy;

                GameManager.gameManager.dialogueController.StartDialogue(actualDialogueBattle, transform);
            }
            catch (System.Exception)
            {
                Debug.Log("Dialogo nulo ou em cooldown");
            }            
            //GameManager.gameManager.dialogueController.OpenDialoguePopUp(transform, null);
            //if (!aimLocked) FindNearestEnemy();
            //battleDialogues[0].TagetedNPC = targetedEnemy;
            //GameManager.gameManager.dialogueController.StartDialogue(battleDialogues[0], transform);
            //GameManager.gameManager.dialogueController.UpdateText(battleDialogues[0].NextString());
            //Invoke("BattleDialogueCooldown", 10);
        }
    }

    //void BattleDialogueCooldown()
    //{
    //    battleDialoguing = false;
    //}

    public void StartBattle(bool byDialogue = true)
    {
        //if (!byDialogue && !GameManager.gameManager.battleController.EnoughNPCs()) return;
        if (byDialogue) inBattle = true;
        GameManager.gameManager.battleController.MainCharacter = this;
    }
    public void DelayStartBattle()
    {
        inBattle = true;
    }

    public void EndBattle()
    {
        inBattle = false;
        aimLocked = false;
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
}
