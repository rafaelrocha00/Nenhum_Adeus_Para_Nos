using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarController : MonoBehaviour
{
    [HideInInspector] string[] daysOfWeek = { "Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado" };
    public string[] DaysOfWeek { get { return daysOfWeek; } }

    [SerializeField] int actualWeek = 1;
    [SerializeField] int actualDay = 0;
    [SerializeField] int hour = 6;
    [SerializeField] int mins = 0;
    public int ActualWeek { get { return actualWeek; } }
    public int ActualDay { get { return actualDay; } }
    public int Hour { get { return hour; } }
    public float Mins { get { return mins; } }    

    float timer = 0.0f;

    //1 minuto jogo = 3,75 segundos reais

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 3.75f)
        {
            timer = 0.0f;
            UpdateMin();
        }              
    }

    void UpdateHour()
    {
        hour++;
        if (hour == 24)
        {
            UpdateDay();
            hour = 0;
        }
    }

    void UpdateMin()
    {
        mins++;
        if (mins == 60)
        {
            UpdateHour();
            mins = 0;
        }

        DayNightCycle.Instance.UpdatePostProcess(hour + mins / 60);
        Debug.Log(hour + " / " + mins);
    }

    void UpdateDay()
    {
        actualDay++;
        if (actualDay == 7)
        {
            actualWeek++;
            actualDay = 0;
        }
    }
}
