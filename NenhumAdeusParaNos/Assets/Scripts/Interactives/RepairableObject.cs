using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairableObject : Interactives
{
    public string objectName;

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
        if (GameManager.gameManager.repairController.TryAddRepair(objectName, dateToBreak))
        {
            if (GameManager.gameManager.repairController.FindRepair(objectName).CompareTo(GameManager.gameManager.calendarController.DateInfo) < 1)
            Break();
        }
        GameManager.gameManager.repairController.AddActiveRepair(this);
    }

    public override void Interact(Player player)
    {
        DesactiveBtp();
        StartCoroutine(DetachAnimation());
        OnExit(player);
    }

    public void Repair(int bonusTime)
    {
        CalendarController.Date d = GameManager.gameManager.calendarController.DateInfo;
        dateToBreak = new CalendarController.Date(d.week, d.day, d.hour, d.mins);
        dateToBreak.UpdateMin(baseLife + bonusTime);
        GameManager.gameManager.repairController.RenewDate(objectName, dateToBreak);
        GameManager.gameManager.repairController.AddActiveRepair(this);
        Attach(true);
    }

    IEnumerator DetachAnimation()
    {
        //Efeito de particula;
        yield return new WaitForSeconds(1);
        Attach(false);
    }

    void Break()
    {
        Debug.Log("Broke");
        broken = true;
        EnableInteraction(true);
        GameManager.gameManager.repairController.RemoveRepair(objectName);
        //Mudar estado de textura/modelo.
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
