using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public Transform player;
    public Transform cam;

    private void Start()
    {
        Invoke("MovePlayer", 0.01f);
    }

    public void MovePlayer()
    {
        try
        {
            Transform currentSpawnP = transform.GetChild(GameManager.gameManager.SpawnpointID);
            Transform currentSpawnPCam = transform.GetChild(GameManager.gameManager.SpawnpointID).GetChild(0);

            player.transform.position = currentSpawnP.transform.position;
            player.transform.rotation = currentSpawnP.transform.rotation;

            cam.transform.position = currentSpawnPCam.transform.position;
            cam.transform.rotation = currentSpawnPCam.transform.rotation;
        }
        catch { Debug.Log("Não tem esse spawn;"); }
    }
}
