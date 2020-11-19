using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public int mainSceneIndex;

    public Image blackScreen;
    public GameObject menuUI;
    //public InputField nameSpace;

    //public void NewGame()
    //{
    //    //SceneManager.LoadScene(mainSceneIndex);
    //    //nameSpace.transform.parent.gameObject.SetActive(true);
    //    LoadMainScene();
    //}

    //public void LoadMainScene()
    //{
    //    //if (!nameSpace.text.Trim(' ').Equals(""))
    //    //{
    //        GameManager.gameManager.PlayerName = nameSpace.text;
    //        SceneManager.LoadScene(mainSceneIndex);
    //    //}
    //}

    private void Start()
    {
        if (!GameManager.gameManager.NewGame) this.gameObject.SetActive(false);
        else Invoke("DisableGame", 0.05f);
    }

    public void NewGame()
    {
        GameManager.gameManager.MainHud.ShowHiddenUI(true);
        StartCoroutine(FadeOut());
        menuUI.SetActive(false);

        GameManager.gameManager.MainHud.CheckIfWindowOpen();
        ChangeCursor(0);
    }
    
    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator FadeOut()
    {
        EnableGame(true);
        float timer = 0.0f;
        Color startColor = blackScreen.color;
        Color finalColor = new Color(0, 0, 0, 0);

        while (timer <= 1.0f)
        {
            blackScreen.color = Color.Lerp(startColor, finalColor, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        GameManager.gameManager.NewGame = false;
        gameObject.SetActive(false);
    }

    void DisableGame()
    {
        //if (GameManager.gameManager.NewGame)
            EnableGame(false);
    }

    void EnableGame(bool v)
    {
        GameManager.gameManager.MainHud.enabled = v;
        GameManager.gameManager.MainCamera.enabled = v;
        GameManager.gameManager.battleController.MainCharacter.enabled = v;
        if (v)
        {
            GameManager.gameManager.battleController.MainCharacter.EnableCharController(true);
            GameManager.gameManager.battleController.MainCharacter.CanMove = false;
        }
    }

    public void ChangeCursor(int state)
    {
        GameManager.gameManager.ChangeCursor(state);
    }

    public void ChangeScene(string sn)
    {
        SceneManager.LoadScene(sn);
    }
}
