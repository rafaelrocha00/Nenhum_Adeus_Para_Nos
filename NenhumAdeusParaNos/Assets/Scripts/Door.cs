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

    public AudioClip clip_changeScene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("npc") && !other.isTrigger) { Destroy(other.gameObject); return; }

        if (other.CompareTag("player"))
        {
            if (quesToAccept != null && !quesToAccept.Accepted && quesToBeCompleted.Completed) quesToAccept.AcceptQuest();

            if (GameManager.gameManager.dialogueController.ActiveMainDialogue) return;

            if (!sameScene)
            {
                if (sceneName.Equals("")) return;

                //GameManager.gameManager.SpawnpointID = spawnpointID;

                //GameManager.gameManager.inventoryController.Inventory.SaveItems();
                //GameManager.gameManager.itemsSaver.BlockChestGen();

                //SceneManager.LoadScene(sceneName);

                GameManager.gameManager.ChangeScene(sceneName, spawnpointID);
                GameManager.gameManager.audioController.PlayEffect(clip_changeScene);
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
