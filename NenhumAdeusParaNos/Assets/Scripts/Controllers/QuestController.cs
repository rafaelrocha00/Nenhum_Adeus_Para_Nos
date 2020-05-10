using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();

    public void AcceptQuest(Quest q)
    {
        activeQuests.Add(q);
    }

    public void CompleteQuest(Quest q)
    {
        try
        {
            int aux = activeQuests.FindIndex(x => x.Name.Equals(q.Name));
            activeQuests.RemoveAt(aux);

            completedQuests.Add(q);
        }
        catch (System.Exception)
        {

            throw;
        }        
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
}
