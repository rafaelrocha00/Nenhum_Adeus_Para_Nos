using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairController : MonoBehaviour
{
    public Dictionary<string, CalendarController.Date> repairs = new Dictionary<string, CalendarController.Date>();
    public List<RepairableObject> activeRepairs = new List<RepairableObject>();

    private void Start()
    {
        OnLoadScene();
    }

    public CalendarController.Date FindRepair(string n)
    {
        return repairs[n];
    }

    public bool TryAddRepair(string n, CalendarController.Date date)
    {
        if (!repairs.ContainsKey(n))
        {
            repairs.Add(n, date);
            return false;
        }
        else return true;
    }

    public void RenewDate(string n, CalendarController.Date newDate)
    {
        CalendarController.Date d = repairs[n];
        d = newDate;
    }

    public void AddActiveRepair(RepairableObject r)
    {
        activeRepairs.Add(r);
    }
    public void RemoveRepair(string name)
    {
        int i = activeRepairs.FindIndex(x => x.Name.Equals(name));
        if (i >= 0) activeRepairs.RemoveAt(i);
    }

    public void CheckIfActiveRepairBroke(CalendarController.Date d)
    {
        for (int i = 0; i < activeRepairs.Count; i++)
        {
            activeRepairs[i].CheckDate(d);
        }
    }

    public void OnLoadScene()
    {
        Debug.Log("Limpando Consertos ativos");
        activeRepairs.Clear();
    }
}
