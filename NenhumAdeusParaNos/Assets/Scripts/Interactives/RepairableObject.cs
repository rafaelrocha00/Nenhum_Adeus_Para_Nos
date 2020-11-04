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

    public int baseLife = 1;
    public bool broken = false;
    Collider c;
    CalendarController.Date dateToBreak;

    public Interactives unlockableInt;

    [Header("Repair Quests")]
    public bool genRepairQuest = true;
    public bool unbreakableByTime = false;
    public Quest condToBreak;

    private void Start()
    {
        c = GetComponent<Collider>();
        Invoke("SetDate", 0.02f);/////////////////////////////
    }

    public override void CheckForQuestObjectives(Quest q_)
    {
        base.CheckForQuestObjectives(q_);

        if (!(q_ is RepairQuest)) return;

        RepairQuest q = (RepairQuest)q_;
        if (q.ObjectToRepair.Equals(Name))
        {
            SpawnQuestMarker();
            active_quest = q;
            return;
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
        if (unlockableInt != null)
        {
            unlockableInt.canInteract = true;
            unlockableInt.EnableCollider(true);
        }
        GameManager.gameManager.questController.CheckQuests(this);

        GameManager.gameManager.battleController.MainCharacter.Repair(state_broken.transform.position);
        StartCoroutine(DetachAnimation(true));
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

    public void Break()
    {
        Debug.Log("Broke");
        broken = true;
        EnableInteraction(true);
        GameManager.gameManager.repairController.RemoveDateEvent(Name);
        if (genRepairQuest && !AlreadyHasQuest()) GameManager.gameManager.questGenerator.GenRepQuest(Name);
        //Mudar estado de textura/modelo.
        state_repaired.SetActive(false);
        state_broken.SetActive(true);
    }

    bool AlreadyHasQuest()
    {
        Debug.Log("Vendo se tem a quest já");
        return GameManager.gameManager.questGenerator.CheckIfQuestExist(Name) || GameManager.gameManager.companyController.CheckIfQuestExist(Name);
    }

    void Attach(bool v)
    {
        //state_repaired.SetActive(v);
        //state_broken.SetActive(!v);
        if (v) state_repaired.SetActive(v);
        else state_broken.SetActive(v);
        state_broken_detached.SetActive(!v);
        if (!v) EnableInteraction(false);
    }

    public void EnableInteraction(bool v)
    {
        c.enabled = v;
    }
}
