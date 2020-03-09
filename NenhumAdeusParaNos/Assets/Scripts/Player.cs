using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, BattleUnit
{
    [HideInInspector] CharacterStats charStats;
    public CharacterStats CharStats { get { return charStats; } }

    public float defaultSpeed = 3.5f;
    public float maxSpeed = 6.0f;
    public float runningSpeed = 9.0f;
    public float rotateSpeed = 1.0f;
    public float acceleration = 3.0f;
    public float accelerationTime = 1.0f;

    public MeleeW myMelee;

    public float moveSpeed;
    float acceleratedSpeed;
    Vector3 forward, right;

    float moveTime = 0.0f;
    bool running;

    public float strongAtkHoldTime = 0.7f;
    float strongAtkTimer = 0.0f;
    bool releasedAtk = false;
    public float defaultAttackCD = 0.5f;
    public float strongAttackCD = 1.0f;
    float attackCD;
    int strongAtk = 1;

    bool attacking;

    CamMove cam;

    [HideInInspector] bool canInteract;
    public bool CanInteract { get { return canInteract; } set { canInteract = value; } }
    [HideInInspector] Interactives interactingObj;
    public Interactives InteractingObj { get { return interactingObj; } set { interactingObj = value; } }

    bool inBattle;
    LayerMask layermask = 1 << 0;

    private void Start()
    {
        charStats = new CharacterStats(this);

        moveSpeed = defaultSpeed;

        forward = Camera.main.transform.forward;
        cam = Camera.main.GetComponent<CamMove>();
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire3")) RunSwitch(true);
        if (Input.GetButtonUp("Fire3")) RunSwitch(false);


        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Move();
        }
        else
        {
            if (!running) moveSpeed = defaultSpeed;
            moveTime = 0.0f;
        }

        if (inBattle)
        {
            if (Input.GetMouseButton(0))
            {
                if (!releasedAtk)
                {
                    strongAtkTimer += Time.deltaTime;
                    if (strongAtkTimer >= strongAtkHoldTime)
                    {
                        releasedAtk = true;
                        //Debug.Log("AtaqueForte");
                        strongAtk = 2;
                        attackCD = strongAttackCD;

                        Attack();
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log(strongAtkTimer);
                if (strongAtkTimer < strongAtkHoldTime && !releasedAtk)
                {
                    //Debug.Log("AtaqueFraco");
                    strongAtk = 1;
                    attackCD = defaultAttackCD;

                    Attack();
                }
                releasedAtk = false;
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

            if (Physics.Raycast(ray, out hit, 1000, layermask))
            {                
                Vector3 lookPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                transform.LookAt(lookPos);
                //Debug.Log(lookPos);
            }            
        }
        //cam.Move(transform.position);
    }

    void Attack()
    {        
        if (!attacking)
        {
            //Debug.Log("Atacando");
            attacking = true;
            myMelee.Attack(attackCD, strongAtk);            
            Invoke("AttackCooldown", attackCD);
        }
    }

    void AttackCooldown()
    {
        attacking = false;
        strongAtkTimer = 0;       
    }

    public void StartBattle()
    {
        inBattle = true;
        GameManager.gameManager.battleController.MainCharacter = this;
    }

    public void EndBattle()
    {
        inBattle = false;
    }

    public bool CanFight()
    {
        return charStats.CanFight;
    }
}
