using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CalendarController : MonoBehaviour
{
    public class Date
    {
        public int week = 1;
        public int day, hour, mins;

        public Date(int w, int d, int h, int m)
        {
            week = 1;
            day = d;
            hour = h;
            mins = m;
        }

        public void UpdateMin(int v = 1)
        {
            mins += v;
            if (mins >= 60)
            {
                int h = mins / 60;
                mins %= 60;
                UpdateHour(h);
            }
        }
        public void UpdateHour(int v = 1)
        {
            hour += v;
            if (hour >= 24)
            {
                int d = hour / 24;
                hour %= 24;
                UpdateDay(d);
            }
        }
        public void UpdateDay(int v = 1)
        {
            day += v;
            if (day >= 7)
            {
                week += day / 7;
                day %= 7;
            }
        }

        public int CompareTo(Date date)
        {
            if (week < date.week) return -1;
            else if (week > date.week) return 1;
            else
            {
                if (day < date.day) return -1;
                else if (day > date.day) return 1;
                else
                {
                    if (hour < date.hour) return -1;
                    else if (hour > date.hour) return 1;
                    else
                    {
                        if (mins < date.mins) return -1;
                        else if (mins > date.mins) return 1;
                        else return 0;
                    }
                }
            }
        }

        public override string ToString()
        {
            return week + " | " + day + " | " + hour + " | " + mins;
        }
    }

    [HideInInspector] string[] daysOfWeek = { "Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado" };
    public string[] DaysOfWeek { get { return daysOfWeek; } }

    [SerializeField] Date date;
    public Date DateInfo { get { return date; } }

    float timer = 0.0f;

    //1 minuto jogo = 3,75 segundos reais
    public float timeScale = 3.75f;

    private void Start()
    {
        date = new Date(1, 0, 6, 0);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeScale)
        {
            timer = 0.0f;
            if (SceneManager.GetActiveScene().buildIndex > 1) UpdateMin();
        }

        if (Input.GetKeyDown(KeyCode.G)) UpdateMin(480);
        //if (Input.GetKeyDown(KeyCode.H)) UpdateMin(1000);
        //if (Input.GetKeyDown(KeyCode.J)) UpdateMin(1440);
    }

    public void PassTime(int mins/*, int hours, int days*/)
    {
        UpdateMin(mins);

        DayNightCycle.Instance.UpdatePostProcess(date.hour + mins / 60);
        Debug.Log(date.hour + " / " + mins);
        UpdateHudH();
    }

    void UpdateHour()
    {
        date.UpdateHour();
        if ((date.hour == 6) || (date.hour == 18))
        {
            for (int i = 0; i < 2; i++)
            {
                GameManager.gameManager.questGenerator.GenQuest();
            }
        }
    }

    void UpdateMin(int v = 1)
    {
        date.UpdateMin(v);

        DayNightCycle.Instance.UpdatePostProcess(date.hour + date.mins / 60);
        GameManager.gameManager.repairController.CheckIfActiveRepairBroke(date);
        //Debug.Log(date.hour + " / " + date.mins);
        UpdateHudH();
    }

    void UpdateDay()
    {
        GameManager.gameManager.questController.CancelQuestsOnLimit();
        date.UpdateDay();       
    }

    public void UpdateHudH()
    {
        try { GameManager.gameManager.MainHud.UpdateDate(daysOfWeek[date.day], date.hour, date.mins); }
        catch { Debug.Log("Hud Null"); }
    }
}
