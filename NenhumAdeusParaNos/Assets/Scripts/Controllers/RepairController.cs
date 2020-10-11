using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairController : MonoBehaviour
{
    public Dictionary<string, CalendarController.Date> dateEvents = new Dictionary<string, CalendarController.Date>();
    public List<IDateEvent> activeDateEvents = new List<IDateEvent>();

    private void Start()
    {
        OnLoadScene();
    }

    public CalendarController.Date FindDateEvent(string n)
    {
        try { return dateEvents[n]; }
        catch { return null; }
    }

    public bool TryAddDateEvent(string n, CalendarController.Date date)
    {
        if (!dateEvents.ContainsKey(n))
        {
            dateEvents.Add(n, date);
            return false;
        }
        else return true;
    }

    public void RenewDate(string n, CalendarController.Date newDate)
    {
        //CalendarController.Date d = dateEvents[n];
        //d = newDate;
        dateEvents[n] = newDate;
    }

    public void AddActiveDateEvent(IDateEvent d)
    {
        activeDateEvents.Add(d);
    }
    public void RemoveDateEvent(string name)
    {
        //int i = activeDateEvents.FindIndex(x => x.Name.Equals(name));
        int i = activeDateEvents.FindIndex(x => x.GetName().Equals(name));
        if (i >= 0) activeDateEvents.RemoveAt(i);
    }

    public void CheckDateEvent(CalendarController.Date d)
    {
        for (int i = 0; i < activeDateEvents.Count; i++)
        {
            activeDateEvents[i].CheckDate(d);
        }
    }

    public void ForceDateEvent(string name)
    {
        //int i = activeDateEvents.FindIndex(x => x.Name.Equals(name));
        int i = activeDateEvents.FindIndex(x => x.GetName().Equals(name));
        if (i >= 0) activeDateEvents[i].ForceEvent();
    }

    public void OnLoadScene()
    {
        Debug.Log("Limpando Consertos ativos");
        activeDateEvents.Clear();
    }
}
