using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            GameManager.gameManager.inventoryController.Inventory.SaveItems();
            GameManager.gameManager.itemsSaver.BlockChestGen();

            SceneManager.LoadScene(sceneName);
        }
    }
}
