using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToGo : MonoBehaviour
{
    [SerializeField] string placeName = "";
    public string PlaceName { get { return placeName; } set { placeName = value; } }

    public Quest questToBeAccepted;
    public bool stopPlayerComp;
    public bool removeComp;

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
                        if (!removeComp) p.DespawnCompanions();
                        else p.RemoveCompanions();
                    }
                }
                catch { }
            }

            Debug.Log("Player chegou em " + placeName);
            GameManager.gameManager.questController.CheckQuests(this);
        }
    }
}
