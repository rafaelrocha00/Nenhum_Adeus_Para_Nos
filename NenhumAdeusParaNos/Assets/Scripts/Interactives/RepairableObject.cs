using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairableObject : Interactives, IDateEvent
{
    [Header("Repairable Object")]
    //public string objectName;

    public GameObject state_broken_detached;
    public GameObject state_broken;
    public GameObject state_repaired;

    public Item returnableItem;
    //public bool repairByHit = false;

    public int baseLife = 1;
    public bool broken = false;
    Collider c;
    CalendarController.Date dateToBreak;

    public Interactives unlockableInt;

    [Header("Repair by Hit")]
    public bool waitingToHit = false;
    int bonusT;
    public int life = 0;
    public int maxLife = 0;

    Animator anim;

    [Header("Repair Quests")]
    public bool genRepairQuest = true;
    public bool unbreakableByTime = false;
    public Quest condToBreak;

    private void Start()
    {
        c = GetComponent<Collider>();
        Invoke("SetDate", 0.02f);/////////////////////////////
        anim = GetComponent<Animator>();
    }

    public override void CheckForQuestObjectives(Quest q_)
    {
        //if (GameManager.gameManager.BattleUnlocked) repairByHit = true;

        base.CheckForQuestObjectives(q_);

        if ((q_ is RepairQuest))
        {

            RepairQuest q = (RepairQuest)q_;
            if (q.ObjectToRepair.Equals(Name))
            {
                SpawnQuestMarker();
                active_quest = q;
                return;
            }
        }
        else if (q_ is BreakQuest)
        {
            BreakQuest q = (BreakQuest)q_;
            if (q.objectToBreak.Equals(Name))
            {
                SpawnQuestMarker();
                active_quest = q;
                return;
            }
        }
    }

    public void SetDate()
    {
        if (unbreakableByTime)
        {
            if (condToBreak.Accepted && !condToBreak.Completed) Break();
            else if (condToBreak.Completed) Repair(0);
            GameManager.gameManager.repairController.AddActiveDateEvent(this);
            return;
        }

        //CalendarController.Date d = GameManager.gameManager.calendarController.DateInfo;
        //dateToBreak = new CalendarController.Date(d.week, d.day, d.hour, d.mins);
        //dateToBreak.UpdateMin(baseLife);
        CreateNewDate();
        if (GameManager.gameManager.repairController.TryAddDateEvent(Name, dateToBreak))
        {
            CalendarController.Date d = GameManager.gameManager.repairController.FindDateEvent(Name);
            dateToBreak = d;

            if (dateToBreak.CompareTo(GameManager.gameManager.calendarController.DateInfo) < 1)
                Break();
        }
        if (!broken) GameManager.gameManager.repairController.AddActiveDateEvent(this);

        Debug.Log(dateToBreak.ToString());
    }

    public void CheckDate(CalendarController.Date d)
    {
        if (unbreakableByTime) return;

        if (dateToBreak.CompareTo(d) < 1) Break();
    }

    public string GetName()
    {
        return Name;
    }

    public void ForceEvent()
    {
        Break();
    }

    public override void Interact(Player player)
    {
        DesactiveBtp();
        StartCoroutine(DetachAnimation(false));
        CheckQuest();
    }

    public void Repair(int bonusTime)
    {
        if (!unbreakableByTime)
        {
            //CalendarController.Date d = GameManager.gameManager.calendarController.DateInfo;
            //dateToBreak = new CalendarController.Date(d.week, d.day, d.hour, d.mins);
            //dateToBreak.UpdateMin(baseLife + bonusTime);
            Debug.Log(bonusTime);
            CreateNewDate(bonusTime);
            GameManager.gameManager.repairController.RenewDate(Name, dateToBreak);
            GameManager.gameManager.repairController.AddActiveDateEvent(this);
        }
        broken = false;
        maxLife = state_broken_detached.GetComponent<BrokenObject>().MinCombinedValue;
        life = maxLife;
        if (unlockableInt != null)
        {
            unlockableInt.canInteract = true;
            unlockableInt.EnableCollider(true);
        }
        GameManager.gameManager.questController.CheckQuests(this);

        //if (!repairByHit)
        //{
        //    GameManager.gameManager.battleController.MainCharacter.Repair(state_broken.transform.position);
        //    StartCoroutine(DetachAnimation(true));
        //}
        //else
        //{
        waitingToHit = false;
        //maxLife = 0;
        //life = 0;
        //maxLife = baseLife;
        //life = maxLife;

        state_broken.SetActive(false);
        state_repaired.SetActive(true);
        //}
    }

    void CreateNewDate(int bonusTime = 0)
    {
        CalendarController.Date d = GameManager.gameManager.calendarController.DateInfo;
        dateToBreak = new CalendarController.Date(d.week, d.day, d.hour, d.mins);
        dateToBreak.UpdateMin(baseLife + bonusTime);
    }

    IEnumerator DetachAnimation(bool v)
    {
        //Efeito de particula;
        yield return new WaitForSeconds(v ? 1.5f : 0.5f);
        Attach(v);
        OnExit(GameManager.gameManager.battleController.MainCharacter);
        EndInteraction();
    }

    public void Break(bool byPlayer = false)
    {
        Debug.Log("Broke");
        broken = true;
        //if (!repairByHit) EnableInteraction(true);
        GameManager.gameManager.repairController.RemoveDateEvent(Name);
        if (!byPlayer && genRepairQuest && !AlreadyHasQuest()) GameManager.gameManager.questGenerator.GenRepQuest(Name);
        else if (byPlayer) GameManager.gameManager.questController.CheckQuests(this);
        //Mudar estado de textura/modelo.
        state_repaired.SetActive(false);
        state_broken.SetActive(true);
    }

    bool AlreadyHasQuest()
    {
        Debug.Log("Vendo se tem a quest já");
        return GameManager.gameManager.questGenerator.CheckIfQuestExist(Name) || GameManager.gameManager.companyController.CheckIfQuestExist(Name);
    }

    public void HalfAttach(int bonusTime, int maxValue)
    {
        GameManager.gameManager.MainHud.EnableRepairBar(transform, popUPHigh);
        waitingToHit = true;
        maxLife = maxValue;
        bonusT = bonusTime;

        state_broken.SetActive(true);
        state_broken_detached.SetActive(false);
    }

    void Attach(bool v)
    {
        //state_repaired.SetActive(v);
        //state_broken.SetActive(!v);
        if (v) state_repaired.SetActive(v);
        else state_broken.SetActive(v);
        state_broken_detached.SetActive(!v);
        if (!v /*&& !repairByHit*/) EnableInteraction(false);
    }

    public void EnableInteraction(bool v)
    {
        c.enabled = v;
    }

    public void ReceiveHit(MeleeConfig mconfig)
    {
        Debug.Log(mconfig.weaponName);
        if (!mconfig.weaponName.Equals("Martelo")) return;

        if (waitingToHit)
        {
            //anim.SetTrigger("Hit");
            StartCoroutine(ShakeObject());

            life = Mathf.Clamp(life + 10, 0, maxLife);
            //Debug.Log(life + " | " + maxLife);            

            GameManager.gameManager.MainHud.UpdateRepairBar(life / (float)maxLife);

            if (life == maxLife)
            {
                Repair(bonusT);
                GameManager.gameManager.MainHud.DisableRepairBar();
            }
            return;
        }

        if (broken && state_broken.activeSelf)
        {
            StartCoroutine(ShakeObject());
            Debug.Log("Deatch:");
            StartCoroutine(DetachAnimation(false));
        }
        else if (state_repaired.activeSelf)
        {
            if (maxLife == 0)
            {
                maxLife = state_broken_detached.GetComponent<BrokenObject>().MinCombinedValue;
                life = maxLife;
            }

            StartCoroutine(ShakeObject());

            life = Mathf.Clamp(life -= (int)mconfig.defaultDamage, 0, maxLife);

            Debug.Log(life + " | " + maxLife);

            if (life == 0)
            {
                Break(true);
                //Adicionar ao player metade dos itens necessários para o conserto

                int itemQ = state_broken_detached.GetComponent<BrokenObject>().MinCombinedValue / 20;
                for (int i = 0; i < itemQ; i++)
                {
                    GameManager.gameManager.inventoryController.Inventory.AddItem(returnableItem);
                }
            }
        }
    }

    IEnumerator ShakeObject()
    {
        //yield return new WaitForSeconds(0.05f);
        Vector3 originPos = transform.position;
        float timer = 0.0f;

        Vector3[] points = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            points[i] = originPos + new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.05f, 0.05f), Random.Range(-0.15f, 0.15f));

            while (timer < 0.05f)
            {
                transform.position = Vector3.Lerp(transform.position, points[i], timer / 0.05f);
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            timer = 0.0f;
        }

        transform.position = originPos;
    }
}
