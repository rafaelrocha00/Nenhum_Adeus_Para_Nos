using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToGo : MonoBehaviour
{
    [SerializeField] string placeName = "";
    public string PlaceName { get { return placeName; } set { placeName = value; } }

    GameObject quest_mark;

    public Quest questToBeAccepted;
    public bool stopPlayerComp;
    public bool movementComp;
    public bool removeComp;

    public Transform arrivePoints;

    private void Start()
    {
        CustomEvents.instance.onQuestAccepted += CheckForQuestObjectives;
    }

    private void OnDestroy()
    {
        CustomEvents.instance.onQuestAccepted -= CheckForQuestObjectives;
    }

    public void CheckForQuestObjectives(Quest q_)
    {
        if (!(q_ is ArriveQuest)) return;

        ArriveQuest q = (ArriveQuest)q_;
        if (q.placeToGoName.Equals(placeName)) SpawnQuestMarker(); 
    }

    void SpawnQuestMarker()
    {
        if (quest_mark != null)
        {
            quest_mark.SetActive(true);
            return;
        }

        quest_mark = Instantiate(GameManager.gameManager.questController.quest_marker_pref, GameManager.gameManager.MainHud.popUpsHolder, false) as GameObject;
        quest_mark.GetComponent<ButtonToPress>().SetTransf(transform, 1.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            if (quest_mark != null && quest_mark.activeSelf) quest_mark.SetActive(false);

            if (stopPlayerComp && questToBeAccepted.Accepted && !questToBeAccepted.Completed)
            {
                try
                {
                    Player p = other.GetComponent<Player>();
                    if (stopPlayerComp)
                    {
                        INPC[] instancedNPCs = p.DespawnCompanions();
                        if (movementComp)
                        {
                            Debug.Log("Movendo");
                            for (int i = 0; i < instancedNPCs.Length; i++)
                            {
                                Debug.Log("Movendo Novos Npcs para o local");
                                instancedNPCs[i].MoveNavMesh(arrivePoints.GetChild(i).position);
                            }
                        }

                        if (removeComp) p.RemoveCompanions();
                    }
                }
                catch { }
            }

            Debug.Log("Player chegou em " + placeName);
            GameManager.gameManager.questController.CheckQuests(this);
        }
    }
}
