using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToGo : MonoBehaviour
{
    [SerializeField] string placeName = "";
    public string PlaceName { get { return placeName; } set { placeName = value; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            Debug.Log("Player chegou em " + placeName);
            GameManager.gameManager.questController.CheckQuests(this);
        }
    }
}
