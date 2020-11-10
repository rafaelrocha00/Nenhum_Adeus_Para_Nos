using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CompanyPC : Interactives
{
    [Header("CompanyPC")]
    public Transform camT;
    CamMove cam;
    public GameObject pcScreen;
    public GameObject pcLight;

    bool generated = false;
    bool enqueuedQuests = false;

    public GameObject new_quest_marker;

    public Transform rewardsArea;
    public GameObject rewardRankP;

    public Transform map;

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
    public Image questDesc_rewardImage;
    public Text questDesc_limit;
    public GameObject questDesc_acceptButton;
    public GameObject questDesc_refuseButton;

    public GameObject blockButtonsImage;
    //public Text questDesc_contractor;

    bool in_progress_quests;

    public void AddQuest(Quest quest)
    {
        GameObject aux = Instantiate(quest_pref, questList_tab, false) as GameObject;        
        aux.GetComponent<QuestButton>().Set(quest, this);
        quest_list.Add(aux.GetComponent<QuestButton>());    
    }

    public void ShowQuestDescription(Quest quest, QuestButton questB)
    {
        if (blockButtonsImage == null) blockButtonsImage = questDesc_window.transform.Find("block_buttons").gameObject;

        questDesc_window.SetActive(true);
        actualQuestB = questB;
        if (!quest.Accepted)
        {
            questDesc_acceptButton.SetActive(true);
            questDesc_refuseButton.SetActive(true);
            blockButtonsImage.SetActive(false);
        }
        else
        {
            questDesc_acceptButton.SetActive(false);
            questDesc_refuseButton.SetActive(false);
            blockButtonsImage.SetActive(true);
            questB.SetAccepted();
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
        questDesc_reward.text =/* quest.quest_itemRewards[0].itemName + */"x" + quest.quest_itemRQuants[0].ToString();
        if (questDesc_rewardImage == null) questDesc_rewardImage = questDesc_window.transform.Find("reward_image").GetComponent<Image>();
        questDesc_rewardImage.sprite = quest.quest_itemRewards[0].itemSprite;

        questDesc_limit.text = GameManager.gameManager.calendarController.DaysOfWeek[quest.LimitDay] + " 23:59";
        //questDesc_contractor.text = quest.Contractor;
    }
    public QuestButton NextQuest()
    {
        int aux = quest_list.FindIndex(x => x.transform.position.Equals(actualQuestB.transform.position));
        if (aux + 1 >= quest_list.Count) return quest_list[aux - 1];
        else return quest_list[aux + 1];
    }

    public void AcceptQuest()
    {
        actualQuestB.AcceptQuest();
    }

    public void RefuseQuest()
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
    #region Items
    public Storage mainStorage;
    public Transform items_holder;
    public GameObject item_inpc_pref;

    public void SeeStoragedItems()
    {
        mainStorage.GenerateSlots(true);
        StartCoroutine(WaitToSeeItems());
    }
    IEnumerator WaitToSeeItems()
    {
        yield return new WaitForEndOfFrame();
        Dictionary<Item, int> items_quants = mainStorage.GetItemsQuants();

        for (int i = items_holder.childCount - 1; i >= 0; i--)
        {
            Destroy(items_holder.GetChild(i).gameObject);
        }

        for (int i = 0; i < items_quants.Count; i++)
        {
            KeyValuePair<Item, int> item_quant = Enumerable.ElementAt(items_quants, i);

            GameObject aux = Instantiate(item_inpc_pref, items_holder, false) as GameObject;
            aux.transform.GetChild(0).GetComponent<Image>().sprite = item_quant.Key.itemSprite;
            aux.transform.GetChild(1).GetComponent<Text>().text = "x" + item_quant.Value.ToString();
        }

    }
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
                if (canInteract) Invoke("CheckQuests", 0.05f);
                return;
            }
            if (onlyCompletedQuest)
            {
                canInteract = triggerQuest.Completed;
                if (canInteract) Invoke("CheckQuests", 0.05f);
                return;
            }

            //if (triggerQuest.Accepted && !triggerQuest.Completed) canInteract = true;
            canInteract = triggerQuest.Accepted && !triggerQuest.Completed;
        }

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
        Debug.Log("Checando quests no pc");

        if (GameManager.gameManager.questGenerator.quest_queue.Count > 0 || GameManager.gameManager.companyController.quests_onPC.Count > 0)
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
        SeeStoragedItems();

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
        Debug.Log("Ativando icone de quest: " + value);

        if (new_quest_marker == null)
        {
            new_quest_marker = Instantiate(GameManager.gameManager.questController.new_quest_marker_pref, GameManager.gameManager.MainHud.popUpsHolder, false) as GameObject;
            new_quest_marker.GetComponent<ButtonToPress>().SetTransf(transform, 1.75f);
        }

        in_progress_quests = value;
        new_quest_marker.SetActive(value);
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

    private void OnEnable()
    {
        if (in_progress_quests && new_quest_marker != null) new_quest_marker.SetActive(true);

        Invoke("AddQuestEvent", 0.005f);
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
        if (new_quest_marker != null) new_quest_marker.SetActive(false);
    }

    public void MapZoom(float value)
    {
        if ((value > 1 && map.localScale.x >= 10) || (value <= 1 && map.localScale.x < 0.6f)) return;
        map.localScale *= value;
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
