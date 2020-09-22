using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public bool sameScene;
    public InteriorQuickTravel iQuickTravel;

    public string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            if (!sameScene)
            {
                GameManager.gameManager.inventoryController.Inventory.SaveItems();
                GameManager.gameManager.itemsSaver.BlockChestGen();

                SceneManager.LoadScene(sceneName);
            }
            else
            {
                if (other.GetComponent<Player>() != null)
                {
                    try { iQuickTravel.Navigate(other.GetComponent<Player>()); }
                    catch { }
                }
            }
        }
    }
}
