using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public struct NoteContent
//{
//    public int id;
//    public string[] content;
//}

public class QuestController : MonoBehaviour
{
    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();

    public Notes mainNotes;

    public void AddNote(string txt)
    {
        try
        {
            mainNotes.AddNote(txt);
        }
        catch
        {
            try
            {
                InvenSlot ivSlot = GameManager.gameManager.inventoryController.Inventory.FindItem("Anotações #1");
                Notes nt = (Notes)ivSlot.ThisItemButton.Item;
                mainNotes = nt;
                mainNotes.AddNote(txt);
            }
            catch { }
        }
    }

    public void AcceptQuest(Quest q)
    {
        activeQuests.Add(q);
    }

    public void CompleteQuest(Quest q)
    {
        try
        {
            activeQuests.RemoveAt(FindQuestIndex(q.ID));
            completedQuests.Add(q);

            //Enviar pro servidor o nome do player e a recompensa
            if (q.generated) Client_UDP.Singleton.SendToServer("Jogador: " + GameManager.gameManager.PlayerName + " | Maior Recompensa: $" + q.MoneyReward.ToString("00") + "(s)");
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
            return aux;
        }
        catch { return -1; }
    }

    public void CheckQuests<T>(T t)
    {
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
