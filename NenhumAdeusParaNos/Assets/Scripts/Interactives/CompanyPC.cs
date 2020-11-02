using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanyPC : Interactives
{
    [Header("CompanyPC")]
    public Transform camT;
    CamMove cam;
    public GameObject pcScreen;
    public GameObject pcLight;

    bool generated = false;
    bool enqueuedQuests = false;

    public GameObject questMarker;

    public Transform rewardsArea;
    public GameObject rewardRankP;

    #region Pc UI
    #region Jobs
    public Transform questList_tab;
    public GameObject quest_pref;
    public List<QuestButton> quest_list = new List<QuestButton>();
    QuestButton actualQuestB;
    public GameObject questDesc_window;
    public Text questDesc_name;
    public Text questDesc_description;
    public Text questDesc_reward;
    public Text questDesc_limit;
    public Text questDesc_contractor;

    public void AddQuest(Quest quest)
    {
        GameObject aux = Instantiate(quest_pref, questList_tab, false) as GameObject;        
        aux.GetComponent<QuestButton>().Set(quest, this);
        quest_list.Add(aux.GetComponent<QuestButton>());    
    }

    public void ShowQuestDescription(Quest quest, QuestButton questB)
    {
        questDesc_window.SetActive(true);
        actualQuestB = questB;
        if (!quest.Accepted)
        {
            quest.AcceptQuest();
        }

        questDesc_name.text = quest.Name;
        questDesc_description.text = quest.Description;
        //string resourceT = "";
        //switch (quest.resourceT)
        //{
        //    case CompanyController.ResourceType.Tool:
        //        resourceT = "Ferramentas";
        //        break;
        //    case CompanyController.ResourceType.Food:
        //        resourceT = "Comidas";
        //        break;
        //    case CompanyController.ResourceType.Med:
        //        resourceT = "Medicamentos";
        //        break;
        //    case CompanyController.ResourceType.Feedstock:
        //        resourceT = "Matéria-Prima";
        //        break;
        //}
        //questDesc_reward.text = quest.MoneyReward.ToString("0.00") + " & " + quest.ResourceReward.ToString() + " " + resourceT;
        questDesc_reward.text = quest.quest_itemRewards[0].itemName + "  x" + quest.quest_itemRQuants[0].ToString();

        questDesc_limit.text = GameManager.gameManager.calendarController.DaysOfWeek[quest.LimitDay];
        questDesc_contractor.text = quest.Contractor;
    }
    public QuestButton NextQuest()
    {
        int aux = quest_list.FindIndex(x => x.transform.position.Equals(actualQuestB.transform.position));
        if (aux + 1 >= quest_list.Count) return quest_list[aux - 1];
        else return quest_list[aux + 1];
    }
    public void CancelQuest()
    {
        questDesc_window.SetActive(false);
        QuestButton next = null;
        if (quest_list.Count > 1) next = NextQuest();
        else Enable_DisableQM(false);

        quest_list.Remove(actualQuestB);
        actualQuestB.CancelQuest();

        if (next != null) next.ShowDesc();
    }
    #endregion
    #region Resources
    //public Text money;
    //public Text[] resourcesQuants = new Text[4];

    //public void SetMoney()
    //{
    //    money.text = GameManager.gameManager.companyController.Money.ToString("0.00");
    //}
    //public void SetResource()
    //{
    //    for (int i = 0; i < 4; i++)
    //    {
    //        resourcesQuants[i].text = GameManager.gameManager.companyController.GetResourceQuant(i).ToString();
    //    }
    //}
    #endregion
    #endregion

    //public Quest[] testquest;

    private void Start()
    {
        if (triggerQuest != null)
        {
            if (onlyAcceptedQuest)
            {
                canInteract = triggerQuest.Accepted;
                return;
            }
            if (onlyCompletedQuest)
            {
                canInteract = triggerQuest.Completed;
                return;
            }

            //if (triggerQuest.Accepted && !triggerQuest.Completed) canInteract = true;
            canInteract = triggerQuest.Accepted && !triggerQuest.Completed;
        }

        if (canInteract) Invoke("CheckQuests", 0.05f);

        //firstEnable = false;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    for (int i = 0; i < testquest.Length; i++)
        //    {
        //        AddQuest(testquest[i]);
        //    }
        //}
    }

    void CheckQuests()
    {
        if (GameManager.gameManager.questGenerator.quest_queue.Count > 0 /*|| GameManager.gameManager.companyController.quests_onPC.Count > 0*/)
            Enable_DisableQM(true);
        else Enable_DisableQM(false);
    }

    public override void Interact(Player player)
    {
        DesactiveBtp();
        //interacted = true;
        if (cam == null) cam = GameManager.gameManager.MainCamera;

        cam.TargetingPlayer = false;
        cam.LerpLoc(camT.position, 1);
        cam.LerpRot(camT.rotation, 1f);
        pcScreen.SetActive(true);
        pcLight.SetActive(true);

        //SetMoney();
        //SetResource();

        GenerateJobs();
        CheckQuest();
        CheckQuests();

        GameManager.gameManager.MainHud.EnterPC(true, this);

        //SetRanking();
    }

    public void Exit()
    {
        ExitPC(GameManager.gameManager.battleController.MainCharacter);

        EndInteraction();
    }

    public void ExitPC(Player p)
    {
        base.OnExit(p);

        cam.TargetingPlayer = true;
        cam.LerpRot(cam.DefaultRotation, 1.0f);
        pcScreen.SetActive(false);
        pcLight.SetActive(false);

        Invoke("EnableQMs", 1.0f);
    }
    void EnableQMs()
    {
        GameManager.gameManager.MainHud.EnterPC(false);
    }

    public void Enable_DisableQM(bool value)
    {
        questMarker.SetActive(value);
    }

    public void GenerateJobs()
    {
        if (!generated)
        {
            //Quest[] activeQuests = GameManager.gameManager.questController.activeQuests.ToArray();
            //for (int i = 0; i < activeQuests.Length; i++)
            //{
            //    if (activeQuests[i].generated)
            //    {
            //        AddQuest(activeQuests[i]);
            //    }
            //}
            int queueSize = GameManager.gameManager.companyController.quests_onPC.Count;
            for (int i = 0; i < queueSize; i++)
            {
                Quest q = GameManager.gameManager.companyController.quests_onPC.Dequeue();
                if (!q.Cancelled && !q.Completed) AddQuest(q);

            }

            if (GameManager.gameManager.questGenerator.quest_queue.Count > 0 && quest_list.Count < 9)
            {
                Debug.Log("Trying Dequeue Quests | " + GameManager.gameManager.questGenerator.quest_queue.Count);
                queueSize = GameManager.gameManager.questGenerator.quest_queue.Count;
                for (int i = 0; i < queueSize; i++)
                {
                    if (quest_list.Count < 9)
                    {
                        Debug.Log("Dequeuing");
                        Quest q = GameManager.gameManager.questGenerator.quest_queue.Dequeue();
                        if (q != null) AddQuest(q);
                    }
                }
            }

            generated = true;
        }
    }

    private void OnDisable()
    {
        if (!enqueuedQuests)
        {
            for (int i = 0; i < quest_list.Count; i++)
            {
                GameManager.gameManager.companyController.quests_onPC.Enqueue(quest_list[i].Quest);
            }
            enqueuedQuests = true;
        }

        CustomEvents.instance.onQuestAccepted -= CheckForQuestObjectives;

        if (quest_mark != null) quest_mark.SetActive(false);
    }

    //public void SetRanking()
    //{
    //    //string msg = Client_UDP.Singleton.SendToServer("load(ls)");

    //    if (msg.Contains("(sc)"))
    //    {
    //        //try
    //        //{
    //        //}
    //        //catch { }
    //        for (int i = 0; i < rewardsArea.childCount; i++)
    //        {
    //            Destroy(rewardsArea.GetChild(i).gameObject);
    //        }

    //        string allScores = msg.Split(')')[1];
    //        allScores = allScores.Replace("\n", "*");
    //        print(allScores);

    //        string[] scores = allScores.Split('*');

    //        for (int i = 0; i < scores.Length - 1; i++)
    //        {
    //            print(scores[i]);
    //            GameObject go = Instantiate(rewardRankP, rewardsArea, false) as GameObject;
    //            go.GetComponent<Text>().text = scores[i];
    //        }
    //    }
    //}
}
