using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, BattleUnit
{
    [HideInInspector] CharacterStats charStats;
    public CharacterStats CharStats { get { return charStats; } }

    public Animator animator;
    public GameObject staminaBar;
    public ShieldShaderControl shieldControl;

    public Sprite portrait;

    [HideInInspector] List<Companion> myCompanions = new List<Companion>();
    public List<Companion> MyCompanions { get { return myCompanions; } set { myCompanions = value; } }

    public float defaultSpeed = 3.5f;
    public float maxSpeed = 6.0f;
    public float runningSpeed = 9.0f;
    public float rotateSpeed = 1.0f;
    public float acceleration = 3.0f;
    public float accelerationTime = 1.0f;
    public float dashSpeed = 25.0f;
    public float dashTime = 0.75f;
    public float dashDistance = 7.5f;
    public int dashMaxQuant = 2;
    public float dashCooldown = 2.5f;
    int dashQuant = 2;
    bool dashInCooldown = false;

    [SerializeField] float defense_strength = 75.0f;//Porcentagem da diminuição de dano;
    public float Defense_Strength { get { return defense_strength; } }    
    [HideInInspector] bool defending = false;
    public bool Defending { get { return defending; } }
    [SerializeField] float defense_maxLife = 40.0f;
    public float Defense_MaxLife { get { return defense_maxLife; } }
    public float defense_parryGap = 0.1f;
    float defense_parryTimer = 0;    
    float defense_life;

    //public MeleeW equippedMelee;
    //public RangedW equippedRanged;
    Weapon[] weapons = new Weapon[4];
    public MeleeW weapon_melee;
    public MeleeW weapon_extraMelee;
    public RangedW weapon_ranged;   
    public RangedW weapon_extraRanged;
    public MeleeConfig weaponConfig_default;
    //bool usingFirstWeapon = true;
    //int weaponComb = 0;/////////////////////////0 = 1 melee e 1 ranged | 1 = 2 melees | 2 = 2 rangeds | 3 = 1 melee e 1 ranged invertidos
    //RangedConfig originalRconfig;
    //MeleeConfig originalMconfig;
    bool autoShooting = false;
    bool shooting = false;

    public Weapon myWeapon;

    public float moveSpeed;
    float acceleratedSpeed;
    Vector3 forward, right;
    //float directionMod = 0;
    bool moving = false;
    int movingTowardWall = 1;
    Vector3 heading = Vector3.zero;

    float moveTime = 0.0f;
    bool running;
    bool slowMoving = false;
    public float defaultSlow = 30.0f;//%

    [HideInInspector] bool canMove = true;
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    [HideInInspector] bool dashing = false;
    public bool Dashing { get { return dashing; } }

    public float strongAtkHoldTime = 0.7f;
    float strongAtkTimer = 0.0f;
    bool releasedAtk = false;
    bool strongAtk = false;

    Vector3 battleAim = new Vector3(0, 0, 0);
    bool aimLocked = false;
    INPC targetedEnemy;

    bool isWeaponHide = false;

    [HideInInspector] bool battleDialoguing = false;
    public bool BattleDialoguing { get { return battleDialoguing; } set { battleDialoguing = value; } }
    bool attacking = false;

    public float dialogueCooldown = 10.0f;

    CamMove cam;

    [HideInInspector] bool placingItem = false;
    public bool PlacingItem { get { return placingItem; } set { placingItem = value; } }
    [HideInInspector] bool aimingThrowable = false;
    public bool AimingThrowable { get { return aimingThrowable; } set { aimingThrowable = value; } }
    public GameObject launchTrajectory;
    ThrowableItem itemToThrow;
    PlaceableItem itemToPlace;
    GameObject granadeObj;
    public Transform rightHand;

    [HideInInspector] bool interacting = false;
    public bool Interacting { get { return interacting; } set { interacting = value; } }
    [HideInInspector] bool canInteract = true;
    public bool CanInteract { get { return canInteract; } set { canInteract = value; } }
    [HideInInspector] List<Interactives> interactingObjs = new List<Interactives>();
    public List<Interactives> InteractingObjs { get { return interactingObjs; } set { interactingObjs = value; } }

    #region Audio Clips
    public AudioClip clip_walkGrass;
    public AudioClip clip_hit;
    public AudioClip clip_heal;
    public AudioClip clip_dash;
    public AudioClip clip_shieldHit;
    public AudioClip clip_shieldBreak;
    #endregion
    public float walkSFXInterval = 0.5f;

    [HideInInspector] bool battleUnlocked = false;
    public bool BattleUnlocked { get { return battleUnlocked; } set { battleUnlocked = value; } }

    bool inBattle;
    LayerMask aimLayermask = 1 << 0;
    LayerMask dashMask;

    bool canReceiveKnockback = true;

    bool playedShieldBreakeSong = false;

    CharacterController cc;

    private void Start()
    {
        cc = GetComponent<CharacterController>();

        string[] aux = new string[2];
        aux[0] = "Default";
        aux[1] = "Walls";
        dashMask = LayerMask.GetMask(aux);

        charStats = new CharacterStats(this);

        moveSpeed = defaultSpeed;

        dashQuant = dashMaxQuant;
        defense_life = defense_maxLife;

        if (myWeapon == null) myWeapon = GetComponentInChildren<Weapon>();

        weaponConfig_default = weapon_melee.meleeConfig;
        //originalRconfig = equippedRanged.rangedWConfig;

        //equippedMelee.myHolder = this;
        //equippedRanged.myHolder = this;
        weapons[0] = weapon_melee; weapon_melee.isEquipped = true;
        weapons[1] = weapon_extraMelee;
        weapons[2] = weapon_ranged; weapon_ranged.isEquipped = true;
        weapons[3] = weapon_extraRanged;
        //weapon_melee.myHolder = this;
        //weapon_extraMelee.myHolder = this;
        //weapon_ranged.myHolder = this;
        //weapon_extraRanged.myHolder = this;
        for (int i = 0; i < 4; i++)
            weapons[i].myHolder = this;

        forward = Camera.main.transform.forward;
        cam = Camera.main.GetComponent<CamMove>();
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        if (!battleUnlocked) HideShowWeapon();
        StartCoroutine(WalkSFX());

        if (!GameManager.gameManager.NewGame) EnableCharController(true);

        StartCoroutine("GetMainHUD");

        Invoke("SpawnCompanions", 0.02f);
    }
    IEnumerator GetMainHUD()
    {
        yield return new WaitForEndOfFrame();
        GameManager.gameManager.MainHud.MainCharacter = this;
    }

    public void SpawnCompanions()
    {
        if (GameManager.gameManager == null || myCompanions.Count > 0 || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Farmacia")) return;

        Debug.Log("Checando se tem companion pra spawnar");

        for (int i = 0; i < GameManager.gameManager.PlayerCompanionsPref.Count; i++)
        {
            GameObject aux = Instantiate(GameManager.gameManager.PlayerCompanionsPref[i], transform.position, transform.rotation) as GameObject;
            myCompanions.Add(aux.GetComponent<Companion>());
        }
    }

    public void MoveCompanions(Vector3 newPos)
    {
        for (int i = 0; i < myCompanions.Count; i++)
        {
            myCompanions[i].DirectMove(newPos);
        }
    }

    public void DespawnCompanions()
    {
        for (int i = 0; i < myCompanions.Count; i++)
        {
            myCompanions[i].SpawnMyNPC();
        }
    }

    public void RemoveCompanions()
    {
        for (int i = 0; i < myCompanions.Count; i++)
        {
            Destroy(myCompanions[i].gameObject);
        }
        myCompanions.Clear();
        GameManager.gameManager.PlayerCompanionsPref.Clear();
    }
    //private void OnDisable()
    //{
    //for (int i = 0; i < myCompanions.Count; i++)
    //{
    //    GameManager.gameManager.PlayerCompanionsPref.Add(myCompanions[i].myPref);
    //}
    //}

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Debug.Log(moving);

        if (cc.enabled && !cc.isGrounded)
        {
            Vector3 downForce = new Vector3(0, -9.81f, 0) * Time.deltaTime;
            cc.Move(downForce);
        }

        if (Physics.Raycast(ray, out hit, 1000, aimLayermask))
        {
            //Debug.Log("Colisor: " + hit.collider.name);
            if (!aimLocked) battleAim = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            if (aimingThrowable || placingItem) transform.LookAt(battleAim);
            //Debug.Log(lookPos);
        }        

        if (canMove && CanFight() && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
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
            if (moving) StopMoving();
        }

        if (Input.GetMouseButtonDown(2))//GetKeyDown(KeyCode.C))
        {
            //Debug.Log("Item rápido");
            if (!aimingThrowable && !placingItem) UseEquippedItem();
            else if (aimingThrowable) CancelThrowItem();
            else if (placingItem) CancelPlaceItem();
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            //SwitchWeapon();
            GameManager.gameManager.inventoryController.Inventory.ChangeQuickItem(true);
        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            GameManager.gameManager.inventoryController.Inventory.ChangeQuickItem(false);
        }
        if (Input.GetMouseButtonDown(1))
        {
            SwitchWeapon();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (battleUnlocked) HideShowWeapon();
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
            else if (placingItem)
            {
                if (Input.GetMouseButtonDown(0)) PlaceItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("InteracingObjs != null? " + interactingObjs != null);

            if (interactingObjs != null && interactingObjs.Count > 0 && canInteract && CanFight() && !GameManager.gameManager.dialogueController.ActiveMainDialogue)
            {
                Debug.Log("Achando o npc mais na sua frente");
                interacting = true;
                Interactives aux = interactingObjs[0];
                for (int i = 0; i < interactingObjs.Count; i++)
                {
                    if (Vector3.Dot((interactingObjs[i].transform.position - this.transform.position).normalized, transform.forward) >
                        Vector3.Dot((aux.transform.position                - this.transform.position).normalized, transform.forward))
                    {
                        Debug.Log("atualizando o mais na frente");
                        aux = interactingObjs[i];
                    }
                    Debug.Log(aux.name);
                }
                
                canInteract = false;
                aux.Interact(this);
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
            GameManager.gameManager.dialogueController.ChooseOption(null, null, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameManager.gameManager.dialogueController.ChooseOption(null, null, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameManager.gameManager.dialogueController.ChooseOption(null, null, 2);
        }

        if (inBattle)
        {
            if (Input.GetMouseButtonDown(2))
            {
                //LockAim();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))//GetMouseButtonDown(1))
            {
                if (!autoShooting)
                {
                    defending = true;
                    //stamina_regen /= 2;
                    animator.SetBool("Defending", true);
                    if (!slowMoving) StartCoroutine(Slowdown(defaultSlow));
                    //GameManager.gameManager.MainHud.ShowHideDefenseBar();
                    shieldControl.gameObject.SetActive(true);
                    UpdateDefense(0);
                }
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                if (defending) CancelDefense();
            }
            if (defending) defense_parryTimer += Time.deltaTime;
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
        CancelSlow();
    }

    void RunSwitch(bool value)
    {
        if (value)
        {
            running = value;
            acceleratedSpeed = moveSpeed;
            moveSpeed = runningSpeed;
        }
        else if (!value && running)
        {
            running = value;
            moveSpeed = acceleratedSpeed;
        }
    }

    void Move()
    {
        //if (!moving)
        //{
        //    //StopCoroutine("WalkSFX");
        //    StartCoroutine(WalkSFX());
        //}
        moving = true;        
        moveTime += Time.deltaTime;
        if (moveTime < accelerationTime) moveSpeed += acceleration * Time.deltaTime;
        else if (!running && moveSpeed < maxSpeed && !slowMoving) moveSpeed = maxSpeed;
        moveSpeed = Mathf.Clamp(moveSpeed, 0, runningSpeed);

        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 rightMov = right * xMov;
        Vector3 upMov = forward * zMov;

        heading = Vector3.Normalize(rightMov + upMov);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, heading, rotateSpeed * Time.deltaTime, 0);

        RaycastHit hit;
        movingTowardWall = 1;
        if (Physics.Raycast(transform.up * 0.2f + transform.position, heading, out hit, 0.5f, dashMask)) movingTowardWall = 0;

        transform.position += heading * moveSpeed * Time.deltaTime * movingTowardWall;
        cc.Move(heading * moveSpeed * Time.deltaTime);

        if (aimLocked)
        {
            if (targetedEnemy != null)
            {
                battleAim = new Vector3(targetedEnemy.transform.position.x, transform.position.y, targetedEnemy.transform.position.z);
            }
            else LockAim();
        }

        CheckLook_WalkDir(heading, xMov, zMov);

        if (!aimingThrowable && !placingItem) transform.LookAt(battleAim);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (dashQuant > 0 && battleUnlocked)
            {
                Vector3 targetPos = GetDashDest();
                StartCoroutine(Dash(targetPos));
                UpdateDashQuant(-1);
            }
        }  
    }
    IEnumerator WalkSFX()
    {
        yield return new WaitForEndOfFrame();
        while (true)
        {           
            if (moving) GameManager.gameManager.audioController.PlayEffect(clip_walkGrass, false, 0, true);
            yield return new WaitForSeconds(walkSFXInterval);
        }
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

    Vector3 GetDashDest(bool forward = true, float dis = 0.0f)
    {
        RaycastHit hit;
        Vector3 targetPos;
        Vector3 dir;
        if (forward) dir = (moving) ? heading : transform.forward;
        else dir = -transform.forward;
        float dist = (dis > 0) ? dis : dashDistance;

        if (Physics.Raycast(transform.up * 0.2f + transform.position, dir, out hit, dist, dashMask))
        {

            targetPos = (hit.collider.bounds.ClosestPoint(transform.position) - transform.position) * 0.8f + transform.position;
        }
        else targetPos = dir * /*7.5f*/dist + transform.position;

        return targetPos;
    }

    public void DamageDash(float damage)
    {
        StartCoroutine(Dash(GetDashDest(), true, damage));
    }
    IEnumerator Dash(Vector3 dir, bool doDamage = false, float damage = 0.0f, float dis = 0.0f)
    {
        dashing = true;
        Vector3 originPos = transform.position;
        float distance = (dir - transform.position).magnitude;
        float maxDis = (dis > 0) ? dis : dashDistance;
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
        else GameManager.gameManager.audioController.PlayEffect(clip_dash);

        animator.SetBool("Dashing", true);
        float timer = 0.0f;
        do
        {
            //transform.position += dir * dashSpeed * Time.deltaTime;
            //timer += Time.deltaTime;
            timer += Time.deltaTime / dashTime * /*7.5f*/dashDistance / distance;
            transform.position = Vector3.Lerp(originPos, dir, timer);
            yield return new WaitForEndOfFrame();
        } while (timer < 1 && dashing);
        dashing = false;
        //dashingDoingDamage = false;
        animator.SetBool("Dashing", false);
    }

    void CancelDefense()
    {
        defending = false;
        animator.SetBool("Defending", false);
        CancelSlow();
        if (defense_life > 0) shieldControl.gameObject.SetActive(false);
        defense_parryTimer = 0.0f;
    }

    void CheckLook_WalkDir(Vector3 moveDir, float xmov, float zmov)
    {
        float auxDot = Vector3.Dot(transform.forward, moveDir);
        float dirX = 0;
        float dirZ = 0;

        if (auxDot > 0.5f)
        {
            dirZ = 1;
            CancelSlow();
        }
        else if (auxDot > -0.5f)
        {
            CancelSlow();//slowMoving = false;
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

        INPC[] allEnemies;
        if (!GameManager.gameManager.battleController.ActiveBattle)
            allEnemies = GameManager.gameManager.dialogueController.GetNearbyNPCs(transform.position, 10);
        else
            allEnemies = GameManager.gameManager.battleController.AllEnemyFighters.ToArray();

        if (allEnemies != null && allEnemies.Length > 0)
        {
            if (Physics.Raycast(ray, out hit, 1000, aimLayermask))
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
            //Invoke("ResetAnim", /*0.2f*/attackCD);

            StopCoroutine("ResetAnim");
            StartCoroutine(ResetAnim(0.55f));
        }
    }
    IEnumerator ResetAnim(float t)
    {
        yield return new WaitForSeconds(t);
        animator.SetInteger("Attacking", 0);
    }

    void AttackCooldown()
    {
        attacking = false;
        strongAtkTimer = 0;       
    }

    //////////////////////////////////////////////
    public void EquipWeapon(WeaponConfig wconfig, int id)
    {
        Debug.Log("EquippingWeapon");
        //if (wconfig is MeleeConfig)
        //{
        //    equippedMelee.Equip(wconfig);
        //}
        //else
        //{
        //    equippedRanged.Equip(wconfig);
        //}

        if (id == 0)
        {
            if (wconfig is MeleeConfig)
            {
                weapon_melee.Equip(wconfig);
            }
            else
            {
                weapon_extraRanged.Equip(wconfig);
            }
            weapon_melee.isEquipped       =  (wconfig is MeleeConfig);
            weapon_extraRanged.isEquipped = !(wconfig is MeleeConfig);
        }
        else
        {
            if (wconfig is RangedConfig)
            {
                weapon_ranged.Equip(wconfig);
            }
            else
            {
                weapon_extraMelee.Equip(wconfig);
            }
            weapon_ranged.isEquipped     =  (wconfig is RangedConfig);
            weapon_extraMelee.isEquipped = !(wconfig is RangedConfig);
        }
        if (!myWeapon.isEquipped) SwitchMainWeapon();
    }
    void SwitchMainWeapon()
    {
        for (int i = 0; i < 4; i++)
        {
            if (weapons[i].isEquipped)
            {
                if (!isWeaponHide)
                {
                    myWeapon.EnableModel(false);
                    weapons[i].EnableModel(true);
                }
                myWeapon = weapons[i];
            }
        }
    }

    public void EquipOriginalWeapon(int slotID)
    {
        //if (isRanged) equippedRanged.Equip(originalRconfig);
        //else equippedMelee.Equip(originalMconfig);

        //if (slotID == 0) weapon_melee.Equip(weaponConfig_default);
        //else weapon_melee.Equip(weaponConfig_default);
        EquipWeapon(weaponConfig_default, slotID);
    }

    void SwitchWeapon()
    {
        if (!isWeaponHide)
        {
            myWeapon.EnableModel(false);
            for (int i = 0; i < 4; i++)
            {
                if (weapons[i].isEquipped)
                {
                    if (!weapons[i].WeaponConfig.weaponName.Equals(myWeapon.WeaponConfig.weaponName))
                    {
                        //Debug.Log(weapons[i].WeaponConfig.weaponName + " | " + myWeapon.WeaponConfig.weaponName);
                        //Debug.Log(weapons[i].WeaponConfig.weaponName.Equals(myWeapon.WeaponConfig.weaponName));
                        myWeapon = weapons[i];
                        myWeapon.EnableModel(true);
                        return;
                    }
                }
            }
        }
    }

    void HideShowWeapon()
    {
        if (!animator.GetBool("Healing"))
        {
            myWeapon.EnableModel(isWeaponHide);
            isWeaponHide = !isWeaponHide;
        }
    }

    public void UseEquippedItem()
    {
        GameManager.gameManager.inventoryController.Inventory.selectedItemSlot.UseItemEffect();
    }

    //Uso de item Arremessavel
    #region Throw Item
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
        Vector3 force = launchTrajectory.GetComponent<LauchTragectory>().MousePos;// - transform.position + Vector3.up * 10;//10
        launchTrajectory.SetActive(false);
        Granade aux = granadeObj.GetComponent<Granade>();
        aux.UnlockCol();
        itemToThrow.Throw(force, aux);
        GameManager.gameManager.inventoryController.Inventory.selectedItemSlot.ConfirmUse();
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
    #endregion

    //Uso de item colocável
    #region Place Item
    public void StartPlaceItem(PlaceableItem pI)
    {
        placingItem = true;
        itemToPlace = pI;
        GameManager.gameManager.objectPlacer.StartPlacingItem(pI.itemToPlace, pI.itemToPlaceMeshonly, aimLayermask);
        HideShowWeapon();
    }
    public void PlaceItem()
    {
        //itemToPlace.Place()
        GameManager.gameManager.objectPlacer.ComfirmPlace(itemToPlace);
        GameManager.gameManager.inventoryController.Inventory.selectedItemSlot.ConfirmUse();
        CancelPlaceItem();
    }
    public void CancelPlaceItem()
    {
        GameManager.gameManager.objectPlacer.CancelPlacing();
        HideShowWeapon();
        placingItem = false;
    }
    #endregion

    public void StartBattle(bool byDialogue = true)
    {
        if (!battleUnlocked) return;
        if (byDialogue)
        {
            interacting = false;
            CombatSet();
        }
        GameManager.gameManager.battleController.MainCharacter = this;        
    }
    public void DelayStartBattle()
    {
        if (!battleUnlocked) return;
        CombatSet();
    }
    void CombatSet()
    {
        InteractingObjs.Clear();
        animator.SetLayerWeight(1, 0);
        inBattle = true;
        if (myCompanions.Count > 0) SetCompanionsBattle(true);//myCompanion.StartBattle();
        RunSwitch(true);
        animator.SetBool("InBattle", true);
    }

    public void EndBattle()
    {
        animator.SetBool("InBattle", false);        
        RunSwitch(false);
        inBattle = false;
        if (myCompanions.Count > 0) SetCompanionsBattle(false);//myCompanion.EndBattle();
        aimLocked = false;
        if (defending)
        {
            CancelDefense();
        }
        defense_life = defense_maxLife;
        UpdateDefense(0);
        playedShieldBreakeSong = false;
    }

    void SetCompanionsBattle(bool v)
    {
        for (int i = 0; i < myCompanions.Count; i++)
        {
            if (v) myCompanions[i].StartBattle();
            else myCompanions[i].EndBattle();
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

    public bool ReceiveDamage(float damage)
    {
        bool aux = charStats.ReceiveDamage(damage);
        if (defending)
        {
            if (defense_parryTimer <= defense_parryGap)
            {
                Parry();
            }
            else
            {
                GameManager.gameManager.audioController.PlayEffect(clip_shieldHit);
                animator.SetBool("DefendedAttack", true);
                Invoke("BackToIdle", 0.1f);
            }
        }
        else
        {
            GameManager.gameManager.audioController.PlayEffect(clip_hit);
            animator.SetBool("Damaging", true);
            Invoke("BackToIdle", 0.1f);
        }
        UpdateLife();
        return aux;
    }
    void BackToIdle()
    {
        animator.SetBool("DefendedAttack", false);
        animator.SetBool("Damaging", false);
    }

    void Parry()
    {
        //animações bla bla bla
        Debug.Log("Parry");
    }

    public void Die()
    {
        GameManager.gameManager.dialogueController.EndDialogue();
        GameManager.gameManager.MainHud.GameOver();
        animator.SetBool("Died", true);
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public Transform GetItemSpawnTransf()
    {
        return rightHand;
    }

    public void Knockback(float dis)
    {
        if (canReceiveKnockback)
        {
            StartCoroutine(Dash(GetDashDest(false, dis), false, 0, dis));
            canReceiveKnockback = false;
            Invoke("ResetReceiveKnockback", 0.1f);
        }
    }
    void ResetReceiveKnockback()
    {
        canReceiveKnockback = true;
    }

    void StartDashCooldown()
    {
        if (dashQuant < dashMaxQuant && !dashInCooldown)
        {            
            StartCoroutine("DashCooldown");
        }
    }
    IEnumerator DashCooldown()
    {
        dashInCooldown = true;
        GameManager.gameManager.MainHud.dashIcon.Cooldown(dashCooldown);
        yield return new WaitForSeconds(dashCooldown);
        dashInCooldown = false;
        UpdateDashQuant(1);        
    }

    public void Heal(float value)
    {
        GameManager.gameManager.audioController.PlayEffect(clip_heal);
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

    void UpdateDashQuant(int value)
    {
        dashQuant += value;
        dashQuant = Mathf.Clamp(dashQuant, 0, dashMaxQuant);
        GameManager.gameManager.MainHud.dashIcon.SetQuant(dashQuant);
        StartDashCooldown();
    }
    void UpdateLife()
    {
        try { GameManager.gameManager.MainCamera.GetComponent<DamageEffect>().density = 1 - charStats.LifePercentage(); }
        catch { }
    }
    public void UpdateDefense(float value)
    {
        defense_life += value;
        defense_life = Mathf.Clamp(defense_life, 0, defense_maxLife);
        if (IsShiledBroken())
        {
            CancelDefense();            
            if (shieldControl.gameObject.activeSelf) shieldControl.StartCoroutine("BreakAnim");
            if (!playedShieldBreakeSong) GameManager.gameManager.audioController.PlayEffect(clip_shieldBreak);
            playedShieldBreakeSong = true;
        }
        if (shieldControl.gameObject.activeSelf) shieldControl.SetShieldValue(defense_life / defense_maxLife);
    }
    public bool IsShiledBroken()
    {
        if (defense_life > 0) return false;
        else return true;
    }

    public void EnableCharController(bool v)
    {
        cc.enabled = v;
    }
}
