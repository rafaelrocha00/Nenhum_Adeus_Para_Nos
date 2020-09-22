using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairableObject : Interactives
{
    //public string objectName;

    public GameObject state_broken;
    public GameObject state_repaired;

    public int baseLife = 1;
    public bool broken = false;
    Collider c;
    CalendarController.Date dateToBreak;

    private void Start()
    {
        c = GetComponent<Collider>();
        Invoke("SetDate", 0.05f);
    }
    void SetDate()
    {
        CalendarController.Date d = GameManager.gameManager.calendarController.DateInfo;
        dateToBreak = new CalendarController.Date(d.week, d.day, d.hour, d.mins);//Tem q o ver o quando passou em outra cena
        dateToBreak.UpdateMin(baseLife);
        if (GameManager.gameManager.repairController.TryAddRepair(Name, dateToBreak))
        {
            if (GameManager.gameManager.repairController.FindRepair(Name).CompareTo(GameManager.gameManager.calendarController.DateInfo) < 1)
            Break();
        }
        GameManager.gameManager.repairController.AddActiveRepair(this);
    }

    public override void Interact(Player player)
    {
        DesactiveBtp();
        StartCoroutine(DetachAnimation(false));
        CheckQuest();
    }

    public void Repair(int bonusTime)
    {
        CalendarController.Date d = GameManager.gameManager.calendarController.DateInfo;
        dateToBreak = new CalendarController.Date(d.week, d.day, d.hour, d.mins);
        dateToBreak.UpdateMin(baseLife + bonusTime);
        GameManager.gameManager.repairController.RenewDate(Name, dateToBreak);
        GameManager.gameManager.repairController.AddActiveRepair(this);
        broken = false;
        GameManager.gameManager.questController.CheckQuests(this);
        StartCoroutine(DetachAnimation(true));
    }

    IEnumerator DetachAnimation(bool v)
    {
        //Efeito de particula;
        yield return new WaitForSeconds(1);
        Attach(v);
        OnExit(GameManager.gameManager.battleController.MainCharacter);
    }

    void Break()
    {
        Debug.Log("Broke");
        broken = true;
        EnableInteraction(true);
        GameManager.gameManager.repairController.RemoveRepair(Name);
        if (!AlreadyHasQuest()) GameManager.gameManager.questGenerator.GenRepQuest(Name);
        //Mudar estado de textura/modelo.
    }

    bool AlreadyHasQuest()
    {
        Debug.Log("Vendo se tem a quest já");
        return GameManager.gameManager.questGenerator.CheckIfQuestExist(Name) || GameManager.gameManager.companyController.CheckIfQuestExist(Name);
    }

    void Attach(bool v)
    {
        state_repaired.SetActive(v);
        state_broken.SetActive(!v);
        if (!v) EnableInteraction(false);

    }

    public void EnableInteraction(bool v)
    {
        c.enabled = v;
    }

    public void CheckDate(CalendarController.Date d)
    {
        if (dateToBreak.CompareTo(d) < 1) Break();
    }
}
