using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDateEvent
{
    void SetDate();

    void CheckDate(CalendarController.Date d);

    void ForceEvent();

    string GetName();
}
