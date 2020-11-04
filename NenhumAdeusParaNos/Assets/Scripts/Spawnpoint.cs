using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public Transform player;
    public Transform cam;

    private void Start()
    {
        // Invoke("MovePlayer", 0.01f);
        //MovePlayer();
        if (!GameManager.gameManager.NewGame) StartCoroutine(TeleportPlayer());
    }

    public void MovePlayer()
    {
        try
        {            
            Debug.Log("Movendo Player | Spawn: " + GameManager.gameManager.SpawnpointID);
            Transform currentSpawnP = transform.GetChild(GameManager.gameManager.SpawnpointID);
            Transform currentSpawnPCam = transform.GetChild(GameManager.gameManager.SpawnpointID).GetChild(0);

            if (currentSpawnP != null) player.GetComponent<Player>().EnableCharController(false);

            player.transform.position = currentSpawnP.transform.position;
            player.transform.rotation = currentSpawnP.transform.rotation;

            cam.transform.position = currentSpawnPCam.transform.position;
            cam.transform.rotation = currentSpawnPCam.transform.rotation;

            Debug.Log("Trocou posição do player");
        }
        catch { Debug.Log("Não tem esse spawn;"); }
    }

    IEnumerator TeleportPlayer()
    {

        MovePlayer();
        yield return new WaitForEndOfFrame();
        player.GetComponent<Player>().EnableCharController(true);
    }
}
