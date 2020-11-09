using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStateManager : MonoBehaviour
{
    //public GameObject firstFloor;
    //public GameObject secondFloor;

    //public Transform ffPos;
    //public Transform sfPos;

    //bool inFirstFloor = true;

    Player player;

    [Header("States")]
    public GameObject[] sceneStates = new GameObject[1];
    //public int changingState = -1;
    public Transform newPlayerPos;

    [Header("Conditions to Change State")]
    //public Quest questTobeCompleted;
    //public Quest questTobeAccepted;
    public SceneStateConditions[] sceneStateConditions = new SceneStateConditions[1];

    private void Start()
    {
        //if (questTobeCompleted.Completed && !questTobeAccepted.Accepted)
        //{
        //    ChangeSceneState(1);
        //}
        for (int i = 0; i < sceneStateConditions.Length; i++)
        {
            if (sceneStateConditions[i].CanChangeState())
            {
                Debug.Log("Carregando cena: " + sceneStateConditions[i].name);
                ChangeSceneState(sceneStateConditions[i].ChangingState, sceneStateConditions[i].MovePlayer, sceneStateConditions[i].FadeInOut);
                return;
            }
        }
    }

    public void ChangeSceneState(int s, bool movePlayer, bool fade)
    {
        StartCoroutine(ChangeState(s, movePlayer, fade));
    }

    IEnumerator ChangeState(int s, bool movePlayer = true, bool fade = true)
    {
        yield return new WaitForEndOfFrame();
        //if (s == changingState)
        //{
        //    if (movePlayer) StartCoroutine(ChangePlayerPosOnChange());
        //}
        if (movePlayer) StartCoroutine(ChangePlayerPosOnChange());

        if (fade)
        {
            GameManager.gameManager.MainHud.FadeInOut();
            yield return new WaitForSeconds(1.5f);
        }

        for (int i = 0; i < sceneStates.Length; i++)
        {
            sceneStates[i].SetActive(false);
        }
        sceneStates[s].SetActive(true);

        GameManager.gameManager.questController.SpawnAllQuestMarks();
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
        if (player == null) player = GameManager.gameManager.battleController.MainCharacter;
        player.EnableCharController(v);
        player.enabled = v;
    }

    void DisablePlayerMovement()
    {
        player.CanMove = false;
    }

    #region Navigation
    //public void GoToFirstFloor(Player p)
    //{
    //    Debug.Log("Indo primeiro andar");
    //    //AlternateFloors();
    //    secondFloor.SetActive(false);
    //    firstFloor.SetActive(true);
    //    inFirstFloor = true;
    //    p.transform.position = ffPos.position;
    //    Invoke("ActivePlayer", 0.5f);
    //}

    //public void GoToSecondFloor(Player p)
    //{
    //    Debug.Log("Indo segundo andar");
    //    //AlternateFloors();
    //    secondFloor.SetActive(true);
    //    firstFloor.SetActive(false);
    //    inFirstFloor = false;
    //    p.transform.position = sfPos.position;
    //    Invoke("ActivePlayer", 0.2f);
    //}

    ////void AlternateFloors()
    ////{
    ////    secondFloor.SetActive(inFirstFloor);
    ////    firstFloor.SetActive(!inFirstFloor);
    ////    inFirstFloor = !inFirstFloor;
    ////}

    //public void Navigate(Player p)
    //{
    //    player = p;
    //    Debug.Log("Navigando");
    //    p.enabled = false;
    //    if (inFirstFloor) GoToSecondFloor(p);
    //    else GoToFirstFloor(p);
    //}

    //void ActivePlayer()
    //{
    //    player.enabled = true;
    //}
    #endregion
}
