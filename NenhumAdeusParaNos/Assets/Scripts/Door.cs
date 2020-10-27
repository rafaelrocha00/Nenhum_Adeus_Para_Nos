using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public bool sameScene;
    public int spawnpointID = -1;
    public SceneNav iQuickTravel;

    public string sceneName;

    public Quest quesToBeCompleted;
    public Quest quesToAccept;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("npc") && !other.isTrigger) Destroy(other.gameObject);

        if (quesToAccept != null && !quesToAccept.Accepted && quesToBeCompleted.Completed) quesToAccept.AcceptQuest();

        if (other.CompareTag("player"))
        {
            if (GameManager.gameManager.dialogueController.ActiveMainDialogue) return;

            if (!sameScene)
            {
                if (sceneName.Equals("")) return;

                GameManager.gameManager.SpawnpointID = spawnpointID;

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
