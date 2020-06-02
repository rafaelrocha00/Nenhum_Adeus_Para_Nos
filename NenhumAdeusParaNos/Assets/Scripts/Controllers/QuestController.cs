using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();

    public Notes mainNotes;

    public void AcceptQuest(Quest q)
    {
        activeQuests.Add(q);
    }

    public void CompleteQuest(Quest q)
    {
        try
        {
            //int aux = activeQuests.FindIndex(x => x.Name.Equals(q.Name));
            //activeQuests.RemoveAt(aux);

            //completedQuests.Add(q);
            activeQuests.RemoveAt(FindQuestIndex(q.ID));
            completedQuests.Add(q);
        }
        catch (System.Exception)
        {

            throw;
        }        
    }

    public void CancelQuest(Quest q)
    {
        try
        {
            //int idx = FindQuestIndex(q.Name); 
            //activeQuests[idx].Reset();
            activeQuests.RemoveAt(FindQuestIndex(q.ID));
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public int FindQuestIndex(int _id)
    {
        try
        {
            int aux = activeQuests.FindIndex(x => x.ID.Equals(_id));
            //activeQuests.RemoveAt(aux);
            return aux;
        }
        catch { return -1; }
    }

    public void CheckQuests<T>(T t)
    {
        //foreach (Quest quest in activeQuests)
        //{
        //    quest.CheckComplete(t);
        //}

        for (int i = 0; i < activeQuests.Count; i++)
        {
            activeQuests[i].CheckComplete(t);
        }
    }

    public void CancelQuestsOnLimit()
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].generated && activeQuests[i].LimitDay == GameManager.gameManager.calendarController.ActualDay)
                activeQuests[i].Cancel();
        }
    }

    public void InstantiateQuestObjs(ObjectInstancer oi)
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].generated)
            {
                activeQuests[i].InstantiateObjs(oi);
            }
        }
    }
}
