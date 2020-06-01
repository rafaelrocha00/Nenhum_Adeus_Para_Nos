using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float timeScale = 3.75f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeScale)
        {
            timer = 0.0f;
            UpdateMin();
        }              
    }

    public void PassTime(int mins, int hours, int days)
    {
        for (int i = 0; i < mins; i++)
        {
            UpdateMin();
        }
        for (int i = 0; i < hours; i++)
        {
            UpdateHour();
        }
        for (int i = 0; i < days; i++)
        {
            UpdateDay();
        }

        DayNightCycle.Instance.UpdatePostProcess(hour + mins / 60);
        Debug.Log(hour + " / " + mins);
        UpdateHudH();
    }

    void UpdateHour()
    {
        hour++;
        if (hour == 24)
        {
            hour = 0;
            UpdateDay();          
        }
        if ((hour == 6) || (hour == 18))
        {
            for (int i = 0; i < 3; i++)
            {
                GameManager.gameManager.questGenerator.GenQuest();
            }
        }
    }

    void UpdateMin()
    {
        mins++;
        if (mins == 60)
        {
            mins = 0;
            UpdateHour();
        }

        DayNightCycle.Instance.UpdatePostProcess(hour + mins / 60);
        Debug.Log(hour + " / " + mins);
        UpdateHudH();
    }

    void UpdateDay()
    {
        GameManager.gameManager.questController.CancelQuestsOnLimit();
        actualDay++;
        if (actualDay == 7)
        {
            actualWeek++;
            actualDay = 0;
        }        
    }

    public void UpdateHudH()
    {
        GameManager.gameManager.MainHud.UpdateDate(daysOfWeek[actualDay], hour, mins);
    }
}
