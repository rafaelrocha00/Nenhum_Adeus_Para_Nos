using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public int mainSceneIndex;
    public InputField nameSpace;

    public void NewGame()
    {
        //SceneManager.LoadScene(mainSceneIndex);
        nameSpace.transform.parent.gameObject.SetActive(true);
    }

    public void LoadMainScene()
    {
        if (!nameSpace.text.Trim(' ').Equals(""))
        {
            GameManager.gameManager.PlayerName = nameSpace.text;
            SceneManager.LoadScene(mainSceneIndex);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
