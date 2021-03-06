﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    public Text quest_name;
    [HideInInspector] Quest quest = null;
    public Quest Quest { get { return quest; } }
    CompanyPC pc = null;

    public Image jobStateIcon;
    public Sprite jobAcceptedIcon;

    public void Set(Quest _quest, CompanyPC _pc)
    {
        quest = _quest;
        quest_name.text = quest.Name;
        pc = _pc;
    }

    public void ShowDesc()
    {
        pc.ShowQuestDescription(quest, this);
    }

    public void AcceptQuest()
    {
        SetAccepted();
        quest.AcceptQuest();
    }

    public void CancelQuest()
    {
        //quest.Cancel();
        Destroy(gameObject);
    }

    public void SetAccepted()
    {
        jobStateIcon.sprite = jobAcceptedIcon;
    }

    public void ChangeCursor(int state)
    {
        GameManager.gameManager.ChangeCursor(state);
    }
}
