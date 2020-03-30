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
    public float dashCooldown = 2.0f;
    bool dashInCooldown = false;

    public float maxStamina = 100.0f;
    public float stamina_runDecay = 25f;
    public float stamina_dashCost = 25f;
    public float stamina_defendingDecay = 10.0f;
    public float stamina_regen = 15f;
    bool stoppedStaminaRegen = false;
    bool canRegenStamina = true;
    float stamina;

    [SerializeField] float defense_strength = 75.0f;//Porcentagem da diminuição de dano;
    public float Defense_Strength { get { return defense_strength; } }    
    [HideInInspector] bool defending = false;
    public bool Defending { get { return defending; } }
    [SerializeField] float defense_maxLife = 40.0f;
    public float Defense_MaxLife { get { return defense_maxLife; } }
    float defense_life;
    public float defense_slow = 40.0f;//Porcentagem da diminuição de velocidade;

    public MeleeW equippedMelee;
    public RangedW equippedRanged;
    bool autoShooting = false;
    bool shooting = false;

    public Weapon myWeapon;

    public float moveSpeed;
    float acceleratedSpeed;
    Vector3 forward, right;

    float moveTime = 0.0f;
    bool running;
    bool slowMoving = false;
    public float defaultSlow = 30.0f;//%

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

    public float dialogueCooldown = 10.0f;
    bool dialogueInCooldown = false;
    int equippedDialogueType = 0;

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
        stamina = maxStamina;
        defense_life = defense_maxLife;

        forward = Camera.main.transform.forward;
        cam = Camera.main.GetComponent<CamMove>();
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        GameManager.gameManager.MainHud.MainCharacter = this;        
    }

    void Update()
    {
        if (canRegenStamina)
        {
            UpdateStamina(stamina_regen * Time.deltaTime);
        }

        if (Input.GetButtonDown("Fire3"))
        {
            RunSwitch(true);           
        }
        if (Input.GetButtonUp("Fire3"))
        {
            RunSwitch(false);
        }


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
                    RangedW ranged = (RangedW)myWeapon;
                    if (ranged.auto)
                    {
                        autoShooting = true;
                        StartCoroutine("AutoShoot");
                    }
                    else
                    {
                        StopCoroutine("ResetNormalSpeed");
                        shooting = true;
                        slowMoving = true;
                        StartCoroutine("ResetNormalSpeed");
                    }
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
                        //Debug.Log("AtaqueForte");
                        strongAtk = true;

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

                        Attack();
                    }
                    releasedAtk = false;
                }
                else
                {
                    autoShooting = false;
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
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                equippedDialogueType = 0;
                GameManager.gameManager.MainHud.EquipDialogue(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                equippedDialogueType = 1;
                GameManager.gameManager.MainHud.EquipDialogue(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                equippedDialogueType = 2;
                GameManager.gameManager.MainHud.EquipDialogue(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                equippedDialogueType = 3;
                GameManager.gameManager.MainHud.EquipDialogue(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                equippedDialogueType = 4;
                GameManager.gameManager.MainHud.EquipDialogue(4);
            }

            if (Input.GetMouseButtonDown(2))
            {
                LockAim();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                UseDialogue();
            }
            //if (Input.GetKeyDown(KeyCode.V))
            //{
            //    UseDialogue(1);
            //}

            if (Input.GetKeyDown(KeyCode.V))
            {
                defending = true;
                GameManager.gameManager.MainHud.ShowHideDefenseBar();
            }
            if (Input.GetKeyUp(KeyCode.V))
            {
                if (defending) CancelDefense();
            }

            if (defending && stamina > 0)
            {
                CancelStaminaRegen(true);
                UpdateStamina(-stamina_defendingDecay * Time.deltaTime);
                //moveSpeed = defaultSpeed - defaultSpeed * defense_slow / 100;
                if (stamina == 0) CancelDefense();
            }
        }
    }

    IEnumerator ResetNormalSpeed()
    {
        yield return new WaitForSeconds(0.5f);
        if (running) moveSpeed = runningSpeed;
        slowMoving = false;
        shooting = false;
    }

    IEnumerator AutoShoot()
    {
        slowMoving = true;
        do
        {
            yield return new WaitForSeconds(0.1f);
            Attack();
        } while (autoShooting);
        slowMoving = false;
    }

    void RunSwitch(bool value)
    {
        if (value && stamina > 0)
        {
            running = value;
            acceleratedSpeed = moveSpeed;
            moveSpeed = runningSpeed;
        }
        else if (!value && running)
        {
            running = value;
            moveSpeed = acceleratedSpeed;
            StartCoroutine("StartStaminaRegen");
            stoppedStaminaRegen = false;
        }
    }

    void Move()
    {
        moveTime += Time.deltaTime;
        if (moveTime < accelerationTime) moveSpeed += acceleration * Time.deltaTime;
        else if (!running && moveSpeed < maxSpeed) moveSpeed = maxSpeed;

        if (running && stamina > 0 && !defending)
        {
            //canRegenStamina = false;
            //if (!stoppedStaminaRegen)
            //{
            //    StopCoroutine("StartStaminaRegen");
            //    stoppedStaminaRegen = true;
            //}
            CancelStaminaRegen(true);
            moveSpeed = Mathf.Clamp(moveSpeed, 0, runningSpeed);
            UpdateStamina(-stamina_runDecay * Time.deltaTime);
            if (stamina == 0) RunSwitch(false);
        }
        else if (defending)
        {
            moveSpeed = defaultSpeed - defaultSpeed * defense_slow / 100;
        }
        if (!defending && slowMoving)
        {
            if (running) moveSpeed = runningSpeed - runningSpeed * defaultSlow / 100;
            else moveSpeed = defaultSpeed*2 - defaultSpeed*2 * defaultSlow / 100;
        }

        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 rightMov = right * xMov;
        Vector3 upMov = forward * zMov;

        Vector3 heading = Vector3.Normalize(rightMov + upMov);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, heading, rotateSpeed * Time.deltaTime, 0);

        transform.position += heading * moveSpeed * Time.deltaTime;
        //if (!inBattle) transform.rotation = Quaternion.LookRotation(newDirection);
        //else
        //{
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
        else
        {
            if (targetedEnemy != null)
            {
                battleAim = new Vector3(targetedEnemy.transform.position.x, transform.position.y, targetedEnemy.transform.position.z);
            }
            else LockAim();
        }

        transform.LookAt(battleAim);
        CheckLook_WalkDir(heading);
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (stamina >= stamina_dashCost && !dashInCooldown)
            {
                //Cooldown do dash
                StartCoroutine("DashCooldown");
                CancelStaminaRegen(false);
                UpdateStamina(-stamina_dashCost);
                StartCoroutine(Dash(heading));
                StartCoroutine("StartStaminaRegen");
            }            
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
    IEnumerator DashCooldown()
    {
        dashInCooldown = true;
        yield return new WaitForSeconds(dashCooldown);
        dashInCooldown = false;
    }

    void CancelDefense()
    {
        defending = false;
        StartCoroutine("StartStaminaRegen");
        stoppedStaminaRegen = false;
        GameManager.gameManager.MainHud.ShowHideDefenseBar();
    }

    void CheckLook_WalkDir(Vector3 moveDir)
    {
        float auxDot = Vector3.Dot(transform.forward, moveDir);
        if (auxDot > 0.5f)
        {
            //Andando e olhando para mesma direção
            if (!autoShooting && !shooting) slowMoving = false;
        }
        else if (auxDot > -0.5f)
        {
            if (!autoShooting && !shooting) slowMoving = false;
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
            if (inBattle) slowMoving = true;
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

    public void UseDialogue(/*int idx*/)
    {
        //Debug.Log("Tentando usar dialogo");
        if (!GameManager.gameManager.dialogueController.ActiveDialogue && !GameManager.gameManager.MainHud.IsQuickMenuActive && !dialogueInCooldown)
        {
            //Debug.Log("Tentando usar dialogo em batalha");
            try
            {
                //battleDialoguing = true;
                //DialogueBattle actualDialogueBattle = GameManager.gameManager.MainHud.GetDialogueFromSlot(idx);
                dialogueInCooldown = true;
                if (!aimLocked) FindNearestEnemy();
                int random = Random.Range(0, 2);
                DialogueBattle actualDialogueBattle = GameManager.gameManager.dialogueController.GetDialogueBattle((int)targetedEnemy.enemyType, equippedDialogueType, random);
                actualDialogueBattle.MainCharacter = this;
                actualDialogueBattle.TagetedNPC = targetedEnemy;

                GameManager.gameManager.MainHud.IconCooldown(dialogueCooldown);
                GameManager.gameManager.dialogueController.StartDialogue(actualDialogueBattle, transform);
                Invoke("DialogueCooldown", dialogueCooldown);
            }
            catch
            {
                Debug.Log("Dialogo nulo");
            }
            //GameManager.gameManager.dialogueController.OpenDialoguePopUp(transform, null);
            //if (!aimLocked) FindNearestEnemy();
            //battleDialogues[0].TagetedNPC = targetedEnemy;
            //GameManager.gameManager.dialogueController.StartDialogue(battleDialogues[0], transform);
            //GameManager.gameManager.dialogueController.UpdateText(battleDialogues[0].NextString());
            //Invoke("BattleDialogueCooldown", 10);
        }
    }
    void DialogueCooldown()
    {
        dialogueInCooldown = false;
    }

    //void BattleDialogueCooldown()
    //{
    //     = false;
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
        if (defending)
        {
            CancelDefense();
        }
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
        UpdateLife();
    }

    public void Die()
    {
        GameManager.gameManager.dialogueController.EndDialogue();
    }

    IEnumerator StartStaminaRegen()
    {
        yield return new WaitForSeconds(1.5f);
        canRegenStamina = true;
    }

    void CancelStaminaRegen(bool continuousAction)
    {
        canRegenStamina = false;
        if (continuousAction)
        {            
            if (!stoppedStaminaRegen)
            {
                StopCoroutine("StartStaminaRegen");
                stoppedStaminaRegen = true;
            }
        }
        else StopCoroutine("StartStaminaRegen");
    }

    void UpdateStamina(float value)
    {
        stamina += value;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        GameManager.gameManager.MainHud.UpdateStamina(stamina / maxStamina);
    }
    void UpdateLife()
    {
        GameManager.gameManager.MainHud.UpdateLife(charStats.LifePercentage());
    }
    public void UpdateDefense(float value)
    {
        defense_life += value;
        defense_life = Mathf.Clamp(defense_life, 0, defense_maxLife);
        GameManager.gameManager.MainHud.UpdateDefense(defense_life / defense_maxLife);
    }
}
