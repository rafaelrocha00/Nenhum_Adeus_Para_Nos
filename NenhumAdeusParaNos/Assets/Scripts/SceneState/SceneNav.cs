using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneNav : MonoBehaviour
{
    public GameObject firstFloor;
    public GameObject secondFloor;

    public Transform ffPos;
    public Transform sfPos;

    bool inFirstFloor = true;

    Player player;

    public void GoToFirstFloor(Player p)
    {
        Debug.Log("Indo primeiro andar");
        //AlternateFloors();
        secondFloor.SetActive(false);
        firstFloor.SetActive(true);
        inFirstFloor = true;
        p.transform.position = ffPos.position;
        p.MoveCompanions(ffPos.position);
        Invoke("ActivePlayer", 0.25f);
    }

    public void GoToSecondFloor(Player p)
    {
        Debug.Log("Indo segundo andar");
        //AlternateFloors();
        secondFloor.SetActive(true);
        firstFloor.SetActive(false);
        inFirstFloor = false;
        p.transform.position = sfPos.position;
        p.MoveCompanions(sfPos.position);
        Invoke("ActivePlayer", 0.25f);
    }

    //void AlternateFloors()
    //{
    //    secondFloor.SetActive(inFirstFloor);
    //    firstFloor.SetActive(!inFirstFloor);
    //    inFirstFloor = !inFirstFloor;
    //}

    public void Navigate(Player p)
    {
        player = p;
        Debug.Log("Navigando");
        p.enabled = false;
        //p.EnableCharController(false);
        if (inFirstFloor) GoToSecondFloor(p);
        else GoToFirstFloor(p);

        Invoke("SpawnQM", 0.02f);
    }

    void ActivePlayer()
    {
        player.enabled = true;
        //player.EnableCharController(true);
    }

    void SpawnQM()
    {
        GameManager.gameManager.questController.SpawnAllQM();
    }
}
