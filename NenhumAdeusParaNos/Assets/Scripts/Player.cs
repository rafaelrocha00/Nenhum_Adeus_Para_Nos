using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, BattleUnit
{
    //public DialogueBattle[] battleDialogues = new DialogueBattle[5];

    [HideInInspector] CharacterStats charStats;
    public CharacterStats CharStats { get { return charStats; } }

    public Animator animator;

    public float defaultSpeed = 3.5f;
    public float maxSpeed = 6.0f;
    public float runningSpeed = 9.0f;
    public float rotateSpeed = 1.0f;
    public float acceleration = 3.0f;
    public float accelerationTime = 1.0f;
    public float dashSpeed = 25.0f;
    public float dashTime = 0.75f;
    //public float dashCooldown = 2.0f;
    //bool dashInCooldown = false;

    public float maxStamina = 100.0f;
    //public float stamina_runDecay = 25f;//TIRAR
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
    //public float defense_slow = 40.0f;//Porcentagem da diminuição de velocidade;

    public MeleeW equippedMelee;
    public RangedW equippedRanged;
    RangedConfig originalRconfig;
    MeleeConfig originalMconfig;
    bool autoShooting = false;
    bool shooting = false;

    public Weapon myWeapon;

    public float moveSpeed;
    float acceleratedSpeed;
    Vector3 forward, right;
    //float directionMod = 0;
    bool moving = false;

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

    [HideInInspector] bool aimingThrowable = false;
    public bool AimingThrowable { get { return aimingThrowable; } set { aimingThrowable = value; } }
    public GameObject launchTrajectory;
    ThrowableItem itemToThrow;
    GameObject granadeObj;
    public Transform rightHand;

    [HideInInspector] bool interacting = false;
    public bool Interacting { get { return interacting; } set { interacting = value; } }
    [HideInInspector] bool canInteract = true;
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
        //acceleratedSpeed = moveSpeed * 1.75f;
        stamina = maxStamina;
        defense_life = defense_maxLife;

        if (myWeapon == null) myWeapon = GetComponentInChildren<Weapon>();

        originalMconfig = equippedMelee.meleeConfig;
        originalRconfig = equippedRanged.rangedWConfig;

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

        //TIRAR
        //if (Input.GetButtonDown("Fire3"))
        //{
        //    RunSwitch(true);           
        //}
        //if (Input.GetButtonUp("Fire3"))
        //{
        //    RunSwitch(false);
        //}
        //TIRAR

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, layermask))
        {
            if (!aimLocked) battleAim = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            if (aimingThrowable) transform.LookAt(battleAim);
            //Debug.Log(lookPos);
        }        

        if (CanFight() && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            if (!dashing)
            {
                if (!moving && inBattle) animator.SetLayerWeight(1, 0);
                Move();
            }
        }
        else
        {
            if (!running) moveSpeed = defaultSpeed;
            moveTime = 0.0f;
            //moving = false;
            if (moving) StopMoving();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //Debug.Log("Item rápido");
            if (!aimingThrowable) UseEquippedItem();
            else CancelThrowItem();
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

        if (!EventSystem.current.IsPointerOverGameObject() && !interacting)
        {
            if (!isWeaponHide)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (myWeapon is RangedW)
                    {
                        RangedW ranged = (RangedW)myWeapon;

                        if (ranged.HasAmmo())
                        {
                            if (!ranged.Reloading)
                            {
                                if (defending) CancelDefense();
                                Attack();
                                if (ranged.IsAuto())
                                {
                                    autoShooting = true;
                                    StartCoroutine(AutoShoot(ranged));
                                }
                                else
                                {
                                    StopCoroutine("ResetNormalSpeed");
                                    shooting = true;
                                    //slowMoving = true;
                                    StartCoroutine(Slowdown(defaultSlow));
                                    StartCoroutine("ResetNormalSpeed");
                                }
                            }
                        }
                        else
                        {
                            //Animação de reload;
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
                            if (!GameManager.gameManager.inventoryController.Dragging)
                            {
                                releasedAtk = true;
                                //Debug.Log("AtaqueForte");
                                strongAtk = true;

                                Attack();
                            }
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (myWeapon is MeleeW)
                    {
                        if (!GameManager.gameManager.inventoryController.Dragging)
                        {
                            Debug.Log(strongAtkTimer);
                            if (strongAtkTimer < strongAtkHoldTime && !releasedAtk)
                            {
                                //Debug.Log("AtaqueFraco");
                                strongAtk = false;
                                if (defending) CancelDefense();
                                Attack();
                            }
                            releasedAtk = false;
                        }
                    }
                    else
                    {
                        autoShooting = false;
                    }
                }
            }
            else if (aimingThrowable)
            {
                if (Input.GetMouseButtonDown(0)) StartThrowItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (interactingObj != null && canInteract && CanFight() && !GameManager.gameManager.dialogueController.ActiveDialogue)
            {
                interacting = true;
                interactingObj.Interact(this);
                canInteract = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (myWeapon is RangedW)
            {
                RangedW ranged = (RangedW)myWeapon;
                ranged.Reload();
                //Animação de reload
            }
        }

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
        //else if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    equippedDialogueType = 4;
        //    GameManager.gameManager.MainHud.EquipDialogue(4);
        //}

        if (Input.GetKeyDown(KeyCode.F))
        {
            UseDialogue();
        }

        if (inBattle)
        {
            if (Input.GetMouseButtonDown(2))
            {
                LockAim();
            }

            //if (Input.GetKeyDown(KeyCode.V))
            //{
            //    UseDialogue(1);
            //}

            if (Input.GetMouseButtonDown(1))
            {
                if (!autoShooting)
                {
                    defending = true;
                    animator.SetBool("Defending", true);
                    if (!slowMoving) StartCoroutine(Slowdown(defaultSlow));
                    GameManager.gameManager.MainHud.ShowHideDefenseBar();
                }
            }
            if (Input.GetMouseButtonUp(1))
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
        yield return new WaitForSeconds(0.8f);
        //if (running) moveSpeed = runningSpeed;
        //slowMoving = false;
        CancelSlow();
        shooting = false;
    }

    IEnumerator AutoShoot(RangedW ranged)
    {
        //slowMoving = true;
        StartCoroutine(Slowdown(defaultSlow));
        do
        {
            yield return new WaitForSeconds(0.1f);
            if (ranged.HasAmmo())
            {
               if (!ranged.Reloading) Attack();
            }
            else
            {
                //Animação de reload
                CancelSlow();
                yield break;
            }
        } while (autoShooting);
        //slowMoving = false;
        CancelSlow();
    }

    void RunSwitch(bool value)
    {
        //TROCAR DE ANDAR PARA CORRENDO SOMENTE QUANDO ENTRAR EM BATALHA
        //CORRER NÃO GASTA STAMINA
        if (value/* && stamina > 0*/)
        {
            running = value;
            acceleratedSpeed = moveSpeed;
            moveSpeed = runningSpeed;
        }
        else if (!value && running)
        {
            running = value;
            moveSpeed = acceleratedSpeed;
            //StartCoroutine("StartStaminaRegen");
            //stoppedStaminaRegen = false;
        }
    }

    void Move(/*Ray ray*/)
    {
        moving = true;
        moveTime += Time.deltaTime;
        if (moveTime < accelerationTime) moveSpeed += acceleration * Time.deltaTime;
        else if (!running && moveSpeed < maxSpeed && !slowMoving) moveSpeed = maxSpeed;
        moveSpeed = Mathf.Clamp(moveSpeed, 0, runningSpeed);

        //TIRAR
        //if (running && stamina > 0 && !defending)
        //{
        //    //canRegenStamina = false;
        //    //if (!stoppedStaminaRegen)
        //    //{
        //    //    StopCoroutine("StartStaminaRegen");
        //    //    stoppedStaminaRegen = true;
        //    //}
        //    CancelStaminaRegen(true);
        //    moveSpeed = Mathf.Clamp(moveSpeed, 0, runningSpeed);
        //    UpdateStamina(-stamina_runDecay * Time.deltaTime);
        //    if (stamina == 0) RunSwitch(false);
        //}
        //TIRAR

        //else if (defending)
        //{
        //    moveSpeed = defaultSpeed - defaultSpeed * defense_slow / 100;
        //}
        //if (!defending && slowMoving)
        //{
        //    if (running) moveSpeed = runningSpeed - runningSpeed * defaultSlow / 100;
        //    else moveSpeed = defaultSpeed*2 - defaultSpeed*2 * defaultSlow / 100;
        //}

        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 rightMov = right * xMov;
        Vector3 upMov = forward * zMov;
        //if (upMov.x < 0 && rightMov == Vector3.zero) GameManager.gameManager.MainCamera.SetToWalkDown();
        //else GameManager.gameManager.MainCamera.SetDefaultDistance();

        Vector3 heading = Vector3.Normalize(rightMov + upMov);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, heading, rotateSpeed * Time.deltaTime, 0);

        transform.position += heading * moveSpeed * Time.deltaTime;
        //if (!inBattle) transform.rotation = Quaternion.LookRotation(newDirection);
        //else
        //{
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;
        //Vector3 lookPos;

        //if (!aimLocked)
        //{
        //    //if (Physics.Raycast(ray, out hit, 1000, layermask))
        //    //{
        //    //    battleAim = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        //    //    Debug.Log(lookPos);
        //    //}
        //}
        if (aimLocked)
        {
            if (targetedEnemy != null)
            {
                battleAim = new Vector3(targetedEnemy.transform.position.x, transform.position.y, targetedEnemy.transform.position.z);
            }
            else LockAim();
        }

        CheckLook_WalkDir(heading, xMov, zMov);


        //float velX = heading.x * moveSpeed / runningSpeed;
        //float velY = heading.z * moveSpeed / runningSpeed;
        ////Debug.Log(velX + " | " + velY);
        //animator.SetFloat("VelX", velX);
        //animator.SetFloat("VelY", velY);

        if (!aimingThrowable) transform.LookAt(battleAim);        
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (stamina >= stamina_dashCost/* && !dashInCooldown*/)
            {
                //Cooldown do dash
                //StartCoroutine("DashCooldown");
                CancelStaminaRegen(false);
                UpdateStamina(-stamina_dashCost);
                StartCoroutine(Dash(heading));
                StartCoroutine("StartStaminaRegen");
            }            
        }
        //cam.Move(transform.position);        
    }

    void StopMoving()
    {
        animator.SetFloat("VelX", 0);
        animator.SetFloat("VelY", 0);

        if (inBattle) animator.SetLayerWeight(1, 1);
        moving = false;
    }

    IEnumerator Slowdown(float slowValue)
    {
        slowMoving = true;
        while (slowMoving || defending)
        {
            float actualMoveSpeed = (running) ? runningSpeed : defaultSpeed * 2;
            if (!defending) moveSpeed = actualMoveSpeed - actualMoveSpeed * slowValue / 100;
            yield return new WaitForEndOfFrame();            
        }        
    }
    void CancelSlow()
    {
        if (!autoShooting && !shooting && slowMoving && !defending)
        {
            slowMoving = false;
            if (running) moveSpeed = runningSpeed;
            else moveSpeed = defaultSpeed;
        }
    }

    IEnumerator Dash(Vector3 dir)
    {
        dashing = true;
        animator.SetBool("Dashing", true);
        float timer = 0.0f;
        do
        {
            transform.position += dir * dashSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        } while (timer < dashTime);
        dashing = false;
        animator.SetBool("Dashing", false);
    }
    //IEnumerator DashCooldown()
    //{
    //    dashInCooldown = true;
    //    yield return new WaitForSeconds(dashCooldown);
    //    dashInCooldown = false;
    //}

    void CancelDefense()
    {
        defending = false;
        StartCoroutine("StartStaminaRegen");
        stoppedStaminaRegen = false;
        animator.SetBool("Defending", false);
        CancelSlow();
        GameManager.gameManager.MainHud.ShowHideDefenseBar();
    }

    void CheckLook_WalkDir(Vector3 moveDir, float xmov, float zmov)
    {
        float auxDot = Vector3.Dot(transform.forward, moveDir);
        float dirX = 0;
        float dirZ = 0;

        if (auxDot > 0.5f)
        {
            dirZ = 1;
            //Andando e olhando para mesma direção
            /*if (!autoShooting && !shooting && slowMoving)*/ CancelSlow(); //slowMoving = false;
        }
        else if (auxDot > -0.5f)
        {
            //dirZ = 0;           
            /*if (!autoShooting && !shooting && slowMoving)*/ CancelSlow();//slowMoving = false;
            float auxDotRight = Vector3.Dot(transform.right, moveDir);
            if (auxDotRight > 0)
            {
                dirX = 1;
                //Andando pra direita e olhando pre frente
            }
            else
            {
                dirX = -1;
                //Andando pra esquerda e olhando pre frente
            }
        }
        else
        {
            dirZ = -1;
            //Andando pra trás e olhando pra frente
            if (inBattle && !slowMoving) StartCoroutine(Slowdown(defaultSlow));//slowMoving = true;
        }

        if (xmov == 0) xmov = 1;
        if (zmov == 0) zmov = 1;

        float velX = dirX * moveSpeed / runningSpeed * Mathf.Abs(xmov);
        float velY = dirZ * moveSpeed / runningSpeed * Mathf.Abs(zmov);
        //Debug.Log(velX + " | " + velY);
        animator.SetFloat("VelX", velX);
        animator.SetFloat("VelY", velY);
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

        INPC[] allEnemies = GameManager.gameManager.dialogueController.GetNearbyNPCs(transform.position, 10);
        if (allEnemies != null && allEnemies.Length > 0)
        {
            if (Physics.Raycast(ray, out hit, 1000, layermask))
            {

                for (int i = 0; i < allEnemies.Length; i++)
                {
                    if (i > 0)
                    {
                        if ((hit.point - allEnemies[i].transform.position).sqrMagnitude < (hit.point - allEnemies[i - 1].transform.position).sqrMagnitude)
                            targetedEnemy = allEnemies[i];
                    }
                    else targetedEnemy = allEnemies[i];
                }
            }
            else targetedEnemy = allEnemies[0];
        }
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
            attackCD = myWeapon.Attack(animator);            
            Invoke("AttackCooldown", attackCD);
            Invoke("ResetAnim", 0.2f);
        }
    }
    void ResetAnim()
    {
        animator.SetInteger("Attacking", 0);
    }

    void AttackCooldown()
    {
        attacking = false;
        strongAtkTimer = 0;       
    }

    //////////////////////////////////////////////
    public void EquipWeapon(WeaponConfig wconfig)
    {
        Debug.Log("EquippingWeapon");
        if (wconfig is MeleeConfig)
        {
            equippedMelee.Equip(wconfig);
        }
        else
        {
            equippedRanged.Equip(wconfig);
        }
    }
    public void EquipOriginalWeapon(bool isRanged)
    {
        if (isRanged) equippedRanged.Equip(originalRconfig);
        else equippedMelee.Equip(originalMconfig);
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
        if (!animator.GetBool("Healing"))
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
    }

    public void UseEquippedItem()
    {
        GameManager.gameManager.inventoryController.Inventory.quickItemSlot.UseItemEffect();
    }

    public void StartAiming(ThrowableItem tI)
    {
        itemToThrow = tI;
        aimingThrowable = true;
        animator.SetBool("Aiming", true);
        launchTrajectory.SetActive(true);
        HideShowWeapon();

        granadeObj = Instantiate(tI.itemToThrow, rightHand.position, tI.itemToThrow.transform.rotation) as GameObject;
        granadeObj.transform.SetParent(rightHand);
        granadeObj.GetComponent<Granade>().Lock();
    }
    public void StartThrowItem()
    {
        if (aimingThrowable)
        {
            Debug.Log("Arremessando");
            aimingThrowable = false;
            animator.SetBool("ThrowedItem", true);
            animator.SetBool("Aiming", false);
            //Animação de arremessar
            Invoke("ThrowItem", itemToThrow.AnimTime);
        }
    }
    void ThrowItem()
    {
        granadeObj.transform.SetParent(null);
        Vector3 force = (launchTrajectory.GetComponent<LauchTragectory>().MousePos - transform.position) + Vector3.up * 10;
        launchTrajectory.SetActive(false);
        Granade aux = granadeObj.GetComponent<Granade>();
        aux.Unlock();
        itemToThrow.Throw(force, aux);
        GameManager.gameManager.inventoryController.Inventory.quickItemSlot.ConfirmUse();
        animator.SetBool("ThrowedItem", false);
        HideShowWeapon();
    }

    public void CancelThrowItem()
    {
        launchTrajectory.SetActive(false);
        aimingThrowable = false;
        animator.SetBool("Aiming", false);
        Destroy(granadeObj);
        HideShowWeapon();
    }

    public void UseDialogue(/*int idx*/)
    {
        //Debug.Log("Tentando usar dialogo");
        Debug.Log(GameManager.gameManager.dialogueController.ActiveDialogue);
        Debug.Log(dialogueInCooldown);
        if (!GameManager.gameManager.dialogueController.ActiveDialogue /*&& !GameManager.gameManager.MainHud.IsQuickMenuActive*/ && !dialogueInCooldown)
        {
            //Debug.Log("Tentando usar dialogo em batalha");
            try
            {
                //battleDialoguing = true;
                //DialogueBattle actualDialogueBattle = GameManager.gameManager.MainHud.GetDialogueFromSlot(idx);
                if (!aimLocked) FindNearestEnemy();
                int random = Random.Range(0, 2);
                DialogueBattle actualDialogueBattle = GameManager.gameManager.dialogueController.GetDialogueBattle((int)targetedEnemy.enemyType, equippedDialogueType, random);                
                actualDialogueBattle.MainCharacter = this;
                actualDialogueBattle.TagetedNPC = targetedEnemy;

                if (GameManager.gameManager.battleController.ActiveBattle)
                {
                    dialogueInCooldown = true;
                    GameManager.gameManager.MainHud.IconCooldown(dialogueCooldown);
                    Invoke("DialogueCooldown", dialogueCooldown);
                }
                GameManager.gameManager.dialogueController.StartDialogue(actualDialogueBattle, transform);                
            }
            catch
            {
                Debug.Log("Dialogo nulo ou Sem inimigos perto");
            }
            //GameManager.gameManager.dialogueController.OpenDialoguePopUp(transform, null);
            //if (!aimLocked) FindNearestEnemy();
            //battleDialogues[0].TagetedNPC = targetedEnemy;
            //GameManager.gameManager.dialogueController.StartDialogue(battleDialogues[0], transform);
            //GameManager.gameManager.dialogueController.UpdateText(battleDialogues[0].NextString());
            //Invoke("BattleDialogueCooldown", 10);
        }
        else if (GameManager.gameManager.dialogueController.WaitingForAnswer)
        {
            DialogueBattle actualDialogueBattle = GameManager.gameManager.dialogueController.GetDialogueBattle((int)targetedEnemy.enemyType, equippedDialogueType, 0);
            actualDialogueBattle.MainCharacter = this;
            actualDialogueBattle.TagetedNPC = targetedEnemy;
            GameManager.gameManager.dialogueController.StartDialogue(actualDialogueBattle, transform);
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
        if (byDialogue)
        {
            interacting = false;
            //animator.SetLayerWeight(1, 0);
            //inBattle = true;            
            //RunSwitch(true);
            //animator.SetBool("InBattle", true);
            CombatSet();
        }
        GameManager.gameManager.battleController.MainCharacter = this;        
    }
    public void DelayStartBattle()
    {
        //animator.SetLayerWeight(1, 0);
        //inBattle = true;
        //RunSwitch(true);
        //animator.SetBool("InBattle", true);
        CombatSet();
    }
    void CombatSet()
    {
        animator.SetLayerWeight(1, 0);
        inBattle = true;
        RunSwitch(true);
        animator.SetBool("InBattle", true);
    }

    public void EndBattle()
    {
        animator.SetBool("InBattle", false);        
        RunSwitch(false);
        inBattle = false;
        aimLocked = false;
        if (defending)
        {
            CancelDefense();
        }
        defense_life = defense_maxLife;
        UpdateDefense(0);
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
        if (defending)
        {
            animator.SetBool("DefendedAttack", true);
            Invoke("BackToDefending", 0.1f);
        }
        UpdateLife();
        return aux;
    }
    void BackToDefending()
    {
        animator.SetBool("DefendedAttack", false);
    }

    public void Die()
    {
        GameManager.gameManager.dialogueController.EndDialogue();
        GameManager.gameManager.MainHud.GameOver();
    }

    public Vector3 GetPos()
    {
        return transform.position;
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

    public void Heal(float value)
    {
        charStats.UpdateLife(value);        
        UpdateLife();
        HideShowWeapon();
        animator.SetBool("Healing", false);        
    }
    public void HealAnim()
    {
        animator.SetBool("Healing", true);
        if (!isWeaponHide) HideShowWeapon();
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
