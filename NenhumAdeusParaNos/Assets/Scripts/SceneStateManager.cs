using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStateManager : MonoBehaviour
{
    public GameObject firstFloor;
    public GameObject secondFloor;

    public Transform ffPos;
    public Transform sfPos;

    bool inFirstFloor = true;

    Player player;

    [Header("States")]
    public GameObject[] sceneStates = new GameObject[1];
    public int changingState = -1;
    public Transform newPlayerPos;

    [Header("Conditions to Change State")]
    public Quest questTobeCompleted;
    public Quest questTobeAccepted;

    private void Start()
    {
        if (questTobeCompleted.Completed && !questTobeAccepted.Accepted)
        {
            ChangeSceneState(1);
        }
    }

    public void ChangeSceneState(int s)
    {
        StartCoroutine(ChangeState(s));
    }

    IEnumerator ChangeState(int s)
    {
        if (s == changingState)
        {
            StartCoroutine(ChangePlayerPosOnChange());
        }

        GameManager.gameManager.MainHud.FadeInOut();
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < sceneStates.Length; i++)
        {
            sceneStates[i].SetActive(false);
        }
        sceneStates[s].SetActive(true);
    }
    IEnumerator ChangePlayerPosOnChange()
    {
        ActivePlayer(false);
        DisablePlayerMovement();

        yield return new WaitForSeconds(1.5f);

        player.transform.position = newPlayerPos.position;
        player.transform.rotation = newPlayerPos.rotation;
        GameManager.gameManager.MainCamera.transform.position = newPlayerPos.GetChild(0).position;

        yield return new WaitForSeconds(1.5f);

        ActivePlayer(true);
    }

    void ActivePlayer(bool v)
    {
        player = GameManager.gameManager.battleController.MainCharacter;
        player.EnableCharController(v);
        player.enabled = v;
    }

    void DisablePlayerMovement()
    {
        player.CanMove = false;
    }

    #region Navigation
    public void GoToFirstFloor(Player p)
    {
        Debug.Log("Indo primeiro andar");
        //AlternateFloors();
        secondFloor.SetActive(false);
        firstFloor.SetActive(true);
        inFirstFloor = true;
        p.transform.position = ffPos.position;
        Invoke("ActivePlayer", 0.5f);
    }

    public void GoToSecondFloor(Player p)
    {
        Debug.Log("Indo segundo andar");
        //AlternateFloors();
        secondFloor.SetActive(true);
        firstFloor.SetActive(false);
        inFirstFloor = false;
        p.transform.position = sfPos.position;
        Invoke("ActivePlayer", 0.2f);
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
        if (inFirstFloor) GoToSecondFloor(p);
        else GoToFirstFloor(p);
    }

    void ActivePlayer()
    {
        player.enabled = true;
    }
    #endregion
}
