using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooteableSpot : Interactives, IDateEvent
{
    [Header("Looteable Spot")]
    public GameObject visualResources;

    public ResourceItem[] possibleItens = new ResourceItem[1];

    public Vector2Int quantRange;

    Collider c;

    bool collected = false;

    public int timeToSpawn;
    CalendarController.Date dateToSpawn;

    private void Start()
    {
        c = GetComponent<Collider>();
        Invoke("SetDate", 0.02f);/////////////////////////////
    }

    public void SetDate()
    {
        //CalendarController.Date d = GameManager.gameManager.calendarController.DateInfo;
        //dateToSpawn = new CalendarController.Date(d.week, d.day, d.hour, d.mins);//Tem q o ver o quando passou em outra cena
        //dateToSpawn.UpdateMin(timeToSpawn);

        CalendarController.Date d = GameManager.gameManager.repairController.FindDateEvent(Name);

        if (d == null) return;

        //if (GameManager.gameManager.repairController.TryAddDateEvent(Name, dateToSpawn))
        //{
        dateToSpawn = d;
        if (d.CompareTo(GameManager.gameManager.calendarController.DateInfo) > -1)
                RemoveResource();
        //}
        if (collected) GameManager.gameManager.repairController.AddActiveDateEvent(this);
    }

    public void CheckDate(CalendarController.Date d)
    {
        if (dateToSpawn.CompareTo(d) < 1) SpawnResource();
    }

    public string GetName()
    {
        return Name;
    }

    public void ForceEvent()
    {
        SpawnResource();
    }

    public override void Interact(Player player)
    {
        int randItem = Random.Range(0, possibleItens.Length);

        int aux = Random.Range(quantRange.x, quantRange.y + 1);

        GameManager.gameManager.MainHud.OpenCloseInventory(true);

        RemoveResource(aux, randItem, player);

        EndInteraction();
    }

    public void SpawnResource()
    {
        visualResources.SetActive(true);

        c.enabled = true;
        collected = false;

        GameManager.gameManager.repairController.RemoveDateEvent(Name);
    }

    public void RemoveResource()
    {
        visualResources.SetActive(false);

        c.enabled = false;

        collected = true;
    }

    public void RemoveResource(int n, int i, Player p)
    {
        StartCoroutine(GenItens(n, i, p));

        RemoveResource();

        //CalendarController.Date d = GameManager.gameManager.calendarController.DateInfo;
        //dateToSpawn = new CalendarController.Date(d.week, d.day, d.hour, d.mins);
        //dateToSpawn.UpdateMin(timeToSpawn);
        CreateNewDate();
        GameManager.gameManager.repairController.RenewDate(Name, dateToSpawn);
        GameManager.gameManager.repairController.AddActiveDateEvent(this);
    }

    void CreateNewDate()
    {
        CalendarController.Date d = GameManager.gameManager.calendarController.DateInfo;
        dateToSpawn = new CalendarController.Date(d.week, d.day, d.hour, d.mins);
        dateToSpawn.UpdateMin(timeToSpawn);
    }

    IEnumerator GenItens(int quant, int item, Player p)
    {
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < quant; i++)
        {
            GameManager.gameManager.inventoryController.Inventory.AddItem(possibleItens[item]);
        }

        OnExit(p);
        //this.gameObject.SetActive(false);
    }
}
