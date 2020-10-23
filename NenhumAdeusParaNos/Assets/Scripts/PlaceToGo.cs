using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToGo : MonoBehaviour
{
    [SerializeField] string placeName = "";
    public string PlaceName { get { return placeName; } set { placeName = value; } }

    public Quest questToBeAccepted;
    public bool stopPlayerComp;
    public bool movementComp;
    public bool removeComp;

    public Transform arrivePoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
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
